using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClassSchedule.Domain.Models
{
    /// <summary>
    /// Ученая степень
    /// </summary>
    [Table("AcademicDegree", Schema = "dict")]
    public partial class AcademicDegree
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        public int AcademicDegreeId { get; set; }
        
        /// <summary>
        /// Идентификатор для обмена
        /// </summary>
        public Guid AcademicDegreeGuid { get; set; }

        /// <summary>
        /// Наименование ученой степени
        /// </summary>
        [Required]
        [StringLength(50)]
        public string AcademicDegreeName { get; set; }
        
        /// <summary>
        /// Краткое наименование ученой степени
        /// </summary>
        [StringLength(20)]
        public string AcademicDegreeShortName { get; set; }

        /// <summary>
        /// Уровень научного звания - разные доктора наук, кандидаты (по науч.специальностям)
        /// </summary>
        [ForeignKey("AcademicDegreeLevel")]
        public int AcademicDegreeLevelId { get; set; }
        public AcademicDegreeLevel AcademicDegreeLevel { get; set; }

        /// <summary>
        /// Дата последнего обновления записи
        /// </summary>
        public DateTime? UpdatedAt { get; set; }

        /// <summary>
        /// Дата удаления записи
        /// </summary>
        public DateTime? DeletedAt { get; set; }

        /// <summary>
        /// Физ. лица
        /// </summary>
        public List<Person> Persons { get; set; }

        /// <summary>
        /// Вакансии кафедр
        /// </summary>
        public List<PlannedChairJob> PlannedChairJobs { get; set; }
    }
}
