using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClassSchedule.Domain.Models
{
    /// <summary>
    /// Привязка группы к потоку
    /// </summary>
    [Table("GroupFlowBinding", Schema = "dbo")]
    public class GroupFlowBinding
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        public int GroupFlowBindingId { get; set; }

        /// <summary>
        /// Идентификатор для обмена
        /// </summary>
        public Guid GroupFlowBindingGuid { get; set; }

        /// <summary>
        /// Дисциплина
        /// </summary>
        public int DisciplineId { get; set; }
        public Discipline Discilpine { get; set; }

        /// <summary>
        /// Группа
        /// </summary>
        public int GroupId { get; set; }
        public Group Group { get; set; }

        /// <summary>
        /// Поток
        /// </summary>
        public int FlowNumber { get; set; }

        /// <summary>
        /// Дата последнего обновления записи
        /// </summary>
        public DateTime? UpdatedAt { get; set; }

        /// <summary>
        /// Дата удаления записи
        /// </summary>
        public DateTime? DeletedAt { get; set; }
    }
}
