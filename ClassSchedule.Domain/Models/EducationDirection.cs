using System;
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
        /// Новый код направления по ФГОС ВО
        /// </summary>
        public string EducationDirectionFGOSVO { get; set; }


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
