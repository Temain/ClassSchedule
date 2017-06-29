using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClassSchedule.Domain.Models
{
    /// <summary>
    /// Должность выверенная УМУ
    /// </summary>
    [Table("PositionReal", Schema = "dict")]
    public class PositionReal
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        public int PositionRealId { get; set; }

        /// <summary>
        /// Идентификатор для систем обмена
        /// </summary>
        public Guid? PositionRealGuid { get; set; }

        /// <summary>
        /// Наименование должности
        /// </summary>
        [Required]
        [StringLength(100)]
        public string PositionName { get; set; }

        /// <summary>
        /// Дата последнего обновления записи
        /// </summary>
        public DateTime? UpdatedAt { get; set; }

        /// <summary>
        /// Дата удаления записи
        /// </summary>
        public DateTime? DeletedAt { get; set; }


        public List<Position> Positions { get; set; }

        /// <summary>
        /// Вакансии кафедр
        /// </summary>
        public List<PlannedChairJob> PlannedChairJobs { get; set; }
    }
}
