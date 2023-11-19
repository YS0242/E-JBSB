using System;
using Microsoft.AspNetCore.Mvc;
using project.Models;

namespace project.Services
{
    public interface IRecruitmentService
    {
        Task<List<Recruitment>> GetRecruitments();
        Task<Recruitment> PostRecruitment(Recruitment recruitment);
        Task<IActionResult> DeleteRecruitment(int RecruitmentID);
    }
}

