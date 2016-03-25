using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClassSchedule.Domain.Models
{

    /// <summary>
    /// Кафедра 
    /// </summary>
    [Table("Chair", Schema = "dbo")]
    public partial class Chair
    {
        /// <summary>
        /// Идентификатор кафедры
        /// </summary>
        public int ChairId { get; set; }

        /// <summary>
        /// Идентификатор кафедры для обмена - в mdChair поле DivisionId
        /// </summary>
        public Guid? ChairGuid { get; set; }

        /// <summary>
        /// Факультет, к которому относится кафедра
        /// </summary>
        [ForeignKey("Faculty")]
        public int FacultyId { get; set; }
        public virtual Faculty Faculty { get; set; }

        /// <summary>
        /// Код подразделения
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
        /// Код подразделения в программе УП ВПО
        /// </summary>
        public int? DivisionCodeVpo { get; set; }

        /// <summary>
        /// Вышестоящее подразделение
        /// </summary>
        public Guid? ParentId { get; set; }

        /// <summary>
        /// Это факультет или нет
        /// </summary>
        public bool? IsFaculty { get; set; }

        /// <summary>
        /// Отметка об удалении записи
        /// </summary>
        public bool? IsDeleted { get; set; }

        /// <summary>
        /// Дата последнего обновления записи
        /// </summary>
        public DateTime? UpdatedAt { get; set; }




        /// <summary>
        /// Привязка сотрудников к кафедре
        /// </summary>
        public virtual ICollection<Job> Jobs { get; set; }

        /// <summary>
        /// Привязка дисциплин к кафедре
        /// </summary>
        public virtual ICollection<Discipline> Disciplines { get; set; }

        /// <summary>
        /// Аудитории
        /// </summary>
        public virtual ICollection<Auditorium> Auditoriums { get; set; }
    }
}
