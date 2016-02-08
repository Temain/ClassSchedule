using System;
using System.Collections.Generic;
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
        public string HousingName { get; set; }

        /// <summary>
        /// Аудитории
        /// </summary>
        public virtual ICollection<Auditorium> Auditoriums { get; set; } 
    }
}
