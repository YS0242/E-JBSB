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
    [Route("controller")]
    public class AssignmentController : ControllerBase
    {
        private readonly MyDBContext _myDBContext;

        public AssignmentController(MyDBContext myDBContext)
        {
            _myDBContext = myDBContext;
        }

        //allow staffs to approve students' applications
        [HttpPost]
        public async Task<IActionResult> ApprovalRequest(Assignment newAssignment)
        {
            _myDBContext.Assignment.Add(newAssignment);
            await _myDBContext.SaveChangesAsync();
            return Ok(newAssignment);
        }

        //Methods showing the assignment of studnets
        [HttpGet]
        public async Task<IActionResult> GetAssignment()
        {
            var assignments = await _myDBContext.Assignment.ToListAsync();
            return Ok(assignments);
        }

        //When the staffs approve the job application of any student, the stuNumReqRemain decrement by 1
        [HttpPut("{RecruitmentID}")]
        public async Task<IActionResult> UpdateStudentReq(int RecruitmentID, [FromBody] Recruitment updatedRecruitment)
        {
            Console.WriteLine($"RecruitmentID (Route): {RecruitmentID}");
            Console.WriteLine($"RecruitmentID (FromBody): {updatedRecruitment.RecruitmentID}");

            if (RecruitmentID != updatedRecruitment.RecruitmentID)
            {
                return BadRequest();
            }

            // Check if the Recruitment record with the given ID exists
            var existingRecruitment = await _myDBContext.Recruitment.FindAsync(RecruitmentID);
            if (existingRecruitment == null)
            {
                return NotFound();
            }

            // Update the properties of the existing Recruitment record
            existingRecruitment.StuNumReqRemain = existingRecruitment.StuNumReqRemain - 1;

            try
            {
                // Save the changes to the database
                await _myDBContext.SaveChangesAsync();
                return NoContent(); // 204 No Content
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RecruitmentExists(RecruitmentID))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }

        private bool RecruitmentExists(int RecruitmentID)
        {
            return _myDBContext.Recruitment.Any(e => e.RecruitmentID == RecruitmentID);
        }
    }

}

