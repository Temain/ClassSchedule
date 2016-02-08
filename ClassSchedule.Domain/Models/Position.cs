using System;
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
        /// Отметка об удалении записи
        /// </summary>
        public bool? IsDeleted { get; set; }

        /// <summary>
        /// Дата последнего обновления записи
        /// </summary>
        public DateTime? UpdatedAt { get; set; }

    }
}
