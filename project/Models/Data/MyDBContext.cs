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
	}
}

