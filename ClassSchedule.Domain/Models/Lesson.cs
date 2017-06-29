using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClassSchedule.Domain.Models
{
    /// <summary>
    /// Занятие
    /// </summary>
    [Table("Lesson", Schema = "dbo")]
    public class Lesson
    {
        public Lesson()
        {
            CreatedAt = DateTime.Now;
            LessonGuid = Guid.NewGuid();
        }

        public int LessonId { get; set; }

        public Guid? LessonGuid { get; set; }

        /// <summary>
        /// Позиция в расписании
        /// </summary>
        public int ScheduleId { get; set; }
        public Schedule Schedule { get; set; }

        /// <summary>
        /// Тип занятия
        /// </summary>
        public int? LessonTypeId { get; set; }
        public LessonType LessonType { get; set; }

        /// <summary>
        /// Дисциплина
        /// </summary>
        public int DisciplineId { get; set; }
        public Discipline Discipline { get; set; }

        /// <summary>
        /// Порядок
        /// </summary>
        public int? Order { get; set; }

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
        /// В какой(их) аудиториях и какой(ие) преподаватели ведет(ут)
        /// </summary>
        public List<LessonDetail> LessonDetails { get; set; }
    }
}
