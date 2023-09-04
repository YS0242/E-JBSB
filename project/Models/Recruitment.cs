using System;
using System.ComponentModel.DataAnnotations;

namespace project.Models
{
	public class Recruitment
	{

        [Required]
        public int RecruitmentID { get; set; }

        [Required]
        public string RecruitmentPoster { get; set; }

        [Required]
        public int StuNumReq { get; set; }// Number of students required

        [Required]
        public string JobShiftDate { get; set; }// Job shift date

        [Required]
		public string StartTime { get; set; }// Job shift start time

        [Required]
		public string EndTime { get; set; }// Job shift end time

        [Required]
		public string JobDescription { get; set; }// Job description

        [Required]
		public int IsFCFS { get; set; }// Is first come first serve?

       

        //public int StuNumReq { get; set; }// Number of students required

        //public DateOnly JobShiftDate { get; set; }// Job shift date

        //public TimeOnly StartTime { get; set; }// Job shift start time

        //public TimeOnly EndTime { get; set; }// Job shift end time

        //public string JobDescription { get; set; }// Job description

        //public Boolean IsFCFS { get; set; }// Is first come first serve?


        //public Boolean ShiftSlot { get; set; }// Morning slot (830am - 1230am) or Afternoon slot (230pm - 630pm) 
    }
}
