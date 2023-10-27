using System;
using Microsoft.EntityFrameworkCore;

namespace project.Models.Data
{
    public class MyDBContext : DbContext
    {
        public MyDBContext(DbContextOptions<MyDBContext> options) : base(options)
        {
        }

        public DbSet<Recruitment> Recruitment { get; set; }

        public DbSet<Application> Application { get; set; }

        public DbSet<Assignment> Assignment { get; set; }

        public DbSet<Staff> Staff { get; set; }

        public DbSet<Student> Student { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure composite primary key for the Assignment entity
            modelBuilder.Entity<Assignment>()
                .HasKey(a => new { a.RecruitmentID, a.StudentID });
        }
    }
}

