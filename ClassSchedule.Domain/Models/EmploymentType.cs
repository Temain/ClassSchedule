using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClassSchedule.Domain.Models
{
    /// <summary>
    /// Условия работы
    /// </summary>
    [Table("EmploymentType", Schema = "dict")]
    public partial class EmploymentType
    {

        /// <summary>
        /// Идентификатор
        /// </summary>
        public int EmploymentTypeId { get; set; }

        /// <summary>
        /// Идентификатор для систем обмена
        /// </summary>
        public Guid? EmploymentTypeGuid { get; set; }

        /// <summary>
        /// Наименование условия работы
        /// </summary>
        [Required]
        [StringLength(50)]
        public string EmploymentTypeName { get; set; }

        /// <summary>
        /// Отметка об удалении записи
        /// </summary>
        public bool? IsDeleted { get; set; }

        /// <summary>
        /// Дата последнего обновления записи
        /// </summary>
        public DateTime? UpdatedAt { get; set; }

        /// <summary>
        /// Связь для отчета 1.1 Rep. 1.1
        /// </summary>
        public virtual ICollection<Job> Jobs { get; set; }

    }
}
