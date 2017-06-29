using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClassSchedule.Domain.Models
{
    /// <summary>
    /// Физическое лицо
    /// </summary>
    [Table("Person", Schema = "dbo")]
    public class Person
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        public int PersonId { get; set; }

        /// <summary>
        /// Идентификатор для систем обмена
        /// </summary>
        public Guid? PersonGuid { get; set; }

        /// <summary>
        /// Код физического лица
        /// </summary>
        [StringLength(20)]
        public string PersonCode { get; set; }

        /// <summary>
        /// Фамилия
        /// </summary>
        [Required]
        [StringLength(200)]
        public string LastName { get; set; }

        /// <summary>
        /// Имя
        /// </summary>
        [Required]
        [StringLength(200)]
        public string FirstName { get; set; }

        /// <summary>
        /// Отчество
        /// </summary>
        [StringLength(200)]
        public string MiddleName { get; set; }

        /// <summary>
        /// Дата рождения
        /// </summary>
        public DateTime? Birthday { get; set; }

        /// <summary>
        /// Ученая степень
        /// </summary>
        [ForeignKey("AcademicDegree")]
        public int? AcademicDegreeId { get; set; }
        public AcademicDegree AcademicDegree { get; set; }

        /// <summary>
        /// Научное звание
        /// </summary>
        [ForeignKey("AcademicStatus")]
        public int? AcademicStatusId { get; set; }
        public AcademicStatus AcademicStatus { get; set; }

        /// <summary>
        /// Дата последнего обновления записи
        /// </summary>
        public DateTime? UpdatedAt { get; set; }

        /// <summary>
        /// Дата удаления записи
        /// </summary>
        public DateTime? DeletedAt { get; set; }


        public List<Employee> Employees { get; set; }


        /// <summary>
        /// Метод возвращает фамилию имя отчество
        /// </summary>
        public string FullName
        {
            get
            {
                var fullName = LastName + " " + FirstName;
                if (!String.IsNullOrEmpty(MiddleName))
                {
                    fullName += " " + MiddleName;
                }
                return fullName;
            }
        }

        /// <summary>
        /// Инициалы и фамилия: И.И. Иванов
        /// </summary>
        public string ShortName
        {
            get
            {
                const string initialTerminator = ".";
                var shortName = FirstName[0] + initialTerminator;

                if (String.IsNullOrEmpty(MiddleName))
                    shortName += LastName;
                else
                    shortName += MiddleName[0] + initialTerminator + " " + LastName;

                return shortName;     
            }
        }

    }

}
