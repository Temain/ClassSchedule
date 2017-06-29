using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClassSchedule.Domain.Models
{
    /// <summary>
    /// Группа в наборе групп
    /// </summary>
    [Table("GroupSetGroup", Schema = "dbo")]
    public class GroupSetGroup
    {
        public GroupSetGroup()
        {
            CreatedAt = DateTime.Now;
        }

        [Key, Column(Order = 0)]
        public int GroupSetId { get; set; }
        public GroupSet GroupSet { get; set; }

        [Key, Column(Order = 1)]
        public int GroupId { get; set; }
        public Group Group { get; set; }

        public int Order { get; set; }

        /// <summary>
        /// Дата последнего обновления записи
        /// </summary>
        public DateTime CreatedAt { get; set; }

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
