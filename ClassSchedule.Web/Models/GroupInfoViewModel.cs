using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ClassSchedule.Web.Models
{
    public class GroupInfoViewModel
    {
        public int GroupId { get; set; }

        public int ProgramOfEducationId { get; set; }

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
    }
}