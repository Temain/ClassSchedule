﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClassSchedule.Domain.Models
{
    /// <summary>
    /// Детализация графика учебного процесса по курсам
    /// </summary>
    [Table("CourseSchedule", Schema = "plan")]
    public class CourseSchedule
    {
        public int CourseScheduleId { get; set; }

        /// <summary>
        /// Номер курса
        /// </summary>
        public int CourseNumber { get; set; }

        /// <summary>
        /// Максимальная нагрузка 1
        /// </summary>
        public float FirstMaxLoad { get; set; }

        /// <summary>
        /// Максимальная нагрузка 2
        /// </summary>
        public float SecondMaxLoad { get; set; }

        /// <summary>
        /// Теоретическое обучение, недель
        /// </summary>
        public int TheoreticalTrainingWeeks { get; set; }

        /// <summary>
        /// Экзаменационные сессии, недель
        /// </summary>
        public int ExamSessionWeeks { get; set; }

        /// <summary>
        /// Учебные практики, недель
        /// </summary>
        public int StudyTrainingWeeks { get; set; }

        /// <summary>
        /// Производственные практики, недель
        /// </summary>
        public int PracticalTrainingWeeks { get; set; }

        /// <summary>
        /// Выпускная квалификационная работа, недель
        /// </summary>
        public int FinalQualifyingWorkWeeks { get; set; }

        /// <summary>
        /// Гос. экзамены и/или защита ВКР, недель
        /// </summary>
        public int StateExamsWeeks { get; set; }

        /// <summary>
        /// Каникулы, недель
        /// </summary>
        public int WeeksOfHolidays { get; set; }

        /// <summary>
        /// Научно-исследовательская работа, недель
        /// </summary>
        public int ResearchWorkWeeks { get; set; }

        /// <summary>
        /// График учебного процесса
        /// </summary>
        public string Schedule { get; set; }

        public int NumberOfLastWeek
        {
            get { return Schedule.Length; }
        }

        /// <summary>
        /// Основная образовательная программа
        /// </summary>
        public int BaseProgramOfEducationId { get; set; }
        public BaseProgramOfEducation BaseProgramOfEducation { get; set; }

        /// <summary>
        /// Дата последнего обновления записи
        /// </summary>
        public DateTime? UpdatedAt { get; set; }

        /// <summary>
        /// Дата удаления записи
        /// </summary>
        public DateTime? DeletedAt { get; set; }

        /// <summary>
        /// Детализация графика учебного процесса
        /// для конкретного курса по семестрам
        /// </summary>
        public List<SemesterSchedule> SemesterSchedules { get; set; } 
    }
}
