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
        /// Аудитории
        /// </summary>
        public virtual ICollection<Auditorium> Auditoriums { get; set; } 
    }
}
