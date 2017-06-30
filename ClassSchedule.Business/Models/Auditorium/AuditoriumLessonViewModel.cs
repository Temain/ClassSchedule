using System.Collections.Generic;

namespace ClassSchedule.Business.Models.Auditorium
{
    public class AuditoriumLessonViewModel
    {
        public int WeekNumber { get; set; }
        public int DayNumber { get; set; }
        public int ClassNumber { get; set; }

        /// <summary>
        /// Это окно в расписании аудитории
        /// </summary>
        public bool IsDowntime { get; set; }

        public IEnumerable<AuditoriumDisciplineViewModel> Disciplines { get; set; } 
    }
}