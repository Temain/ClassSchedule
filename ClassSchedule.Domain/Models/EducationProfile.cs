using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClassSchedule.Domain.Models
{
    /// <summary>
    /// Профиль подготовки
    /// </summary>
    [Table("EducationProfile", Schema = "dict")]
    public partial class EducationProfile
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        public int EducationProfileId { get; set; }

        /// <summary>
        /// Идентификатор для обмена
        /// </summary>
        public Guid? EducationProfileGuid { get; set; }
        
        /// <summary>
        /// Направления (специальности) подготовки
        /// </summary>
        [ForeignKey("EducationDirection")]
        public int EducationDirectionId { get; set; }
        public EducationDirection EducationDirection { get; set; }

        /// <summary>
        /// Наименование профиля подготовки
        /// </summary>
        [Required]
        [StringLength(200)]
        public string EducationProfileName { get; set; }

        /// <summary>
        /// Профили отмечены галкой только те, которые есть в файле от УМУ - высылал Горовой С.А. Есть в нашем университете
        /// </summary>
        public bool? IsCorrectProfile { get; set; }

        /// <summary>
        /// Дата последнего обновления записи
        /// </summary>
        public DateTime? UpdatedAt { get; set; }

        /// <summary>
        /// Дата удаления записи
        /// </summary>
        public DateTime? DeletedAt { get; set; }

        /// <summary>
        /// У профиля может быть много ООП
        /// </summary>
        public List<BaseProgramOfEducation> BaseProgramOfEducations { get; set; }
    }
}
