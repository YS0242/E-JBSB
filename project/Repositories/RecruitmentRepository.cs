using System;
using project.Models;
using project.Models.Data;
using project.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace project.Repository
{
    public class RecruitmentRepository : IRecruitmentRepository
    {
        private readonly MyDBContext _myDBContext;

        public RecruitmentRepository(MyDBContext myDBContext)
        {
            _myDBContext = myDBContext;
        }

        public async Task<List<Recruitment>> GetRecruitments()
        {
            return await _myDBContext.Recruitment.ToListAsync();
        }

        // Post Recruitment
        public async Task<Recruitment> AddRecruitment(Recruitment newRecruitment)
        {
            _myDBContext.Recruitment.Add(newRecruitment);
            await _myDBContext.SaveChangesAsync();
            return newRecruitment;
        }

        // Delete Recruitment
        public async Task<Recruitment?> GetRecruitmentByIDAsync(int RecruitmentID)
        {
            return await _myDBContext.Recruitment.FindAsync(RecruitmentID);
        }

        // !!! IEnumerable -- collection
        public async Task<IEnumerable<Assignment?>> GetAssignmentsAsync(int RecruitmentID)
        {
            return await _myDBContext.Assignment.Where(a => a.RecruitmentID == RecruitmentID).ToListAsync();
        }

        public async Task<IEnumerable<Application?>> GetApplicationsAsync(int RecruitmentID)
        {
            return await _myDBContext.Application.Where(a => a.RecruitmentID == RecruitmentID).ToListAsync();
        }

        public void RemoveAssignments(IEnumerable<Assignment?> assignments)
        {
            _myDBContext.Assignment.RemoveRange(assignments!);
        }

        public void RemoveApplications(IEnumerable<Application?> applications)
        {
            _myDBContext.Application.RemoveRange(applications!);
        }

        public async Task DeleteRecruitment(Recruitment recruitment)
        {
            _myDBContext.Recruitment.Remove(recruitment);
            await _myDBContext.SaveChangesAsync();
        }
    }
}

