using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClassSchedule.Domain.Models
{
    /// <summary>
    /// Время занятия
    /// На данный момент используется лишь как вспомогательная таблица
    /// </summary>
    [Table("ClassTime", Schema = "dict")]
    public class ClassTime
    {
        [Key, Column(Order = 0)]
        public int DayNumber { get; set; }

        [Key, Column(Order = 1)]
        public int ClassNumber { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        /// <summary>
        /// Дата последнего обновления записи
        /// </summary>
        public DateTime? UpdatedAt { get; set; }

        /// <summary>
        /// Дата удаления записи
        /// </summary>
        public DateTime? DeletedAt { get; set; }

        /// <summary>
        /// Позиция в расписании
        /// </summary>
        public List<Schedule> Schedule { get; set; }
    }
}
