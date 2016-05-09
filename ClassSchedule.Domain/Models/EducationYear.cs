using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClassSchedule.Domain.Models
{
    /// <summary>
    /// Учебный год
    /// </summary>
    [Table("EducationYear", Schema = "dict")]
    public class EducationYear
    {
        public int EducationYearId { get; set; }

        /// <summary>
        /// Идентификатор для обмена
        /// </summary>
        public Guid? EducationYearGuid { get; set; }

        /// <summary>
        /// Наименование учебного года
        /// </summary>
        [Required]
        [StringLength(20)]
        public string EducationYearName { get; set; }

        /// <summary>
        /// В каком году начался
        /// </summary>
        [Required]
        public int YearStart { get; set; }

        /// <summary>
        /// В каком году закончился
        /// </summary>
        [Required]
        public int YearEnd { get; set; }

        /// <summary>
        /// Дата начала
        /// </summary>
        public DateTime DateStart { get; set; }

        /// <summary>
        /// Дата окончания
        /// </summary>
        public DateTime DateEnd { get; set; }
        
        /// <summary>
        /// Отметка об удалении записи
        /// </summary>
        public bool? IsDeleted { get; set; }

        /// <summary>
        /// Дата последнего обновления записи
        /// </summary>
        public DateTime? UpdatedAt { get; set; }

        public virtual ICollection<Lesson> Lessons { get; set; } 
        public virtual ICollection<ApplicationUser> ApplicationUsers { get; set; } 
    }
}
