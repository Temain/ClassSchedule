using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public virtual Housing Housing { get; set; }

        /// <summary>
        /// Тип аудитории
        /// </summary>
        public int AuditoriumTypeId { get; set; }
        public virtual AuditoriumType AuditoriumType { get; set; }

        /// <summary>
        /// Мест в аудитории
        /// </summary>
        public int? Places { get; set; }

        /// <summary>
        /// Кафедра
        /// </summary>
        public int? ChairId { get; set; }
        public virtual Chair Chair { get; set; }

        /// <summary>
        /// Примечание
        /// </summary>
        [StringLength(100)]
        public string Comment { get; set; }

        public bool IsDeleted { get; set; }

        /// <summary>
        /// Занятия в этой аудитории
        /// </summary>
        public virtual ICollection<Lesson> Lessons { get; set; } 
    }
}
