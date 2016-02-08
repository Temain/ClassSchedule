using System;
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
        public virtual EducationDirection EducationDirection { get; set; }

        /// <summary>
        /// Наименование профиля подготовки
        /// </summary>
        [Required]
        [StringLength(200)]
        public string EducationProfileName { get; set; }


        /// <summary>
        /// Отметка об удалении записи
        /// </summary>
        public bool? IsDeleted { get; set; }

        /// <summary>
        /// Дата последнего обновления записи
        /// </summary>
        public DateTime? UpdatedAt { get; set; }
    }
}
