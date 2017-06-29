using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClassSchedule.Domain.Models
{
    /// <summary>
    /// Версии федеральных государственных стандартов ФГОС ВПО3, ФГОС ВПО3+ и т.д.
    /// </summary>
    [Table("FederalEducationalStandard", Schema = "dict")]
    public class FederalEducationalStandard
    {
        /// <summary>
        /// Идентификатор 
        /// </summary>
        public int FederalEducationalStandardId { get; set; }

        /// <summary>
        /// Идентификатор для обмена
        /// </summary>
        public Guid? FederalEducationalStandardGuid { get; set; }

        /// <summary>
        /// Название версии образовательного стандарта
        /// </summary>
        [StringLength(512)]
        public string Name { get; set; }

        /// <summary>
        /// Версия стандарта (3 или 3.5)
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// Дата последнего обновления записи
        /// </summary>
        public DateTime? UpdatedAt { get; set; }

        /// <summary>
        /// Дата удаления записи
        /// </summary>
        public DateTime? DeletedAt { get; set; }


        /// <summary>
        /// Основные образовательные программы
        /// </summary>
        public virtual ICollection<BaseProgramOfEducation> BaseProgramOfEducations { get; set; }
    }
}
