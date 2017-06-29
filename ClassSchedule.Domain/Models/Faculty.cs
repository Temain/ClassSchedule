using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClassSchedule.Domain.Models
{ 
    /// <summary>
    /// Факультет
    /// </summary>
    [Table("Faculty", Schema = "dbo")]
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
        /// Дата последнего обновления записи
        /// </summary>
        public DateTime? UpdatedAt { get; set; }

        /// <summary>
        /// Дата удаления записи
        /// </summary>
        public DateTime? DeletedAt { get; set; }

        /// <summary>
        /// Кафедры факультета
        /// </summary>
        public List<Chair> Chairs { get; set; }

        /// <summary>
        /// Курсы на факультете
        /// </summary>
        public List<Course> Courses { get; set; }

        /// <summary>
        /// Пользователи, ответственные за расписание данного факультета
        /// </summary>
        public List<ApplicationUser> ApplicationUsers { get; set; } 
    }
}
