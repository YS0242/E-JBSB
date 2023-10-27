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
    public class ApplicationController : ControllerBase
    {
        private readonly MyDBContext _myDBContext;

        public ApplicationController(MyDBContext myDBContext)
        {
            _myDBContext = myDBContext;
        }

        //Function 2 - Allow students to apply for job
        [HttpPost]
        public async Task<IActionResult> ApplicationRequest(Application newApplication)
        {
            //var newApplication = new Application
            //{
            //    ApplicationID = application.ApplicationID,
            //    RecruitmentID = application.RecruitmentID,
            //    StudentID = application.StudentID,
            //    ApplicationStatus = application.ApplicationStatus - 1
            //};

            _myDBContext.Application.Add(newApplication);
            await _myDBContext.SaveChangesAsync();
            return Ok(newApplication);
        }

        //Showing staffs the list of applicants
        [HttpGet]
        public async Task<IActionResult> GetApplicants()
        {
            var applicants = await _myDBContext.Application.ToListAsync();
            return Ok(applicants);
        }

        
    }
}

