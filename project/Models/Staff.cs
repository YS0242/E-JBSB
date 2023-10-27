using System;
namespace project.Models
{
	public class Staff
	{
		public Staff()
		{
		}

		public string? StaffID { get; set; }

		public string? StaffName { get; set; }

		public string? StaffPassword { get; set; }

		public string? StaffType { get; set; }

		public int DepartmentID { get; set; }

		public string? StaffEmail { get; set; }
	}
}

