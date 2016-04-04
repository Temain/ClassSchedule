using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassSchedule.Domain.Models
{
    /// <summary>
    /// Тип занятие (лекция, семинар и т.д.)
    /// </summary>
    [Table("LessonType", Schema = "dict")]
    public class LessonType
    {
        public int LessonTypeId { get; set; }
        public Guid LessonTypeGuid { get; set; }
        public string LessonTypeName { get; set; }
        public int Order { get; set; }

        /// <summary>
        /// Занятия этого типа
        /// </summary>
        public virtual ICollection<Lesson> Lessons { get; set; } 
    }
}
