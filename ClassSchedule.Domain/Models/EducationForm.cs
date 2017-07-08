using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClassSchedule.Domain.Models
{
    /// <summary>
    /// Форма обучения
    /// </summary>
    [Table("EducationForm", Schema = "dict")]
    public partial class EducationForm
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        public int EducationFormId { get; set; }

        /// <summary>
        /// Идентификатор для обмена
        /// </summary>
        public Guid? EducationFormGuid { get; set; }

        /// <summary>
        /// Наименование формы обучения
        /// </summary>
        [Required]
        [StringLength(50)]
        public string EducationFormName { get; set; }

        /// <summary>
        /// Дата последнего обновления записи
        /// </summary>
        public DateTime? UpdatedAt { get; set; }

        /// <summary>
        /// Дата удаления записи
        /// </summary>
        public DateTime? DeletedAt { get; set; }

        /// <summary>
        /// Не используется
        /// </summary>
        public bool IsNotUsed { get; set; }

        /// <summary>
        /// У формы обучения может быть много ООП
        /// </summary>
        public List<BaseProgramOfEducation> BaseProgramOfEducations { get; set; }
    }
}
