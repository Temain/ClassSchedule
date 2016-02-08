using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClassSchedule.Domain.Models
{
    /// <summary>
    /// Дисциплина (Предмет)
    /// </summary>
    [Table("Discipline", Schema = "dict")]
    public partial class Discipline
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        public int DisciplineId { get; set; }
        
        /// <summary>
        /// Идентификатор для обмена
        /// </summary>
        public Guid? DisciplineGuid { get; set; }
        
        /// <summary>
        /// Наименование дисциплины (предмета)
        /// </summary>
        [Required]
        [StringLength(200)]
        public string DisciplineName { get; set; }

        /// <summary>
        /// Отметка об удалении записи
        /// </summary>
        public bool? IsDeleted { get; set; }

        /// <summary>
        /// Дата последнего обновления записи
        /// </summary>
        public DateTime? UpdatedAt { get; set; }

        /// <summary>
        /// Занятия
        /// </summary>
        public virtual ICollection<Lesson> Lessons { get; set; } 
    }
}
