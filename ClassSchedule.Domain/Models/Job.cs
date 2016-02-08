using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClassSchedule.Domain.Models
{
    /// <summary>
    /// Работа
    /// </summary>
    [Table("Job", Schema = "dbo")]
    public partial class Job
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        public int JobId { get; set; }

        /// <summary>
        /// Идентификатор для обмена
        /// </summary>
        public Guid? JobGuid { get; set; }

        /// <summary>
        /// Подразделение организации - в нашем случае кафедра
        /// </summary>
        [ForeignKey("Chair")]
        public int ChairId { get; set; }
        public virtual Chair Chair { get; set; }

        /// <summary>
        /// Должность
        /// </summary>
        [ForeignKey("Position")]
        public int PositionId { get; set; }
        public virtual Position Position { get; set; }

        /// <summary>
        /// Сотрудник
        /// </summary>
        [ForeignKey("Employee")]
        public int EmployeeId { get; set; }
        public virtual Employee Employee { get; set; }

        /// <summary>
        /// Дата начала работы
        /// </summary>
        public DateTime JobDateStart { get; set; }

        /// <summary>
        /// Дата окончания работы
        /// </summary>
        public DateTime? JobDateEnd { get; set; }

        /// <summary>
        /// Ставка
        /// </summary>
        public float PositionQuantity { get; set; }

        /// <summary>
        /// Условия работы
        /// </summary>
        [ForeignKey("EmploymentType")]
        public int EmploymentTypeId { get; set; }
        public virtual EmploymentType EmploymentType { get; set; }

        /// <summary>
        /// Признак того, что сотрудник преподаватель (из БД Sync.Teachers)
        /// </summary>
        public bool? IsTeacher { get; set; }


        /// <summary>
        /// Признак того, что сотрудник декан факультета
        /// </summary>
        public bool? IsDean { get; set; }

        /// <summary>
        /// Признак того, что сотрудник зав. кафедры
        /// </summary>
        public bool? IsHeadOfChair { get; set; }


        /// <summary>
        /// Отметка об удалении записи
        /// </summary>
        public bool? IsDeleted { get; set; }
        
        /// <summary>
        /// Дата последнего обновления записи
        /// </summary>
        public DateTime? UpdatedAt { get; set; }



        /// <summary>
        /// Отпуск - обычный, по уходу за ребенком. Может быть несколько отпусков. 
        /// </summary>
        public virtual ICollection<PersonVacation> PersonVacations { get; set; }

        /// <summary>
        /// Занятия
        /// </summary>
        public virtual ICollection<Lesson> Lessons { get; set; }

        /// <summary>
        /// Семестровые планы по дисциплинам
        /// </summary>
        public virtual ICollection<DisciplineSemesterPlan> DisciplineSemesterPlans { get; set; } 
    }

}
