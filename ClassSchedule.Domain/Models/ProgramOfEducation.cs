using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClassSchedule.Domain.Models
{
    /// <summary>
    /// Программа обучения 
    /// </summary>
    [Table("ProgramOfEducation", Schema = "dbo")]
    public partial class ProgramOfEducation
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        public int ProgramOfEducationId { get; set; }

        /// <summary>
        /// Идентификатор для обмена
        /// </summary>
        public Guid? ProgramOfEducationGuid { get; set; }

        /// <summary>
        /// Профиль обучения
        /// </summary>
        [ForeignKey("EducationProfile")]
        public int EducationProfileId { get; set; }
        public virtual EducationProfile EducationProfile { get; set; }

        /// <summary>
        /// Форма обучения
        /// </summary>
        [ForeignKey("EducationForm")]
        public int EducationFormId { get; set; }
        public virtual EducationForm EducationForm { get; set; }

        /// <summary>
        /// Уровень образования
        /// </summary>
        [ForeignKey("EducationLevel")]
        public int EducationLevelId { get; set; }
        public virtual EducationLevel EducationLevel { get; set; }
        
        /// <summary>
        /// Дата начала действия программы обучения
        /// </summary>
        public short YearStart { get; set; }

        /// <summary>
        /// Дата прекращения действия программы обучения
        /// </summary>
        public short? YearEnd { get; set; }

        /// <summary>
        /// Код программы обучения 
        /// </summary>
        //[Required]
        [StringLength(5)]
        public string ProgramOfEducationCode { get; set; }

        /// <summary>
        /// Наименование программы обучения
        /// </summary>
        //[Required]
        [StringLength(200)]
        public string ProgramOfEducationName { get; set; }

        /// <summary>
        /// Отметка об удалении записи
        /// </summary>
        public bool? IsDeleted { get; set; }

        /// <summary>
        /// Дата последнего обновления записи
        /// </summary>
        public DateTime? UpdatedAt { get; set; }

        /// <summary>
        /// Группы, обучающиеся по данной программе обучения
        /// </summary>
        public virtual ICollection<Group> Groups { get; set; }

        /// <summary>
        /// Учебные планы
        /// </summary>
        public virtual ICollection<AcademicPlan> AcademicPlans { get; set; } 
    }
}
