using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClassSchedule.Domain.Models
{
    /// <summary>
    /// Группа. Часть данных из представления mdGroup где guid орг.структуры равен 82439137-02B5-4E30-AF30-B246B053EFAA
    /// </summary>
    [Table("Group", Schema = "dbo")]
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
        [ForeignKey("Course")]
        public int CourseId { get; set; }
        public virtual Course Course { get; set; }

        /// <summary>
        /// Код подразделения из 1С
        /// </summary>
        [StringLength(20)]
        public string GroupCode { get; set; }

        /// <summary>
        /// Наименование подразделения
        /// </summary>
        [Required]
        [StringLength(200)]
        public string GroupName { get; set; }

	    /// <summary>
        /// Вышестоящее подразделение. Используется и оставлена для совместимости в схеме обмена. 
        /// </summary>
        public Guid? ParentGuid { get; set; }

        /// <summary>
        /// Количество студентов в группе
        /// </summary>
        public int? NumberOfStudents { get; set; }

        /// <summary>
        /// Дата последнего обновления записи
        /// </summary>
        public DateTime? UpdatedAt { get; set; }

        /// <summary>
        /// Дата удаления записи
        /// </summary>
        public DateTime? DeletedAt { get; set; }

        /// <summary>
        /// Основная программа обучения по которой обучается группа студентов
        /// </summary>
        [ForeignKey("BaseProgramOfEducation")]
        public int? BaseProgramOfEducationId { get; set; }
        public BaseProgramOfEducation BaseProgramOfEducation { get; set; }

        /// <summary>
        /// Позиция в расписании
        /// </summary>
        public List<Schedule> Schedule { get; set; }

        /// <summary>
        /// Группа может быть добавлена пользователем в объединение групп для редактирования расписания
        /// </summary>
        public List<GroupSetGroup> GroupSetGroups { get; set; }

        /// <summary>
        /// Распределение групп по потокам для дисциплин
        /// </summary>
        public List<GroupFlowBinding> GroupFlowBindings { get; set; }

        /// <summary>
        /// Разделение групп на подгруппы по определенным дисциплинам
        /// </summary>
        public List<GroupSubgroups> GroupSubgroups { get; set; }
    }
}
