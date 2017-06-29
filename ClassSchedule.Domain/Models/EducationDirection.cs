using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClassSchedule.Domain.Models
{
    /// <summary>
    /// Направления (специальности) подготовки
    /// </summary>
    [Table("EducationDirection", Schema = "dict")]
    public partial class EducationDirection
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        public int EducationDirectionId { get; set; }

        /// <summary>
        /// Идентификатор для обмена
        /// </summary>
        public Guid? EducationDirectionGuid { get; set; }

        /// <summary>
        /// Код направления/специальности из 1С
        /// </summary>
        [Required]
        [StringLength(20)]
        public string EducationDirectionCode { get; set; }

        /// <summary>
        /// Наименование направления/специальности
        /// </summary>
        [Required]
        [StringLength(200)]
        public string EducationDirectionName { get; set; }

        /// <summary>
        /// Квалификация - изначально отсутствовала
        /// </summary>
        [ForeignKey("Qualification")]
        public int QualificationId { get; set; }
        public Qualification Qualification { get; set; }

        /// <summary>
        /// Новый код направления по ФГОС ВО
        /// </summary>
        public string EducationDirectionFGOSVO { get; set; }


        /// <summary>
        /// Дата последнего обновления записи
        /// </summary>
        public DateTime? UpdatedAt { get; set; }

        /// <summary>
        /// Дата удаления записи
        /// </summary>
        public DateTime? DeletedAt { get; set; }

        /// <summary>
        /// Профили
        /// </summary>
        public List<EducationProfile> EducationProfiles { get; set; }

        /// <summary>
        /// У профиля может быть много ООП
        /// </summary>
        public List<BaseProgramOfEducation> BaseProgramOfEducations { get; set; }
    }
}
