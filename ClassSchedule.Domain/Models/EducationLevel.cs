using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClassSchedule.Domain.Models
{
    /// <summary>
    /// Уровень образования
    /// </summary>
    [Table("EducationLevel", Schema = "dict")]
    public partial class EducationLevel
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        public int EducationLevelId { get; set; }

        /// <summary>
        /// Идентификатор для обмена
        /// </summary>
        public Guid? EducationLevelGuid { get; set; }

        /// <summary>
        /// Наименование уровня образования
        /// </summary>
        [Required]
        [StringLength(50)]
        public string EducationLevelName { get; set; }

        /// <summary>
        /// Отметка об удалении записи
        /// </summary>
        public bool? IsDeleted { get; set; }

        /// <summary>
        /// Дата последнего обновления записи
        /// </summary>
        public DateTime? UpdatedAt { get; set; }

        public virtual ICollection<ProgramOfEducation> ProgramsOfEducation { get; set; }
    }
}
