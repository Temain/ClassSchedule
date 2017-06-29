using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClassSchedule.Domain.Models
{
    /// <summary>
    /// Дисциплина (Предмет)
    /// </summary>
    [Table("Discipline", Schema = "dbo")]
    public partial class Discipline
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        public int DisciplineId { get; set; }
        
        /// <summary>
        /// Идентификатор для обмена
        /// </summary>
        public Guid? DisciplineGuid { get; set; }

        /// <summary>
        /// Название дисциплины через справочник наименований дисциплин
        /// </summary>
        [ForeignKey("DisciplineName")]
        public int DisciplineNameId { get; set; }
        public DisciplineName DisciplineName { get; set; }

        /// <summary>
        /// Основная программа обучения - комплексная сущность, включающая форму, направление обучения и т.п.
        /// </summary>
        [ForeignKey("BaseProgramOfEducation")]
        public int? BaseProgramOfEducationId { get; set; }
        public BaseProgramOfEducation BaseProgramOfEducation { get; set; }

        /// <summary>
        /// Семестр, в котором читается дисциплина
        /// </summary>
        [ForeignKey("EducationSemester")]
        public int EducationSemesterId { get; set; }
        public EducationSemester EducationSemester { get; set; }

        /// <summary>
        /// Идентификатор дисциплины, с которой было выполнено слияние
        /// </summary>
        public int? CombinedWithDisciplineId { get; set; }
        public Discipline CombinedWithDiscipline { get; set; }

        /// <summary>
        /// Форма контроля - экзамен
        /// </summary>
        public bool? FormControlExam { get; set; }

        /// <summary>
        /// Форма контроля - зачет
        /// </summary>
        public bool? FormControlCredit { get; set; }

        /// <summary>
        /// Форма контроля - зачет с оценкой
        /// </summary>
        public bool? FormControlCreditWithGrade { get; set; }

        /// <summary>
        /// Форма контроля - курсовой проект
        /// </summary>
        public bool? FormControlCourseProject { get; set; }

        /// <summary>
        /// Форма контроля - курсовая работа
        /// </summary>
        public bool? FormControlCourseWork { get; set; }

        /// <summary>
        /// Форма контроля - контрольная работа
        /// </summary>
        public bool? FormControlControlWork { get; set; }

        /// <summary>
        /// Форма контроля - реферат
        /// </summary>
        public bool? FormControlEssay { get; set; }

        /// <summary>
        /// Кафедра, на которой читается дисциплина
        /// </summary>
        [ForeignKey("Chair")]
        public int ChairId { get; set; }
        public Chair Chair { get; set; }

        /// <summary>
        /// Если дисциплина помечена, она в расчетах не участвует (факультативы, например)
        /// </summary>
        public bool IsExludedFromCalculation { get; set; }

        /// <summary>
        /// Для нескольких дисциплин по выбору вторую помечаем
        /// </summary>
        public bool IsFakeDiscipline { get; set; }

        /// <summary>
        /// Контейнер (группа) дисциплин
        /// </summary>
        public bool IsHeadOfSectionDiscipline { get; set; }

        /// <summary>
        /// Дата последнего обновления записи
        /// </summary>
        public DateTime? UpdatedAt { get; set; }

        /// <summary>
        /// Дата удаления записи
        /// </summary>
        public DateTime? DeletedAt { get; set; }

        /// <summary>
        /// Распределение групп по потокам для дисциплины
        /// </summary>
        public List<GroupFlowBinding> GroupFlowBindings { get; set; }

        /// <summary>
        /// Дисциплины, которые были объединины под этой дисциплиной
        /// </summary>
        public List<Discipline> CombinedDisciplines { get; set; }

        /// <summary>
        /// Разделение групп на подгруппы по определенной дисциплине
        /// </summary>
        public List<GroupSubgroups> GroupSubgroups { get; set; }

        /// <summary>
        /// Занятия по этой дисциплине
        /// </summary>
        public List<Lesson> Lessons { get; set; }
    }
}
