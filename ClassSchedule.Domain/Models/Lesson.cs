using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassSchedule.Domain.Models
{
    /// <summary>
    /// Занятие
    /// </summary>
    [Table("Lesson", Schema = "dbo")]
    public class Lesson
    {
        public int LessonId { get; set; }

        public Guid? LessonGuid { get; set; }

        /// <summary>
        /// Номер пары
        /// </summary>
        public int ClassNumber { get; set; }

        /// <summary>
        /// Дата занятия
        /// </summary>
        public DateTime ClassDate { get; set; }

        /// <summary>
        /// Номер недели
        /// </summary>
        public int WeekNumber { get; set; }

        /// <summary>
        /// Номер дня недели
        /// </summary>
        public int DayNumber { get; set; }

        /// <summary>
        /// Кафедра
        /// </summary>
        //public int ChairId { get; set; }
        //public virtual Chair Chair { get; set; }

        /// <summary>
        /// Преподаватель
        /// </summary>
        public int JobId { get; set; }
        public virtual Job Job { get; set; }

        /// <summary>
        /// Дисциплина
        /// </summary>
        public int DisciplineId { get; set; }
        public virtual Discipline Discipline { get; set; }

        /// <summary>
        /// Аудитория
        /// </summary>
        public int AuditoriumId { get; set; }
        public virtual Auditorium Auditorium { get; set; }

        /// <summary>
        /// Группа
        /// </summary>
        public int GroupId { get; set; }
        public virtual Group Group { get; set; }

        /// <summary>
        /// Тип занятия
        /// </summary>
        public int? LessonTypeId { get; set; }
        public virtual LessonType LessonType { get; set; }

        /// <summary>
        /// Учебный год
        /// </summary>
        public int? EducationYearId { get; set; }
        public virtual EducationYear EducationYear { get; set; }

        /// <summary>
        /// Неактивно
        /// </summary>
        public bool IsNotActive { get; set; }


        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public DateTime? DeletedAt { get; set; }
    }
}
