using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ClassSchedule.Web.Models.Schedule
{
    public class EditLessonViewModel
    {
        public int GroupId { get; set; }
        public int WeekNumber { get; set; }
        public int DayNumber { get; set; }
        public int ClassNumber { get; set; }

        public List<LessonPartViewModel> LessonParts { get; set; } 
    }
}