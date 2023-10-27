using System;
using project.Models;

namespace project.Repositories
{
    public interface IRecruitmentRepository
    {
        Task<List<Recruitment>> GetRecruitments();
        Task<Recruitment> AddRecruitment(Recruitment recruitment);
        Task<Recruitment?> GetRecruitmentByIDAsync(int RecruitmentID);
        Task<IEnumerable<Assignment?>> GetAssignmentsAsync(int RecruitmentID);
        Task<IEnumerable<Application?>> GetApplicationsAsync(int RecruitmentID);
        void RemoveAssignments(IEnumerable<Assignment?> assignments);
        void RemoveApplications(IEnumerable<Application?> applications);
        Task DeleteRecruitment(Recruitment recruitment);
    }


}

