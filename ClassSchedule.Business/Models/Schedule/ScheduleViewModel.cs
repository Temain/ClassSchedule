using System;
using System.Collections.Generic;

namespace ClassSchedule.Business.Models.Schedule
{
    public class ScheduleViewModel
    {
        public int ScheduleId { get; set; }

        /// <summary>
        /// Номер недели
        /// </summary>
        public int WeekNumber { get; set; }

        /// <summary>
        /// Номер дня недели
        /// </summary>
        public int DayNumber { get; set; }

        /// <summary>
        /// Номер пары
        /// </summary>
        public int ClassNumber { get; set; }

        /// <summary>
        /// Дата занятия
        /// </summary>
        public DateTime ClassDate { get; set; }

        /// <summary>
        /// Группа
        /// </summary>
        public int GroupId { get; set; }
        public string GroupName { get; set; }

        /// <summary>
        /// Учебный год
        /// </summary>
        public int? EducationYearId { get; set; }
        public string EducationYear { get; set; }

        /// <summary>
        /// Неактивно
        /// </summary>
        public bool IsNotActive { get; set; }

        /// <summary>
        /// Корпуса
        /// </summary>
        public List<HousingViewModel> Housings { get; set; }

        /// <summary>
        /// Типы занятий
        /// </summary>
        public List<LessonTypeViewModel> LessonTypes { get; set; }

        /// <summary>
        /// Занятия
        /// </summary>
        public List<LessonViewModel> Lessons { get; set; }
    }
}