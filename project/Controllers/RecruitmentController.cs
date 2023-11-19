using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using project.Models;
using project.Models.Data;
using project.Services;

namespace project.Controllers
{
    [ApiController]
    [Route("recruitment/[controller]")]
    public class RecruitmentController : ControllerBase
    {
        private readonly MyDBContext _myDBContext;
        private readonly IRecruitmentService _recruitmentService;
        private readonly IEmailService _emailService;

        public RecruitmentController(MyDBContext myDBContext, IRecruitmentService recruitmentService, IEmailService emailService)
        {
            _myDBContext = myDBContext;
            _recruitmentService = recruitmentService;
            _emailService = emailService;
        }

        // Show the list of applicable recruitments 
        [HttpGet("student/{StudentID}")]
        public async Task<IActionResult> GetRecruitmentDataByStudentID(int StudentID)
        {
            var unavailableDurations = await _myDBContext.Availability
                .Where(a => a.StudentID == StudentID && a.IsAvailable == false)
                .Select(a => new { a.DateAvail, a.HourOfDay })
                .ToListAsync();

            var recruitmentDataList = await _myDBContext.Recruitment.ToListAsync();

            //var recruitment = recruitmentDataList
            //    .Where(r => !(unavailableDurations
            //    .Any(ud => r.JobShiftDate == ud.DateAvail
            //    && ((r.StartTime < ud.HourOfDay
            //    && r.EndTime <= ud.HourOfDay)
            //    || (r.StartTime >= (ud.HourOfDay.AddHours(1))
            //    && r.EndTime > (ud.HourOfDay.AddHours(1)))))))
            //    .ToList();

            // Select only those recruitments that are applicable to given StudentID
            var recruitment = recruitmentDataList
                .Where(r => !unavailableDurations
                .Any(ud => r.JobShiftDate == ud.DateAvail
                && !((r.StartTime < ud.HourOfDay
                && r.EndTime <= ud.HourOfDay)
                || (r.StartTime >= ud.HourOfDay.AddHours(1)))))
                .ToList();

            //        var recruitment = recruitmentDataList
            //.Where(r => !unavailableDurations
            //    .Any(ud => r.JobShiftDate == ud.DateAvail
            //        && (r.StartTime < ud.HourOfDay && r.EndTime <= ud.HourOfDay)
            //        || (r.StartTime >= ud.HourOfDay.AddHours(1) && r.EndTime > ud.HourOfDay.AddHours(1))))
            //.ToList();
            //var existingAssignments = await _myDBContext.Assignment
            //    .Where(a => a.StudentID == StudentID).Select(a => new {a.RecruitmentID, a.StudentID}).ToListAsync();

            //var existingAssignmentsSchedule = await _myDBContext.Recruitment.Where(r => r.RecruitmentID == existingAssignments.Any());

            var existingAssignments = await _myDBContext.Assignment
                .Where(a => a.StudentID == StudentID)
                .Join(_myDBContext.Recruitment, // Join with the Recruitment table
                assignment => assignment.RecruitmentID, // Join condition on RecruitmentID
                recruitment => recruitment.RecruitmentID, // Join condition on RecruitmentID
                (assignment, recruitment) => new // Select the result into a new object
                {
                    AssignmentRecruitmentID = assignment.RecruitmentID,
                    AssignmentStudentID = assignment.StudentID,
                    RecruitmentJobShiftDate = recruitment.JobShiftDate,
                    RecruitmentStartTime = recruitment.StartTime,
                    RecruitmentEndTime = recruitment.EndTime
                })
            .ToListAsync(); // Execute the query asynchronously and convert to a list
                
            // !!!!!Remove overlapped recruitments 
            //var filteredRecruitments = recruitment
            //    .Where(r => !existingAssignments
            //    .Any(ea => r.JobShiftDate == ea.RecruitmentJobShiftDate
            //    && !((r.StartTime < ea.RecruitmentStartTime && r.EndTime <= ea.RecruitmentStartTime)
            //    || (r.StartTime >= ea.RecruitmentEndTime && r.EndTime > ea.RecruitmentEndTime)))).ToList();

            //ATTENTION!!! Still need double check with the operators
            var filteredRecruitments = recruitment
                .Where(r => !existingAssignments
                .Any(ea => r.JobShiftDate == ea.RecruitmentJobShiftDate
                && ((r.StartTime >= ea.RecruitmentStartTime && r.StartTime < ea.RecruitmentEndTime)
                || (r.EndTime > ea.RecruitmentStartTime && r.EndTime <= ea.RecruitmentEndTime)
                || (r.StartTime < ea.RecruitmentStartTime && r.EndTime > ea.RecruitmentEndTime))))
                .ToList();


            //        var totalWorkingHours = await _myDBContext.Assignment
            //            .Where(a => a.StudentID == StudentID)
            //            .Join(_myDBContext.Recruitment, // Join with the Recruitment table
            //                assignment => assignment.RecruitmentID, // Join condition on RecruitmentID
            //                recruitment => recruitment.RecruitmentID, // Join condition on RecruitmentID
            //                (assignment, recruitment) => new // Project the result into a new object
            //                {
            //                  RecruitmentJobShiftDate = recruitment.JobShiftDate,
            //                  RecruitmentStartTime = recruitment.StartTime,
            //                  RecruitmentEndTime = recruitment.EndTime
            //              })
            //            .GroupBy(x => x.RecruitmentJobShiftDate) // Group by the JobShiftDate
            //            .Select(g => new // Project the result into a new object
            //         {
            //             JobShiftDate = g.Key,
            //                TotalWorkingMinutes = g.Sum(a => ( (a.EndTime - a.StartTime)).TotalMinutes)
            //            })
            //.ToList(); // Execute the query asynchronously and convert to a list

            // Calculate total daily working hours of a specified student
            var dailyWorkingHours = existingAssignments
                .GroupBy(a => a.RecruitmentJobShiftDate)
                .Select(g => new
                {
                    JobShiftDate = g.Key,
                    TotalWorkingHours = g.Sum(a => (a.RecruitmentEndTime - a.RecruitmentStartTime).TotalMinutes / 60.0)
                })
                .ToList();

            //// Calculate the total hours by summing the individual differences and converting minutes to hours
            //var grandTotalHours = totalWorkingHours.Sum(x => x.TotalWorkingHours) / 60.0;

            // Remove the recruitments that fall on the day where the exisiting total working hours of a specified student exceeded 8 hrs
            var finalRecruitments = filteredRecruitments
                .Where(fr =>
                {
                    var RecruitmentDurationHours = (fr.EndTime - fr.StartTime).TotalMinutes / 60.0;
                    var existingHoursForDate = dailyWorkingHours.FirstOrDefault(dwh => dwh.JobShiftDate == fr.JobShiftDate)?.TotalWorkingHours ?? 0;
                    return (existingHoursForDate + RecruitmentDurationHours) <= 8;
                }).ToList();

            return Ok(finalRecruitments);
        }

        // Show the list of all recruitments
        [HttpGet]
        public async Task<IActionResult> GetRecruitments()
        {
            var recruitmentData = await _recruitmentService.GetRecruitments();
            return Ok(recruitmentData);
        }

        //Function 1 - Post Job Recruitment OK
        [HttpPost]
        public async Task<IActionResult> PostRecruitment(Recruitment recruitment)
        {
            var newRecruitment = new Recruitment
            {
                StaffID = recruitment.StaffID,
                StuNumReq = recruitment.StuNumReq,
                JobShiftDate = recruitment.JobShiftDate,
                StartTime = recruitment.StartTime,
                EndTime = recruitment.EndTime,
                JobLocation = recruitment.JobLocation,
                JobDescription = recruitment.JobDescription,
                IsFCFS = recruitment.IsFCFS,
                StuNumReqRemain = recruitment.StuNumReq
            };

            _myDBContext.Recruitment.Add(newRecruitment);
            await _myDBContext.SaveChangesAsync();

            //_myDBContext.Recruitment.Add(newRecruitment);
            //await _myDBContext.SaveChangesAsync();
            //return Ok(newRecruitment);

            //var newRecruitment = await _recruitmentService.PostRecruitment(recruitment);

            var studentEmails = await _myDBContext.Student.Select(s => s.StudentEmail).ToArrayAsync();

            foreach (var email in studentEmails)// Send email to all students to remind new job recruitment posted
            {
                await _emailService.SendEmailAsync(email, "New Recruitment Available", "A new recruitment has been posted. Please check it out!");
            }
            return Ok(newRecruitment);
        }

        //Delete Recruitment
        [HttpDelete("{RecruitmentID}")]
        public async Task<IActionResult> DeleteRecruitment(int RecruitmentID)
        {
            // Check if the Recruitment record with the given ID exists
            var existingRecruitment = await _myDBContext.Recruitment.FindAsync(RecruitmentID);
            if (existingRecruitment == null)
            {
                return NotFound();
            }

            try
            {
                // Remove related Assignment records
                var existingAssignments = _myDBContext.Assignment.Where(a => a.RecruitmentID == RecruitmentID);
                _myDBContext.Assignment.RemoveRange(existingAssignments);

                // Remove related Application records
                var existingApplications = _myDBContext.Application.Where(app => app.RecruitmentID == RecruitmentID);
                _myDBContext.Application.RemoveRange(existingApplications);

                // Remove the Recruitment record from the database
                _myDBContext.Recruitment.Remove(existingRecruitment);

                await _myDBContext.SaveChangesAsync();
                return NoContent(); // 204 No Content
            }
            catch (Exception)
            {
                // Handle any exceptions that may occur during deletion
                return StatusCode(500); // Internal Server Error
            }
        }

    }
}



