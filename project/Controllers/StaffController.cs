using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using project.Models;
using project.Models.Data;

namespace project.Controllers
{
    [ApiController]
    [Route("staff/[controller]")]
    public class StaffController : ControllerBase
    {
        private readonly MyDBContext _myDBContext;

        public StaffController(MyDBContext myDBContext)
        {
            _myDBContext = myDBContext;
        }

        [HttpGet]
        public async Task<IActionResult> GetStaffs()
        {
            var staffs = await _myDBContext.Staff.ToListAsync();
            return Ok(staffs);
        }

        [HttpPost]
        public async Task<IActionResult> PostStudent(Staff newStaff)
        {
            _myDBContext.Staff.Add(newStaff);
            await _myDBContext.SaveChangesAsync();
            return Ok(newStaff);
        }
    }
}

