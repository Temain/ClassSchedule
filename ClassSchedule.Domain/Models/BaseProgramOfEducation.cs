using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClassSchedule.Domain.Models
{
    /// <summary>
    /// Основная программа обучения
    /// </summary>
    [Table("BaseProgramOfEducation", Schema = "dbo")]
    public class BaseProgramOfEducation
    {
        /// <summary>
        /// Идентификатор 
        /// </summary>
        public int BaseProgramOfEducationId { get; set; }

        /// <summary>
        /// Идентификатор для обмена
        /// </summary>
        public Guid? BaseProgramOfEducationGuid { get; set; }

        /// <summary>
        /// Версия стандарта (для совместимости и будущих версий образовательных стандартов)
        /// </summary>
        [ForeignKey("FederalEducationalStandard")]
        public int FederalEducationalStandardId { get; set; }
        public FederalEducationalStandard FederalEducationalStandard { get; set; }

        /// <summary>
        /// Наименование основной образовательной программы
        /// </summary>
        [StringLength(1024)]
        public string Name { get; set; }


        /// <summary>
        /// Срок обучения, в месяцах
        /// </summary>
        public int? NumberOfMonths { get; set; }
        
        /// <summary>
        /// Направление обучения
        /// </summary>
        [ForeignKey("EducationDirection")]
        public int EducationDirectionId { get; set; }
        public EducationDirection EducationDirection { get; set; }

        /// <summary>
        /// Профиль обучения
        /// </summary>
        [ForeignKey("EducationProfile")]
        public int EducationProfileId { get; set; }
        public EducationProfile EducationProfile { get; set; }

        /// <summary>
        /// Форма обучения
        /// </summary>
        [ForeignKey("EducationForm")]
        public int EducationFormId { get; set; }
        public EducationForm EducationForm { get; set; }

        /// <summary>
        /// Квалификация
        /// </summary>
        [ForeignKey("Qualification")]
        public int QualificationId { get; set; }
        public Qualification Qualification { get; set; }

        /// <summary>
        /// Уровень образования
        /// </summary>
        [ForeignKey("EducationLevel")]
        public int EducationLevelId { get; set; }
        public EducationLevel EducationLevel { get; set; }

        /// <summary>
        /// Год начала подготовки 
        /// </summary>
        public int EducationYearStartId { get; set; }
        [ForeignKey("EducationYearStartId")]
        public EducationYear EducationYearStart { get; set; }

        /// <summary>
        /// Основание для ускоренного обучения (на базе чего происходит обучение - СПО, ВПО)
        /// </summary>
        [ForeignKey("BaseOfAcceleration")]
        public int? BaseOfAccelerationId { get; set; }
        public BaseOfAcceleration BaseOfAcceleration { get; set; }

        /// <summary>
        /// Дата последнего обновления записи
        /// </summary>
        public DateTime? UpdatedAt { get; set; }

        /// <summary>
        /// Дата удаления записи
        /// </summary>
        public DateTime? DeletedAt { get; set; }



        /// <summary>
        /// Группы, которые обучаются по программе: есть курсы с группами у которых разные профили обучения
        /// </summary>
        public List<Group> Groups { get; set; }

        /// <summary>
        /// Рассчеты нагрузки
        /// </summary>
        public List<Discipline> Disciplines { get; set; }

        /// <summary>
        /// График обучения
        /// </summary>
        public List<CourseSchedule> CourseSchedules { get; set; }
    }
}
