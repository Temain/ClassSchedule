using System.Collections.Generic;

namespace ClassSchedule.Web.Models.Schedule
{
    public class LessonViewModel
    {
        public int? LessonId { get; set; }

        public int LessonTypeId { get; set; }
        public string LessonTypeName { get; set; }

        public int ChairId { get; set; }
        public string ChairName { get; set; }

        public int DisciplineId { get; set; }
        public string DisciplineName { get; set; }

        public int TeacherId { get; set; }
        public string TeacherFullName { get; set; }

        public int AuditoriumId { get; set; }
        public string AuditoriumName { get; set; }
        
        public bool IsNotActive { get; set; }
    }
}