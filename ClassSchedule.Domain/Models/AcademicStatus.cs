using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClassSchedule.Domain.Models
{
    /// <summary>
    /// Ученое звание
    /// </summary>
    [Table("AcademicStatus", Schema = "dict")]
    public class AcademicStatus
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        public int AcademicStatusId { get; set; }

        /// <summary>
        /// Идентификатор
        /// </summary>
        public Guid AcademicStatusGuid { get; set; }

        /// <summary>
        /// Наименование ученого звания
        /// </summary>
        [Required]
        [StringLength(50)]
        public string AcademicStatusName { get; set; }

        /// <summary>
        /// Краткое наименование ученого звания
        /// </summary>
        [StringLength(50)]
        public string AcademicStatusShortName { get; set; }

        /// <summary>
        /// Дата последнего обновления записи
        /// </summary>
        public DateTime? UpdatedAt { get; set; }

        /// <summary>
        /// Дата удаления записи
        /// </summary>
        public DateTime? DeletedAt { get; set; }

        /// <summary>
        /// Физ. лица
        /// </summary>
        public List<Person> Persons { get; set; } 
    }
}
