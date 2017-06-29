using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClassSchedule.Domain.Models
{
    /// <summary>
    /// Ускоренное обучение на базе СПО или ВПО
    /// </summary>
    [Table("BaseOfAcceleration", Schema = "dict")]
    public class BaseOfAcceleration
    {
        /// <summary>
        /// Идентификатор 
        /// </summary>
        public int BaseOfAccelerationId { get; set; }

        /// <summary>
        /// Идентификатор для обмена
        /// </summary>
        public Guid? BaseOfAccelerationGuid { get; set; }

        /// <summary>
        /// Наименование основания
        /// </summary>
        public string BaseOfAccelerationName { get; set; }

        /// <summary>
        /// Дата последнего обновления записи
        /// </summary>
        public DateTime? UpdatedAt { get; set; }

        /// <summary>
        /// Дата удаления записи
        /// </summary>
        public DateTime? DeletedAt { get; set; }

        /// <summary>
        /// ООП с ускоренным обучением на основании СПО или ВПО
        /// </summary>
        public List<BaseProgramOfEducation> BaseProgramsOfEducation { get; set; }
    }
}
