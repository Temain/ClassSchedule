using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClassSchedule.Domain.Models
{
    /// <summary>
    /// Справочник названий дисциплин
    /// </summary>
    [Table("DisciplineName", Schema = "dict")]
    public class DisciplineName
    {
        /// <summary>
        /// Идентификатор 
        /// </summary>
        public int DisciplineNameId { get; set; }

        /// <summary>
        /// Идентификатор для обмена
        /// </summary>
        public Guid? DisciplineNameGuid { get; set; }

        /// <summary>
        /// Название дисциплины
        /// </summary>
        [Required]
        [Display(Name = "Название дисциплины")]
        [StringLength(1024)]
        [Index(IsUnique = true)]
        public string Name { get; set; }


        /// <summary>
        /// Дата последнего обновления записи
        /// </summary>
        public DateTime? UpdatedAt { get; set; }

        /// <summary>
        /// Дата удаления записи
        /// </summary>
        public DateTime? DeletedAt { get; set; }

        /// <summary>
        /// Дисциплины, которые имеют одинаковое название, но читаются по разному
        /// </summary>
        public List<Discipline> Disciplines { get; set; }
    }
}
