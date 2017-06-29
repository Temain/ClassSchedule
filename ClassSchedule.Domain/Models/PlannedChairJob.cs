using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClassSchedule.Domain.Models
{
    /// <summary>
    /// Планируемый штат
    /// </summary>
    [Table("PlannedChairJob", Schema = "dbo")]
    public class PlannedChairJob
    {
        public int PlannedChairJobId { get; set; }

        /// <summary>
        /// Примечание
        /// </summary>
        public string PlannedChairJobComment { get; set; }

        /// <summary>
        /// Учебный год
        /// </summary>
        public int EducationYearId { get; set; }
        public EducationYear EducationYear { get; set; }

        /// <summary>
        /// Кафедра
        /// </summary>
        public int ChairId { get; set; }
        public Chair Chair { get; set; }

        /// <summary>
        /// Должность
        /// </summary>
        public int PositionRealId { get; set; }
        public PositionReal PositionReal { get; set; }

        /// <summary>
        /// Условие работы
        /// </summary>
        public int EmploymentTypeId { get; set; }
        public EmploymentType EmploymentType { get; set; }

        /// <summary>
        /// Ученая степень
        /// </summary>
        public int? AcademicDegreeId { get; set; }
        public AcademicDegree AcademicDegree { get; set; }

        /// <summary>
        /// Ссылка на фактическое место работы
        /// </summary>
        public int? JobId { get; set; }
        public Job Job { get; set; }

        public bool IsHeadOfChair { get; set; }

        /// <summary>
        /// Дата последнего обновления записи
        /// </summary>
        public DateTime? UpdatedAt { get; set; }

        /// <summary>
        /// Дата удаления записи
        /// </summary>
        public DateTime? DeletedAt { get; set; }

        /// <summary>
        /// Занятия, которые проводит данный преподаватель
        /// </summary>
        public List<LessonDetail> LessonDetails { get; set; }
    }
}
