using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ClassSchedule.Web.Models.Schedule
{
    public class TeacherLesson
    {
        public int DayNumber { get; set; }
        public int ClassNumber { get; set; }
        public string Discipline { get; set; }
        public bool IsLection { get; set; }
        public string Auditorium { get; set; }
        public string Group { get; set; }
    }
}