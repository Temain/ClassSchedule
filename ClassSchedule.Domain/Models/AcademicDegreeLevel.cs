using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClassSchedule.Domain.Models
{
    /// <summary>
    /// Уровень научного звания: кандидат, доктор и т.п
    /// </summary>
    [Table("AcademicDegreeLevel", Schema = "dict")]
    public class AcademicDegreeLevel
    {
        /// <summary>
        /// Идентификатор
        /// </summary>      
        public int AcademicDegreeLevelId { get; set; }

        /// <summary>
        /// Идентификатор для обмена
        /// </summary>
        public Guid? AcademicDegreeLevelGuid { get; set; }

        /// <summary>
        /// Название уровня научного звания - кандидат, доктор
        /// </summary>
        [Required]
        [StringLength(40)]
        public string AcademicDegreeLevelName { get; set; }

        /// <summary>
        /// Дата последнего обновления записи
        /// </summary>
        public DateTime? UpdatedAt { get; set; }

        /// <summary>
        /// Дата удаления записи
        /// </summary>
        public DateTime? DeletedAt { get; set; }


        /// <summary>
        /// Уровни научного звания - у одного уровня м.б. несколько научных званий
        /// </summary>
        public List<AcademicDegree> AcademicDegrees { get; set; }
    }
}
