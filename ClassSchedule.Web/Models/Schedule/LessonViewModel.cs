using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ClassSchedule.Web.Models.Schedule
{
    public class LessonViewModel
    {
        public int DisciplineId { get; set; }
        public string DisciplineName { get; set; }

        public int ChairId { get; set; }
        public string ChairName { get; set; }
        public List<TeacherViewModel> ChairTeachers { get; set; } 

        public int DayNumber { get; set; }
        public int ClassNumber { get; set; }
        public int LessonTypeId { get; set; }

        public IEnumerable<LessonDetailViewModel> LessonDetails { get; set; } 
    }
}