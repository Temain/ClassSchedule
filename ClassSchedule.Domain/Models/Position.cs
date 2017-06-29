using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClassSchedule.Domain.Models
{
    /// <summary>
    /// Должность
    /// </summary>
    [Table("Position", Schema = "dict")]
    public partial class Position
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        public int PositionId { get; set; }

        /// <summary>
        /// Идентификатор для систем обмена
        /// </summary>
        public Guid? PositionGuid { get; set; }

        /// <summary>
        /// Код должности
        /// </summary>
        [StringLength(20)]
        public string PositionCode { get; set; }

        /// <summary>
        /// Наименование должности
        /// </summary>
        [Required]
        [StringLength(100)]
        public string PositionName { get; set; }

        /// <summary>
        /// Должность, выверена УМУ
        /// </summary>
        [ForeignKey("PositionReal")]
        public int? PositionRealId { get; set; }
        public PositionReal PositionReal { get; set; }

        /// <summary>
        /// Дата последнего обновления записи
        /// </summary>
        public DateTime? UpdatedAt { get; set; }

        /// <summary>
        /// Дата удаления записи
        /// </summary>
        public DateTime? DeletedAt { get; set; }

        /// <summary>
        /// Ставки сотрудников
        /// </summary>
        public List<Job> Jobs { get; set; }
    }
}
