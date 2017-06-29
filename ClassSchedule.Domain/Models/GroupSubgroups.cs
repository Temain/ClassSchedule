using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClassSchedule.Domain.Models
{
    /// <summary>
    /// Разделение групп на подгруппы по определённой дисциплине (для лабораторных работ)
    /// </summary>
    [Table("GroupSubgroups", Schema = "dbo")]
    public class GroupSubgroups
    {
        public int GroupSubgroupsId { get; set; }

        /// <summary>
        /// Группа
        /// </summary>
        public int GroupId { get; set; }
        public Group Group { get; set; }

        /// <summary>
        /// Дисциплина
        /// </summary>
        public int DisciplineId { get; set; }
        public Discipline Discipline { get; set; }

        /// <summary>
        /// Количество подгрупп
        /// </summary>
        public int NumberOfSubgroups { get; set; }

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
