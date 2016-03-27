using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassSchedule.Domain.Models
{
    [Table("TempDiscipline", Schema = "tmp")]
    public class TempDiscipline
    {
        public int TempDisciplineId { get; set; }
        public string DisciplineName { get; set; }
        public int ChairCode { get; set; }
    }
}
