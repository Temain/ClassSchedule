using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassSchedule.Domain.Models
{
    /// <summary>
    /// Тип контроля экзамена
    /// </summary>
    [Table("SessionControlType", Schema = "dict")]
    public class SessionControlType
    {
        public int SessionControlTypeId { get; set; }
        public string SessionControlTypeName { get; set; }

        public virtual ICollection<DisciplineSemesterPlan> DisciplineSemesterPlans { get; set; } 
    }
}
