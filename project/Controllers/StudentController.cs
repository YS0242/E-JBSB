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
    [Route("student/[controller]")]
    public class StudentController : ControllerBase
    {
        private readonly MyDBContext _myDBContext;

        public StudentController(MyDBContext myDBContext)
        {
            _myDBContext = myDBContext;
        }

        [HttpGet]
        public async Task<IActionResult> GetStudents()
        {
            var students = await _myDBContext.Student.ToListAsync();
            return Ok(students);
        }

        [HttpPost]
        public async Task<IActionResult> PostStudent(Student newStudent)
        {
            _myDBContext.Student.Add(newStudent);
            await _myDBContext.SaveChangesAsync();
            return Ok(newStudent);
        }
    }
}

