using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassSchedule.Domain.Models
{
    /// <summary>
    /// Группа в наборе групп
    /// </summary>
    [Table("GroupSetGroup", Schema = "dbo")]
    public class GroupSetGroup
    {
        [Key, Column(Order = 0)]
        public int GroupSetId { get; set; }
        public virtual GroupSet GroupSet { get; set; }

        [Key, Column(Order = 1)]
        public int GroupId { get; set; }
        public virtual Group Group { get; set; }

        public int Order { get; set; }
    }
}
