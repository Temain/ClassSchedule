using System.Collections.Generic;

namespace ClassSchedule.Business.Models.Teacher
{
    public class TeacherLesson
    {
        public int WeekNumber { get; set; }
        public int DayNumber { get; set; }
        public int ClassNumber { get; set; }

        /// <summary>
        /// Это окно в расписании преподавателя
        /// </summary>
        public bool IsDowntime { get; set; }

        public IEnumerable<TeacherDiscipline> Disciplines { get; set; } 
    }
}