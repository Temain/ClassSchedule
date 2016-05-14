using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassSchedule.Domain.Models
{
    /// <summary>
    /// Совокупность групп, выбранных пользователем для редактирования расписания
    /// Можно сказать, это виртуальные курсы/потоки
    /// </summary>
    [Table("GroupSet", Schema = "dbo")]
    public class GroupSet
    {
        public int GroupSetId { get; set; }
        public string GroupSetName { get; set; }

        /// <summary>
        /// Пользователь
        /// </summary>
        public string ApplicationUserId { get; set; }

        [ForeignKey("ApplicationUserId")]
        public virtual ApplicationUser ApplicationUser { get; set; }

        public bool IsSelected { get; set; }

        public virtual ICollection<GroupSetGroup> GroupSetGroups { get; set; } 
    }
}
