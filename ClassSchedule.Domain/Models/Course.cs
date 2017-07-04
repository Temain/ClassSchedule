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
        // [Required]
        // [StringLength(50)]
        public string CourseName { get; set; }

        /// <summary>
        /// Префикс
        /// </summary>
        public string CourseNamePrefix { get; set; }

        /// <summary>
        /// Суффикс
        /// </summary>
        public string CourseNameSuffix { get; set; }

        /// <summary>
        /// Ускоренность
        /// </summary>
        public bool IsIntensive { get; set; }

        /// <summary>
        /// Порядковый номер курса
        /// </summary>
        public int? CourseNumber { get; set; }

        /// <summary>
        /// Год начала обучения
        /// </summary>
        public int? YearStart { get; set; }

        /// <summary>
        /// Дата последнего обновления записи
        /// </summary>
        public DateTime? UpdatedAt { get; set; }

        /// <summary>
        /// Дата удаления записи
        /// </summary>
        public DateTime? DeletedAt { get; set; }


        /// <summary>
        /// Привязка групп к курсу
        /// </summary>
        public List<Group> Groups { get; set; }
    }
}
