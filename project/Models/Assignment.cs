﻿using System;
using System.ComponentModel.DataAnnotations;

namespace project.Models
{
	public class Assignment
	{
		public Assignment()
		{
		}

		[Key]
		public int RecruitmentID { get; set; }

		public int StudentID { get; set; }
	}
}

