using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using project.Models;
using project.Models.Data;
using Microsoft.EntityFrameworkCore;

namespace project.Controllers
{
    [ApiController]
    [Route("assignment/[controller]")]
    public class AssignmentController : ControllerBase
    {
        private readonly MyDBContext _myDBContext;

        public AssignmentController(MyDBContext myDBContext)
        {
            _myDBContext = myDBContext;
        }


        //allow staffs to approve students' applications

        ////Function 3 - Allow staffs to approve students' applications
        //[HttpPost]
        //public async Task<IActionResult> ApprovalRequest(Assignment newAssignment)
        //{
        //    var StuNumRemain = await _myDBContext.Recruitment
        //        .Where(r => r.RecruitmentID == newAssignment.RecruitmentID)
        //        .Select(r => new { r.StuNumReqRemain })
        //        .FirstOrDefaultAsync();

        //    if (StuNumRemain.StuNumReqRemain != null && StuNumRemain.StuNumReqRemain > 0)
        //    {
        //        _myDBContext.Assignment.Add(newAssignment);
        //        await _myDBContext.SaveChangesAsync();
        //        return Ok(newAssignment);
        //    }
        //    return BadRequest("Sorry: No more quota left.");

        //}



        ////When the staffs approve the job application of any student, the stuNumReqRemain decrement by 1
        //[HttpPut("{RecruitmentID}")]
        //public async Task<IActionResult> UpdateStudentReq(int RecruitmentID)
        //{
        //    // Check if the Recruitment record with the given ID exists
        //    var existingRecruitment = await _myDBContext.Recruitment.FindAsync(RecruitmentID);
        //    if (existingRecruitment == null)
        //    {
        //        return NotFound();
        //    }

        //    // Update the properties of the existing Recruitment record
        //    existingRecruitment.StuNumReqRemain = existingRecruitment.StuNumReqRemain - 1;

        //    try
        //    {
        //        // Save the changes to the database
        //        await _myDBContext.SaveChangesAsync();
        //        return NoContent(); // 204 No Content
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!RecruitmentExists(RecruitmentID))
        //        {
        //            return NotFound();
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }
        //}

        //private bool RecruitmentExists(int RecruitmentID)
        //{
        //    return _myDBContext.Recruitment.Any(e => e.RecruitmentID == RecruitmentID);
        //}

        // Approve job application

        //Methods showing the assignment of studnets
        [HttpGet]
        public async Task<IActionResult> GetAssignments()
        {
            var assignments = await _myDBContext.Assignment.ToListAsync();
            return Ok(assignments);
        }

        // When students apply for job, staffs approve job application
        [HttpPost]
        public async Task<IActionResult> ApproveApplication(Assignment newAssignment)
        {

            //    _myDBContext.Assignment.Add(newAssignment);
            //    await _myDBContext.SaveChangesAsync();
            //    return Ok(newAssignment);
            //}

            ////When the staffs approve the job application of any student, the stuNumReqRemain decrement by 1
            //[HttpPut("{RecruitmentID}")]
            //public async Task<IActionResult> UpdateStudentReq(int RecruitmentID, [FromBody] Recruitment updatedRecruitment)
            //{
            //    Console.WriteLine($"RecruitmentID (Route): {RecruitmentID}");
            //    Console.WriteLine($"RecruitmentID (FromBody): {updatedRecruitment.RecruitmentID}");

            //    if (RecruitmentID != updatedRecruitment.RecruitmentID)
            //    {
            //        return BadRequest();
            //    }

            //    // Check if the Recruitment record with the given ID exists
            //    var existingRecruitment = await _myDBContext.Recruitment.FindAsync(RecruitmentID);
            //    if (existingRecruitment == null)
            //    {
            //        return NotFound();
            //    }

            //    // Update the properties of the existing Recruitment record
            //    existingRecruitment.StuNumReqRemain = existingRecruitment.StuNumReqRemain - 1;

            using var transaction = await _myDBContext.Database.BeginTransactionAsync();

            try
            {
                var recruitment = await _myDBContext.Recruitment
                    .Where(r => r.RecruitmentID == newAssignment.RecruitmentID)
                    .FirstOrDefaultAsync();

                if (recruitment == null)
                {
                    return NotFound("Recruitment not found.");
                }

                if (recruitment.StuNumReqRemain > 0)
                {
                    // Decrement the StuNumReqRemain by 1
                    recruitment.StuNumReqRemain -= 1;

                    // Assign / approve student for a recruitment
                    _myDBContext.Assignment.Add(newAssignment);

                    // Save the changes
                    await _myDBContext.SaveChangesAsync();

                    // Commit the transaction
                    await transaction.CommitAsync();

                    return Ok(newAssignment);
                }
                else
                {
                    // Rollback the transaction if the quota is not enough
                    await transaction.RollbackAsync();
                    return BadRequest("Sorry: No more quota left.");
                }
            }
            catch (DbUpdateConcurrencyException)
            {
                // Rollback the transaction in case of a concurrency update issue
                await transaction.RollbackAsync();
                if (!RecruitmentExists(newAssignment.RecruitmentID))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            catch (Exception)
            {
                // Rollback the transaction for any other type of exception
                await transaction.RollbackAsync();
                throw;
            }
        }

        private bool RecruitmentExists(int RecruitmentID)
        {
            return _myDBContext.Recruitment.Any(e => e.RecruitmentID == RecruitmentID);
        }

        //[HttpDelete("{RecruitmentID}/{StudentID}")]
        //public async Task<IActionResult> DeleteAssignment(int RecruitmentID, int StudentID)
        //{
        //    // Check if the Recruitment record with the given ID exists
        //    var existingAssignment = await _myDBContext.Assignment.FindAsync(RecruitmentID, StudentID);
        //    var existingAssignment = await _myDBContext.Assignment.FindAsync(RecruitmentID, StudentID);
        //    if (existingRecruitment == null)
        //    {
        //        return NotFound();
        //    }

        //    try
        //    {//Since Application and Assignment Tables are using RecruitmentID as foreign key, RecruitmentID in the related
        //     //tables must be removed before removing from Recruitment table 
        //     // Remove related Assignment records
        //        var existingAssignments = _myDBContext.Assignment.Where(a => a.RecruitmentID == RecruitmentID);
        //        _myDBContext.Assignment.RemoveRange(existingAssignments);

        //        // Remove related Application records
        //        var existingApplications = _myDBContext.Application.Where(app => app.RecruitmentID == RecruitmentID);
        //        _myDBContext.Application.RemoveRange(existingApplications);

        //        // Remove the Recruitment record from the database
        //        _myDBContext.Recruitment.Remove(existingRecruitment);

        //        await _myDBContext.SaveChangesAsync();
        //        return NoContent(); // 204 No Content
        //    }
        //    catch (Exception)
        //    {
        //        // Handle any exceptions that may occur during deletion
        //        return StatusCode(500); // Internal Server Error
        //    }
        //}

        //Function 8 - Job Assignment Cancellation
        //
        [HttpDelete("{RecruitmentID}/{StudentID}")]
        public async Task<IActionResult> DeleteAssignment(int RecruitmentID, int StudentID)
        {
            //If all database operations within that scope succeed, the transaction will be committed 
            using var transaction = _myDBContext.Database.BeginTransaction();
            try
            {
                // Find the Assignment record with the specified compound primary key
                var existingAssignment = await _myDBContext.Assignment
                    .FindAsync(RecruitmentID, StudentID);

                if (existingAssignment == null)
                {
                    return NotFound();
                }

                // Remove the Assignment record
                _myDBContext.Assignment.Remove(existingAssignment);

                await _myDBContext.SaveChangesAsync();

                // Commit the transaction if everything was successful
                transaction.Commit();

                return NoContent(); // 204 No Content
            }
            catch (Exception)
            {
                // Rollback the transaction if an exception occurs during deletion
                transaction.Rollback();
                return StatusCode(500); // Internal Server Error
            }
        }


        // Greedy algo -- for auto replacement / recommendation
        // 1. initialise the system with the available job shifts and list of students
        // 2. select available students from the solution space
        // 3. remove any students from the solution space who have overlapping shift
        // 4. remove any students from the solution space who have already reached the maximum of two job shifts per day
        // 5. remove any students from the solution space who have already reached the maximum working hours per day (8 hrs)

        //[HttpPut("{RecruitmentID}")]
        //public async Task<IActionResult> UpdateStudentReq(int RecruitmentID, [FromBody] Recruitment updatedRecruitment)
        //{
        //    Console.WriteLine($"RecruitmentID (Route): {RecruitmentID}");
        //    Console.WriteLine($"RecruitmentID (FromBody): {updatedRecruitment.RecruitmentID}");

        //    if (RecruitmentID != updatedRecruitment.RecruitmentID)
        //    {
        //        return BadRequest();
        //    }

        //    // Check if the Recruitment record with the given ID exists
        //    var existingRecruitment = await _myDBContext.Recruitment.FindAsync(RecruitmentID);
        //    if (existingRecruitment == null)
        //    {
        //        return NotFound();
        //    }

        //    // Update the properties of the existing Recruitment record
        //    existingRecruitment.StuNumReqRemain = existingRecruitment.StuNumReqRemain - 1;

        //    try
        //    {
        //        // Save the changes to the database
        //        await _myDBContext.SaveChangesAsync();
        //        return NoContent(); // 204 No Content
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!RecruitmentExists(RecruitmentID))
        //        {
        //            return NotFound();
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }
        //}

        // Get available student for specified job recruitment
        //[HttpGet("Availability/{RecruitmentID}")]
        //public async Task<IActionResult> GetAvailableStudent(int RecruitmentID)
        //{
        //    var recruitmentData = await _myDBContext.Recruitment
        //        .Where(r => r.RecruitmentID == RecruitmentID)
        //        .Select(r => new { r.JobShiftDate, r.StartTime, r.EndTime })
        //        .FirstOrDefaultAsync();

        //    var availableStudent = await _myDBContext.Availability
        //        .Where(a => a.DateAvail == recruitmentData.JobShiftDate
        //        && a.IsAvailable == 0 
        //        && !((recruitmentData.StartTime < a.HourOfDay
        //        && recruitmentData.EndTime <= a.HourOfDay)
        //        || (recruitmentData.StartTime >= a.HourOfDay.AddHours(1)
        //        && recruitmentData.EndTime > a.HourOfDay.AddHours(1))))
        //        .ToListAsync();

        //    return Ok(availableStudent);
        //}

        // To get the list of available students for a specific job recruitment
        [HttpGet("Availability/{RecruitmentID}")]
        public async Task<IActionResult> GetAvailableStudents(int RecruitmentID)
        {
            // Retrieve recruitment data for specified RecruitmentID
            var recruitmentData = await _myDBContext.Recruitment
                .Where(r => r.RecruitmentID == RecruitmentID)
                .Select(r => new { r.JobShiftDate, r.StartTime, r.EndTime })
                .FirstOrDefaultAsync();

            if (recruitmentData == null)
            {
                return NotFound($"Recruitment with ID {RecruitmentID} not found.");
            }

            // Calculate the number of continuous hours needed for the job shift (for TimeOnly types)
            TimeSpan jobDuration = recruitmentData.EndTime - recruitmentData.StartTime;
            int hoursNeeded = (int)Math.Ceiling(jobDuration.TotalHours);

            // Get all available time blocks for the job shift date
            var availableBlocks = await _myDBContext.Availability
                .Where(a => a.DateAvail == recruitmentData.JobShiftDate
                && a.IsAvailable == true)
                .OrderBy(a => a.StudentID)
                .ThenBy(a => a.HourOfDay)
                .ToListAsync();

            var availableStudents = new List<int>(); 

            // Check each student's availability for continuous blocks
            var studentsGroupedAvailability = availableBlocks
                .GroupBy(a => a.StudentID);

            foreach (var studentAvailability in studentsGroupedAvailability)
            {
                var blocks = studentAvailability
                    .OrderBy(a => a.HourOfDay).ToList();

                var continuousBlocks = new List<Availability>(); // This list holds the continuous blocks for checking

                // Build a list of continuous blocks
                foreach (var block in blocks)
                {
                    if (!continuousBlocks.Any() || block.HourOfDay == continuousBlocks.Last().HourOfDay.AddHours(1))
                    {
                        continuousBlocks.Add(block);
                    }
                    else
                    {
                        // If there's a gap, reset the list
                        continuousBlocks = new List<Availability> { block };
                        //continuousBlocks.Clear();
                        //continuousBlocks.Add(block);
                    }                    

                    // Check if we have enough continuous blocks
                    if (continuousBlocks.Count >= hoursNeeded
                        && continuousBlocks.First().HourOfDay <= recruitmentData.StartTime
                        && continuousBlocks.Last().HourOfDay.AddHours(1) >= recruitmentData.EndTime)
                    {
                        availableStudents.Add(studentAvailability.Key);
                        break;
                    }
                }

                //var existingAssignments = await _myDBContext.Assignment
                //        .Where(a => availableStudents.Contains(a.StudentID))
                //        .Join(_myDBContext.Recruitment, // Join with the Recruitment table
                //        assignment => assignment.RecruitmentID, // Join condition on RecruitmentID
                //        recruitment => recruitment.RecruitmentID, // Join condition on RecruitmentID
                //        (assignment, recruitment) => new // Select the result into a new object
                //        {
                //            AssignmentRecruitmentID = assignment.RecruitmentID,
                //            AssignmentStudentID = assignment.StudentID,
                //            RecruitmentJobShiftDate = recruitment.JobShiftDate,
                //            RecruitmentStartTime = recruitment.StartTime,
                //            RecruitmentEndTime = recruitment.EndTime
                //        })
                //    .ToListAsync(); // Execute the query asynchronously and convert to a list

            }
            //***** double check ya
            //var existingAssignments = await _myDBContext.Assignment
            //    .Where(a => availableStudents.Contains(a.StudentID) && a.RecruitmentID == RecruitmentID)
            //    .Select(a => a.StudentID, a.RecruitmentID )
            //    .ToListAsync(); // Execute the query asynchronously and convert to a list

            var existingAssignments = await _myDBContext.Assignment
                .Where(a => availableStudents.Contains(a.StudentID))
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

            //!!!!!
            //            availableStudents = availableStudents.Where(a => !existingAssignments
            //                .Any(ea => a.JobShiftDate == ea.RecruitmentJobShiftDate
            //                && !((a.StartTime < ea.RecruitmentStartTime && a.EndTime <= ea.RecruitmentEndTime)
            //                || (a.StartTime >= ea.RecruitmentEndTime && a.EndTime > ea.RecruitmentEndTime)))).ToList();

            //            var availableStudentAvailabilities = // join availableStudents with their availabilities to get JobShiftDate, StartTime, and EndTime

            //availableStudents = availableStudents
            //    .Where(studentId => !existingAssignments
            //        .Any(ea => availableStudentAvailabilities.Any(asa => asa.StudentID == studentId
            //        && asa.JobShiftDate == ea.RecruitmentJobShiftDate
            //        && !((asa.StartTime < ea.RecruitmentStartTime && asa.EndTime <= ea.RecruitmentEndTime)
            //        || (asa.StartTime >= ea.RecruitmentEndTime && asa.EndTime > ea.RecruitmentEndTime))
            //    ))).ToList();

            // Remove students with conflicting assignments
            //availableStudents = availableStudents
            //    .Where(StudentID => !existingAssignments
            //    .Any(assignment => assignment.AssignmentStudentID == StudentID
            //    && assignment.RecruitmentJobShiftDate == recruitmentData.JobShiftDate
            //    && ((assignment.RecruitmentStartTime < recruitmentData.EndTime
            //    && assignment.RecruitmentEndTime > recruitmentData.StartTime)
            //    || (assignment.RecruitmentEndTime > recruitmentData.StartTime
            //    && assignment.RecruitmentStartTime < recruitmentData.EndTime))))
            //    .ToList();

            //availableStudents = availableStudents
            //    .Where(StudentID => !existingAssignments
            //    .Any(assignment => assignment.AssignmentStudentID == StudentID
            //    && assignment.RecruitmentJobShiftDate == recruitmentData.JobShiftDate
            //    && !((assignment.RecruitmentStartTime < recruitmentData.StartTime
            //    && assignment.RecruitmentEndTime <= recruitmentData.StartTime)
            //    || (assignment.RecruitmentStartTime >= recruitmentData.EndTime
            //    && assignment.RecruitmentEndTime > recruitmentData.EndTime))))
            //    .ToList();

            availableStudents = availableStudents // Best
                .Where(StudentID => !existingAssignments
                .Any(assignment => assignment.AssignmentStudentID == StudentID
                && assignment.RecruitmentJobShiftDate == recruitmentData.JobShiftDate
                && ((assignment.RecruitmentStartTime >= recruitmentData.StartTime
                && assignment.RecruitmentStartTime < recruitmentData.EndTime)
                || (assignment.RecruitmentEndTime > recruitmentData.StartTime
                && assignment.RecruitmentEndTime <= recruitmentData.EndTime)
                || (assignment.RecruitmentStartTime < recruitmentData.EndTime
                && assignment.RecruitmentEndTime > recruitmentData.StartTime))))
                .ToList();

            //var dailyWorkingHours = existingAssignments
            //    .GroupBy(a => a.RecruitmentJobShiftDate)
            //    .Select(g => new
            //    {
            //        JobShiftDate = g.Key,
            //        TotalWorkingHours = g.Sum(a => (a.RecruitmentEndTime - a.RecruitmentStartTime).TotalMinutes / 60.0)
            //    })
            //    .ToList();

            //var dailyWorkingHours = existingAssignments
            //    .GroupBy(a => a.RecruitmentJobShiftDate)
            //    .Select(g => new
            //    {
            //        JobShiftDate = g.Key,
            //        TotalWorkingHours = g.Sum(a => (a.RecruitmentEndTime - a.RecruitmentStartTime).TotalMinutes / 60.0)
            //    })
            //    .ToList();

            // Assuming availableStudents is a List<int> of student IDs
            //var dailyWorkingHoursByStudent = availableStudents
            //    .Select(studentID => new
            //    {
            //        StudentID = studentID,
            //        DailyWorkingHours = existingAssignments
            //            .Where(a => a.AssignmentStudentID == studentID)
            //            .GroupBy(a => a.RecruitmentJobShiftDate)
            //            .Select(g => new
            //            {
            //                JobShiftDate = g.Key,
            //                TotalWorkingHours = g.Sum(a => (a.RecruitmentEndTime - a.RecruitmentStartTime).TotalMinutes / 60.0)
            //            })
            //            .ToList() // This will create a list for each student
            //    })
            //    .ToList(); // This will create a list of objects for all available students

            var dailyWorkingHours = existingAssignments
                .Where(a => availableStudents.Contains(a.AssignmentStudentID) && a.RecruitmentJobShiftDate == recruitmentData.JobShiftDate) // Filter for available students
                .GroupBy(a => new { a.AssignmentStudentID, a.RecruitmentJobShiftDate }) // Group by student ID and job shift date
                .Select(g => new
                {
                    StudentID = g.Key.AssignmentStudentID,
                    JobShiftDate = g.Key.RecruitmentJobShiftDate,
                    TotalWorkingHours = g.Sum(a => (a.RecruitmentEndTime - a.RecruitmentStartTime).TotalMinutes / 60.0)
                })
                .ToList();

            //availableStudents = availableStudents
            //    .Where(a =>
            //    {
            //        var RecruitmentDurationHours = (fr.EndTime - fr.StartTime).TotalMinutes / 60.0;
            //        var existingHoursForDate = dailyWorkingHours.FirstOrDefault(dwh => dwh.JobShiftDate == fr.JobShiftDate)?.TotalWorkingHours ?? 0;
            //        return (existingHoursForDate + RecruitmentDurationHours) <= 8;
            //    }).ToList();

            // Get the IDs of students who are exceeding the working hours limit when the current recruitment is included.
            var studentsExceedingHours = dailyWorkingHours
                .Where(dwh => dwh.TotalWorkingHours + hoursNeeded >= 8)
                .Select(dwh => dwh.StudentID)
                .ToList();

            // Filter the availableStudents list to exclude those students.
            availableStudents = availableStudents
                .Where(studentId => !studentsExceedingHours.Contains(studentId))
                .ToList();

            return Ok(availableStudents);
        }

        
    }

}

