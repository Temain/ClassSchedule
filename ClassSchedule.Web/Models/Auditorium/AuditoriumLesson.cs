using System.Collections.Generic;
using ClassSchedule.Web.Models.Teacher;

namespace ClassSchedule.Web.Models.Auditorium
{
    public class AuditoriumLesson
    {
        public int WeekNumber { get; set; }
        public int DayNumber { get; set; }
        public int ClassNumber { get; set; }

        /// <summary>
        /// Это окно в расписании аудитории
        /// </summary>
        public bool IsDowntime { get; set; }

        public IEnumerable<AuditoriumDiscipline> Disciplines { get; set; } 
    }
}