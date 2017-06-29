using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClassSchedule.Domain.Models
{
    /// <summary>
    /// Аудитория
    /// </summary>
    [Table("Auditorium", Schema = "dbo")]
    public class Auditorium
    {
        public int AuditoriumId { get; set; }

        public Guid AuditoriumGuid { get; set; }

        public string AuditoriumNumber { get; set; }

        /// <summary>
        /// Учебный корпус
        /// </summary>
        public int HousingId { get; set; }
        public Housing Housing { get; set; }

        /// <summary>
        /// Тип аудитории
        /// </summary>
        public int AuditoriumTypeId { get; set; }
        public AuditoriumType AuditoriumType { get; set; }

        /// <summary>
        /// Мест в аудитории
        /// </summary>
        public int? Places { get; set; }

        /// <summary>
        /// Кафедра
        /// </summary>
        public int? ChairId { get; set; }
        public Chair Chair { get; set; }

        /// <summary>
        /// Примечание
        /// </summary>
        [StringLength(100)]
        public string Comment { get; set; }

        /// <summary>
        /// Дата последнего обновления записи
        /// </summary>
        public DateTime? UpdatedAt { get; set; }

        /// <summary>
        /// Дата удаления записи
        /// </summary>
        public DateTime? DeletedAt { get; set; }

        /// <summary>
        /// Занятия в этой аудитории
        /// </summary>
        public List<LessonDetail> LessonDetails { get; set; }
    }
}
