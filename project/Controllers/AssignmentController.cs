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
    [Route("[controller]")]
    public class AssignmentController : ControllerBase
    {
        private readonly MyDBContext _myDBContext;

        public AssignmentController(MyDBContext myDBContext)
        {
            _myDBContext = myDBContext;
        }

        //Function 3 - Allow staffs to approve students' applications
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
        public async Task<IActionResult> UpdateStudentReq(int RecruitmentID)
        {
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
            using (var transaction = _myDBContext.Database.BeginTransaction())
            {
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
        }


        // Greedy algo -- for auto replacement / recommendation
        // 1. initialise the system with the available job shifts and list of students
        // 2. select available students from the solution space
        // 3. remove any students from the solution space who have overlapping shift and current shift
        // 4. remove any students from teh solution space who have already reached the maximum of two job shifts per day
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
    }

}

