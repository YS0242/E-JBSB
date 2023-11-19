using System;
namespace project.Models
{
    public class Availability
    {
        public Availability()
        {
        }

        public int StudentID { get; set; }

        public DateOnly? DateAvail { get; set; }

        public TimeOnly HourOfDay { get; set; }

        public bool IsAvailable { get; set; }
    }
}

