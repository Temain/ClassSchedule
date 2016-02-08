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
        /// Конструктор класса - форма обучения
        /// </summary>
        public EducationForm()
        {
            EducationFormGuid = Guid.NewGuid();
        }

        /// <summary>
        /// Идентификатор
        /// </summary>
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
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
        /// Отметка об удалении записи
        /// </summary>
        public bool? IsDeleted { get; set; }

        /// <summary>
        /// Дата последнего обновления записи
        /// </summary>
        public DateTime? UpdatedAt { get; set; }

        public virtual ICollection<ProgramOfEducation> ProgramsOfEducation { get; set; }
    }
}
