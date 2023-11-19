using System;
using Microsoft.EntityFrameworkCore;

namespace project.Models.Data
{
	public class MyDBContext:DbContext
	{
		public MyDBContext(DbContextOptions<MyDBContext> options):base(options)
		{

		}

		public DbSet<Recruitment> Recruitment{ get; set;}

		public DbSet<Application> Application { get; set; }

		public DbSet<Assignment> Assignment { get; set; }
	
        public DbSet<Staff> Staff { get; set; }

        public DbSet<Student> Student { get; set; }

        public DbSet<Availability> Availability { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure composite primary key for the Assignment entity
            modelBuilder.Entity<Assignment>()
                .HasKey(a => new { a.RecruitmentID, a.StudentID });

            //modelBuilder.Entity<Recruitment>().Property(r => r.StartTime).HasColumnType("time");
            //modelBuilder.Entity<Recruitment>().Property(r => r.EndTime).HasColumnType("time");

            //modelBuilder.Entity<Recruitment>()
            //    .Property(r => r.StartTime)
            //    .HasConversion(
            //    timeOnly => timeOnly.ToTimeSpan(),
            //    timeSpan => TimeOnly.FromTimeSpan(timeSpan)
            //    );

            modelBuilder.Entity<Recruitment>(entity =>
            {
                entity.Property(r => r.JobShiftDate)
                .HasConversion(
                    v => v.HasValue ? v.Value.ToDateTime(TimeOnly.MinValue) : (DateTime?)null,
                    v => v.HasValue ? DateOnly.FromDateTime(v.Value) : (DateOnly?)null);

                entity.Property(r => r.StartTime)
                .HasConversion(
                timeOnly => timeOnly.ToTimeSpan(),
                timeSpan => TimeOnly.FromTimeSpan(timeSpan));

                entity.Property(r => r.EndTime)
                .HasConversion(
                timeOnly => timeOnly.ToTimeSpan(),
                timeSpan => TimeOnly.FromTimeSpan(timeSpan));
            });

            //modelBuilder.Entity<Availability>()
            //    .HasKey(a => new { a.StudentID, a.DateAvail, a.HourOfDay });

            modelBuilder.Entity<Availability>(entity =>
            {
                entity.HasKey(a => new { a.StudentID, a.DateAvail, a.HourOfDay });

                entity.Property(a => a.HourOfDay)
                .HasConversion(
                timeOnly => timeOnly.ToTimeSpan(),
                timeSpan => TimeOnly.FromTimeSpan(timeSpan)
                );

                entity.Property(a => a.DateAvail)
                .HasConversion(
                    v => v.HasValue ? v.Value.ToDateTime(TimeOnly.MinValue) : (DateTime?)null,
                    v => v.HasValue ? DateOnly.FromDateTime(v.Value) : (DateOnly?)null);
            });
        }
    }
}

