using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClassSchedule.Domain.Models
{
    /// <summary>
    /// Отпуска. Требуется хранить как отпуска сотрудников, так и студентов (академ, уход за ребенком и т.п.)
    /// </summary>
    [Table("PersonVacation", Schema = "dbo")]
    public partial class PersonVacation
    {

        public PersonVacation()
        {
            this.PersonVacationGuid = Guid.NewGuid();
        }

        /// <summary>
        /// Идентификатор отпуска сотрудника
        /// </summary>
        public int PersonVacationId { get; set; }

        /// <summary>
        /// Идентификатор отпуска сотрудника - для систем обмена
        /// </summary>
        public Guid? PersonVacationGuid { get; set; }
        
        /// <summary>
        /// Идентификатор персоны
        /// </summary>
        [ForeignKey("Person")]
        public int PersonId { get; set; }
        public virtual Person Person { get; set; }

        /// <summary>
        /// Дата начала отпуска 
        /// </summary>
        public DateTime? VacationBeginningDate { get; set; }

        /// <summary>
        /// Дата окончания отпуска
        /// </summary>
        public DateTime? VacationTerminationDate { get; set; }

        /// <summary>
        /// Отметка об удалении записи
        /// </summary>
        public bool? IsDeleted { get; set; }

        /// <summary>
        /// Дата последнего обновления записи
        /// </summary>
        public DateTime? UpdatedAt { get; set; }


        /// <summary>
        /// Связь к работе
        /// </summary>
        [ForeignKey("Job")]
        public int? JobId { get; set; }
        public virtual Job Job { get; set; }


    }
}
