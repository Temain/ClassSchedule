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
        public Chair Chair { get; set; }

        /// <summary>
        /// Должность
        /// </summary>
        [ForeignKey("Position")]
        public int PositionId { get; set; }
        public Position Position { get; set; }

        /// <summary>
        /// Сотрудник
        /// </summary>
        [ForeignKey("Employee")]
        public int EmployeeId { get; set; }
        public Employee Employee { get; set; }

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
        public EmploymentType EmploymentType { get; set; }

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
        /// Дата последнего обновления записи
        /// </summary>
        public DateTime? UpdatedAt { get; set; }

        /// <summary>
        /// Дата удаления записи
        /// </summary>
        public DateTime? DeletedAt { get; set; }


        /// <summary>
        /// Вакансии кафедр
        /// </summary>
        public List<PlannedChairJob> PlannedChairJobs { get; set; }


        /// <summary>
        /// Возвращает ФИО, должность, условия работы, дата начала работы
        /// </summary>
        /// <returns></returns>
        public string GetCurrentActiveTeacherFullNameWithPositionAndDates()
        {
            var fullName = Employee.Person.FullName;
            var position = Position.PositionName;

            return fullName + " - " + position + ", " + EmploymentType.EmploymentTypeName + " [ Работает с " + JobDateStart.ToShortDateString() + " ]";
        }
    }
}
