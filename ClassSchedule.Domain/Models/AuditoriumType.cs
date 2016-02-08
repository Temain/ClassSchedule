using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        /// Аудитории
        /// </summary>
        public virtual ICollection<Auditorium> Auditoriums { get; set; } 
    }
}
