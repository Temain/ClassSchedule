using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClassSchedule.Domain.Models
{
    /// <summary>
    /// Курс
    /// </summary>
    [Table("Course", Schema = "dbo")]
    public partial class Course
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        public int CourseId { get; set; }

        /// <summary>
        /// Идентификатор для обмена
        /// </summary>
        public Guid? CourseGuid { get; set; }
        
        /// <summary>
        /// Подразделение - факультет
        /// </summary>
        [ForeignKey("Faculty")]
        public int FacultyId { get; set; }
        public virtual Faculty Faculty { get; set; }
             
        /// <summary>
        /// Наименование курса
        /// </summary>
        [Required]
        [StringLength(50)]
        public string CourseName { get; set; }

        /// <summary>
        /// Порядковый номер курса
        /// </summary>
        public int CourseNumber { get; set; }

        /// <summary>
        /// Год начала обучения
        /// </summary>
        public int? YearStart { get; set; }

        /// <summary>
        /// Отметка об удалении записи
        /// </summary>
        public bool? IsDeleted { get; set; }

        /// <summary>
        /// Дата последнего обновления записи
        /// </summary>
        public DateTime? UpdatedAt { get; set; }




        /// <summary>
        /// Привязка групп к курсу
        /// </summary>
        public virtual ICollection<Group> Groups { get; set; }

        /// <summary>
        /// Пользователи, которые редактируют расписание для этого курса
        /// </summary>
        public virtual ICollection<ApplicationUser> ApplicationUsers { get; set; }
    }
}
