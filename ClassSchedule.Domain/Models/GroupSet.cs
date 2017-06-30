using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClassSchedule.Domain.Models
{
    /// <summary>
    /// Совокупность групп, выбранных пользователем для редактирования расписания
    /// Можно сказать, это виртуальные курсы/потоки
    /// </summary>
    [Table("GroupSet", Schema = "dbo")]
    public class GroupSet
    {
        public GroupSet()
        {
            CreatedAt = DateTime.Now;
        }

        public int GroupSetId { get; set; }
        public string GroupSetName { get; set; }

        /// <summary>
        /// Пользователь
        /// </summary>
        public string ApplicationUserId { get; set; }
        [ForeignKey("ApplicationUserId")]
        public ApplicationUser ApplicationUser { get; set; }

        public bool IsSelected { get; set; }

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

        public virtual List<GroupSetGroup> GroupSetGroups { get; set; } 
    }
}
