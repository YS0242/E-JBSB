using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using project.Models;
using project.Models.Data;

namespace project.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RecruitmentController : ControllerBase
    {

        private readonly MyDBContext _myDBContext;

        public RecruitmentController(MyDBContext myDBContext)
        {
            _myDBContext = myDBContext;
        }

        //Show recruitments - not yet finalised
        [HttpGet]
        public async Task<IActionResult> GetRecruitmentData()
        {
            var recruitmentData = await _myDBContext.Recruitment.ToListAsync();
            return Ok(recruitmentData);

            //var recruitmentData = await _myDBContext.Recruitment.Where(r => r.JobShiftDate == "2023-11-12").ToListAsync();
            //return Ok(recruitmentData);
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



