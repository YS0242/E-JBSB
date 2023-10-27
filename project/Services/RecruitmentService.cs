using System;
using Microsoft.AspNetCore.Mvc;
using project.Models;
using project.Models.Data;
using project.Repositories;

namespace project.Services
{
    public class RecruitmentService : IRecruitmentService
    {
        private readonly IRecruitmentRepository _repository;
        private readonly MyDBContext _myDBContext;

        public RecruitmentService(IRecruitmentRepository repository, MyDBContext myDBContext)
        {
            _repository = repository;
            _myDBContext = myDBContext;
        }

        public async Task<List<Recruitment>> GetRecruitments()
        {
            return await _repository.GetRecruitments();
        }

        public async Task<Recruitment> PostRecruitment(Recruitment recruitment)
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

            return await _repository.AddRecruitment(newRecruitment);
        }

        public async Task<IActionResult> DeleteRecruitment(int RecruitmentID)
        { 
            // Check if the Recruitment record with the given ID exists
            var existingRecruitment = await _repository.GetRecruitmentByIDAsync(RecruitmentID);
            if (existingRecruitment == null)
            {
                return new NotFoundResult();
            }

            try
            {// Since Application and Assignment Tables are using RecruitmentID as foreign key, RecruitmentID in the related
             // tables must be removed before removing from Recruitment table 

                var existingAssignments = await _repository.GetAssignmentsAsync(RecruitmentID);
                var existingApplications = await _repository.GetApplicationsAsync(RecruitmentID);

                // Remove related Assignment records
                if (existingAssignments != null)
                {
                    _repository.RemoveAssignments(existingAssignments);
                }


                // Remove related Application records

                if (existingApplications != null)
                {
                    _repository.RemoveApplications(existingApplications);
                }

                // Remove the Recruitment record from the database
                await _repository.DeleteRecruitment(existingRecruitment);

                return new NoContentResult(); // 204 No Content
            }
            catch (Exception)
            {
                // Handle any exceptions that may occur during deletion
                return new StatusCodeResult(500); // Internal Server Error
            }
        }
    }

}


