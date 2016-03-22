using System.Collections.Generic;

namespace ClassSchedule.Web.Models.Schedule
{
    public class LessonViewModel
    {
        public int[] LessonIds { get; set; }

        public int LessonTypeId { get; set; }
        public string LessonTypeName { get; set; }
        public int ClassNumber { get;set; }

        public int ChairId { get; set; }
        public int DisciplineId { get; set; }
        public string DisciplineName { get; set; }

        public Dictionary<int, string> Teachers { get; set; }

        public int AuditoriumId { get; set; }
        public string AuditoriumName { get; set; }

        public bool IsActive { get; set; }
    }
}