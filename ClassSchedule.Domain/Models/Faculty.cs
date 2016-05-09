using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClassSchedule.Domain.Models
{ 
    /// <summary>
    /// Факультет - из представления mdFaculty где оргструктура равна 82439137-02B5-4E30-AF30-B246B053EFAA
    /// </summary>
    [Table("Faculty", Schema = "dict")]
    public partial class Faculty
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        public int FacultyId { get; set; }

        /// <summary>
        /// Идентификатор для обмена
        /// </summary>
        public Guid? FacultyGuid { get; set; }

        /// <summary>
        /// Код подразделения из 1С
        /// </summary>
        [StringLength(20)]
        public string DivisionCode { get; set; }

        /// <summary>
        /// Наименование подразделения
        /// </summary>
        [StringLength(200)]
        public string DivisionName { get; set; }

        /// <summary>
        /// Вышестоящее подразделение
        /// </summary>
        public Guid? ParentId { get; set; }

        /// <summary>
        /// Отметка об удалении записи
        /// </summary>
        public bool? IsDeleted { get; set; }

        /// <summary>
        /// Дата последнего обновления записи
        /// </summary>
        public DateTime? UpdatedAt { get; set; }

        /// <summary>
        /// Курсы на факультете
        /// </summary>
        public virtual ICollection<Course> Courses { get; set; }

        /// <summary>
        /// Пользователи, ответственные за расписание данного факультета
        /// </summary>
        public virtual ICollection<ApplicationUser> ApplicationUsers { get; set; } 
    }
}
