using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClassSchedule.Domain.Models
{
    /// <summary>
    /// Сотрудник
    /// </summary>
    [Table("Employee", Schema = "dbo")]
    public partial class Employee
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        public int EmployeeId { get; set; }

        /// <summary>
        /// Идентификатор для обмена
        /// </summary>
        public Guid? EmployeeGuid { get; set; }

        /// <summary>
        /// Физическое лицо
        /// </summary>
        [ForeignKey("Person")]
        public int PersonId { get; set; }
        public virtual Person Person { get; set; }

        /// <summary>
        /// Табельный номер
        /// </summary>
        [Required]
        [StringLength(20)]
        public string EmployeeCode { get; set; }

        /// <summary>
        /// Дата приема на работу
        /// </summary>
        public DateTime EmployeeDateStart { get; set; }

        /// <summary>
        /// Дата увольнения
        /// </summary>
        public DateTime? EmployeeDateEnd { get; set; }

        /// <summary>
        /// Отметка об удалении записи
        /// </summary>
        public bool? IsDeleted { get; set; }

        /// <summary>
        /// Дата последнего обновления записи
        /// </summary>
        public DateTime? UpdatedAt { get; set; }
        
        /// <summary>
        /// Должности сотрудника (выполняемая работа) М.б. несколько должностей у одного сотрудника
        /// </summary>
        public virtual ICollection<Job> Jobs { get; set; }

    }

}
