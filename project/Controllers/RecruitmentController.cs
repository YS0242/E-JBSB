//using System;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.Extensions.Configuration;
//using Microsoft.Data.SqlClient;
//using Microsoft.EntityFrameworkCore;
//using project.Models;

//namespace project.Controllers
//{
//    [ApiController]
//    [Route("api/[controller]")]
//    public class RecruitmentController : ControllerBase
//    {
//        private readonly YourDbContext _context; // Replace "YourDbContext" with your actual DbContext class name

//        public RecruitmentController(YourDbContext context) // Replace "YourDbContext" with your actual DbContext class name
//        {
//            _context = context;
//        }

//        [HttpPost]
//        public ActionResult CreateRecruitment(Recruitment recruitment)
//        {
//            try
//            {
//                _context.Recruitment.Add(recruitment);
//                _context.SaveChanges();
//                return Ok("Recruitment record created successfully.");
//            }
//            catch (Exception ex)
//            {
//                return BadRequest($"An error occurred: {ex.Message}");
//            }
//        }
//    }
//}

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
        //[HttpGet]
        ////public string GetRecruitment() {
        ////    return "Hello";
        ////}

        //public IEnumerable<Recruitment> GetValues()
        //{
        //    return new List<Recruitment>(){
        //        new Recruitment
        //        {
        //        StuNumReq = 5,
        //        JobShiftDate = "28/08/2024",
        //        StartTime = "0800",
        //        EndTime = "1200",
        //        JobDescription = "Packer",
        //        IsFCFS = 0
        //        },

        //        new Recruitment
        //        {
        //        StuNumReq = 6,
        //        JobShiftDate = "28/08/2024",
        //        StartTime = "0800",
        //        EndTime = "1200",
        //        JobDescription = "Packer",
        //        IsFCFS = 0
        //    }};
        //}


        //public IEnumerable<Recruitment> GetRecruitments(List<Recruitment> recruitments)
        //{
        //    //return recruitments{
        //    //    //new Recruitment
        //    //    //{
        //    //    //    //StuNumReq = 5,
        //    //    //    //JobShiftDate = "28/08/2024",
        //    //    //    //StartTime = "0800",
        //    //    //    //EndTime = "1200",
        //    //    //    //JobDescription = "Packer",
        //    //    //    //IsFCFS = 0
        //    //    //};
        //    //}
        //}

        private readonly MyDBContext _myDBContext;

        public RecruitmentController(MyDBContext myDBContext)
        {
            _myDBContext = myDBContext;
        }


        [HttpGet]

        public async Task<IActionResult> GetRecruitmentData()
        {
            var recruitmentData = await _myDBContext.Recruitment.ToListAsync();
            return Ok(recruitmentData);
        }
    }
}



