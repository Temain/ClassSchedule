using System.Collections.Generic;

namespace ClassSchedule.Web.Models.Schedule
{
    public class LessonPartViewModel
    {
        public int? LessonId { get; set; }

        public int DayNumber { get; set; }
        public int ClassNumber { get; set; }

        public int LessonTypeId { get; set; }
        public string LessonTypeName { get; set; }

        public int ChairId { get; set; }
        public string ChairName { get; set; }

        public int DisciplineId { get; set; }
        public string DisciplineName { get; set; }

        public int TeacherId { get; set; }
        public string TeacherLastName { get; set; }
        public string TeacherFirstName { get; set; }
        public string TeacherMiddleName { get; set; }

        public int HousingId { get; set; }
        public int AuditoriumId { get; set; }
        public string AuditoriumName { get; set; }
        public List<AuditoriumViewModel> Auditoriums { get; set; } 
        
        // public bool IsNotActive { get; set; }
    }
}