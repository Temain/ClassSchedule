using System;
using System.Collections.Generic;

namespace ClassSchedule.Web.Models.Schedule
{
    public class ScheduleViewModel
    {
        public string FacultyName { get; set; }

        /// <summary>
        /// Номер недели (по порядку)
        /// </summary>
        public int WeekNumber { get; set; }

        /// <summary>
        /// Первый день недели
        /// </summary>
        public DateTime FirstDayOfWeek { get; set; }

        /// <summary>
        /// Последний день недели
        /// </summary>
        public DateTime LastDayOfWeek { get; set; }

        public List<GroupLessonsViewModel> GroupLessons { get; set; }
    }
}