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
    }
}
