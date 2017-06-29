using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClassSchedule.Domain.Models
{
    /// <summary>
    /// Тип аудитории (лекционная, компьютерный класс и т.д.)
    /// </summary>
    [Table("AuditoriumType", Schema = "dict")]
    public class AuditoriumType
    {
        public int AuditoriumTypeId { get; set; }
        public string AuditoriumTypeName { get; set; }

        /// <summary>
        /// Дата последнего обновления записи
        /// </summary>
        public DateTime? UpdatedAt { get; set; }

        /// <summary>
        /// Дата удаления записи
        /// </summary>
        public DateTime? DeletedAt { get; set; }

        /// <summary>
        /// Аудитории
        /// </summary>
        public List<Auditorium> Auditoriums { get; set; } 
    }
}
