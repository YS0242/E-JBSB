using System;
using Microsoft.EntityFrameworkCore;

namespace project.Models.Data
{
	public class MyDBContext:DbContext
	{
		public MyDBContext(DbContextOptions<MyDBContext> options):base(options)
		{

		}

		public DbSet<Recruitment> Recruitment{get;set;}
	}
}

