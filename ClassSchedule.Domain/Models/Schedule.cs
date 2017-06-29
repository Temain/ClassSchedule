using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClassSchedule.Domain.Models
{
    /// <summary>
    /// Расписание
    /// </summary>
    [Table("Schedule", Schema = "dbo")]
    public class Schedule
    {
        public Schedule()
        {
            CreatedAt = DateTime.Now;
        }

        public int ScheduleId { get; set; }

        public Guid? ScheduleGuid { get; set; }

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
        /// Время занятия
        /// </summary>
        public ClassTime ClassTime { get; set; }

        /// <summary>
        /// Группа
        /// </summary>
        public int GroupId { get; set; }
        public Group Group { get; set; }

        /// <summary>
        /// Учебный год
        /// </summary>
        public int? EducationYearId { get; set; }
        public EducationYear EducationYear { get; set; }

        /// <summary>
        /// Неактивно
        /// </summary>
        public bool IsNotActive { get; set; }

        /// <summary>
        /// Дата последнего обновления записи
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Дата последнего обновления записи
        /// </summary>
        public DateTime? UpdatedAt { get; set; }

        /// <summary>
        /// Дата удаления записи
        /// </summary>
        public DateTime? DeletedAt { get; set; }

        /// <summary>
        /// Занятия
        /// </summary>
        public List<Lesson> Lessons { get; set; }
    }
}
