using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClassSchedule.Domain.Models
{
    /// <summary>
    /// Семестр учебного года
    /// </summary>
    [Table("EducationSemester", Schema = "dict")]
    public class EducationSemester
    {
        /// <summary>
        /// Идентификатор 
        /// </summary>
        public int EducationSemesterId { get; set; }

        /// <summary>
        /// Идентификатор для обмена
        /// </summary>
        public Guid? EducationSemesterGuid { get; set; }

        /// <summary>
        /// Название семестра
        /// </summary>
        [StringLength(256)]
        public string EducationSemesterName  { get; set;}

        /// <summary>
        /// Номер семестра
        /// </summary>
        public int EducationSemesterNumber { get; set; }
        
        [ForeignKey("EducationYear")]
        public int EducationYearId { get; set; }
        public EducationYear EducationYear { get; set; }

        /// <summary>
        /// Дата начала семестра - на разных факультетах может отличатся, но обычно 1 семестр: 1 сентября по 31 января, 2 семестр: 1 февраля по 31 августа
        /// </summary>
        public DateTime EducationSemesterStart { get; set; }

        /// <summary>
        /// Дата окончания семестра - на разных факультетах может отличатся, но обычно 1 семестр: 1 сентября по 31 января, 2 семестр: 1 февраля по 31 августа
        /// </summary>
        public DateTime EducationSemesterEnd { get; set; }


        /// <summary>
        /// Дата последнего обновления записи
        /// </summary>
        public DateTime? UpdatedAt { get; set; }

        /// <summary>
        /// Дата удаления записи
        /// </summary>
        public DateTime? DeletedAt { get; set; }

        /// <summary>
        /// Дисциплины, которые читаются в конкретном семестре
        /// </summary>
        public List<Discipline> Disciplines { get; set; }
    }
}
