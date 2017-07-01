using System.Collections.Generic;

namespace ClassSchedule.Business.Models.Schedule
{
    public class LessonViewModel
    {
        public int LessonId { get; set; }

        public int DisciplineId { get; set; }
        public string DisciplineName { get; set; }

        public int ChairId { get; set; }
        public string ChairName { get; set; }
        public List<TeacherViewModel> ChairTeachers { get; set; } 

        public int DayNumber { get; set; }
        public int ClassNumber { get; set; }
        public int LessonTypeId { get; set; }

        public int? Order { get; set; }

        public IEnumerable<LessonDetailViewModel> LessonDetails { get; set; } 
    }
}