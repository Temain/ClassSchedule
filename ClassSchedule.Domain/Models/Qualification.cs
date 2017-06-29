using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClassSchedule.Domain.Models
{
    /// <summary>
    /// Квалификация
    /// </summary>
    [Table("Qualification", Schema = "dict")]
    public class Qualification
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        public int QualificationId { get; set; }

        /// <summary>
        /// Идентификатор для обмена
        /// </summary>
        public Guid? QualificationGuid { get; set; }

        /// <summary>
        /// Код квалификации
        /// </summary>
        [Required]
        [StringLength(3)]
        public string QualificationCode { get; set; }

        /// <summary>
        /// Наименование квалификации
        /// </summary>
        [Required]
        [StringLength(100)]
        public string QualificationName { get; set; }

        /// <summary>
        /// Дата последнего обновления записи
        /// </summary>
        public DateTime? UpdatedAt { get; set; }

        /// <summary>
        /// Дата удаления записи
        /// </summary>
        public DateTime? DeletedAt { get; set; }



        /// <summary>
        /// У одной квалификации может быть много направлений обучения
        /// </summary>
        public List<EducationDirection> EducationDirections { get; set; }

        /// <summary>
        /// У одной квалификации может быть много основных образовательных программ
        /// </summary>
        public List<BaseProgramOfEducation> BaseProgramOfEducations { get; set; }
    }

}
