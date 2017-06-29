using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClassSchedule.Domain.Models
{
    /// <summary>
    /// Тип занятие (лекция, семинар и т.д.)
    /// </summary>
    [Table("LessonType", Schema = "dict")]
    public class LessonType
    {
        public int LessonTypeId { get; set; }
        public Guid LessonTypeGuid { get; set; }
        public string LessonTypeName { get; set; }
        public int Order { get; set; }

        /// <summary>
        /// Дата последнего обновления записи
        /// </summary>
        public DateTime? UpdatedAt { get; set; }

        /// <summary>
        /// Дата удаления записи
        /// </summary>
        public DateTime? DeletedAt { get; set; }

        /// <summary>
        /// Занятия этого типа
        /// </summary>
        public List<Lesson> Lessons { get; set; } 
    }
}
