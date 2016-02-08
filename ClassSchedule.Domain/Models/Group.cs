using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClassSchedule.Domain.Models
{
    /// <summary>
    /// Группа. Часть данных из представления mdGroup где guid орг.структуры равен 82439137-02B5-4E30-AF30-B246B053EFAA
    /// </summary>
    [Table("Group", Schema = "dict")]
    public partial class Group
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        public int GroupId { get; set; }

        /// <summary>
        /// Идентификатор для обмена
        /// </summary>
        public Guid? GroupGuid { get; set; }

        /// <summary>
        /// Курс, на котором обучается группа
        /// </summary>
        public int CourseId { get; set; }
        public virtual Course Course { get; set; }

        /// <summary>
        /// Код подразделения из 1С
        /// </summary>
        [StringLength(20)]
        public string DivisionCode { get; set; }

        /// <summary>
        /// Наименование подразделения
        /// </summary>
        [Required]
        [StringLength(200)]
        public string DivisionName { get; set; }

	    /// <summary>
        /// Вышестоящее подразделение. Используется и оставлена для совместимости в схеме обмена. 
        /// </summary>
        public Guid? ParentGuid { get; set; }

        /// <summary>
        /// Отметка об удалении записи
        /// </summary>
        public bool? IsDeleted { get; set; }

        /// <summary>
        /// Дата последнего обновления записи
        /// </summary>
        public DateTime? UpdatedAt { get; set; }


        /// <summary>
        /// Программа обучения
        /// </summary>
        public int ProgramOfEducationId { get; set; }
        public virtual ProgramOfEducation ProgramOfEducation { get; set; }

        /// <summary>
        /// Занятия
        /// </summary>
        public virtual ICollection<Lesson> Lessons { get; set; } 
    }
}
