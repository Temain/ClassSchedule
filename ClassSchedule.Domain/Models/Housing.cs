using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClassSchedule.Domain.Models
{
    /// <summary>
    /// Учебный корпус
    /// </summary>
    [Table("Housing", Schema = "dict")]
    public class Housing
    {
        public int HousingId { get; set; }

        /// <summary>
        /// Наименование корпуса
        /// </summary>
        [StringLength(500)]
        public string HousingName { get; set; }

        /// <summary>
        /// Сокращение
        /// </summary>
        [StringLength(20)]
        public string Abbreviation { get; set; }

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
