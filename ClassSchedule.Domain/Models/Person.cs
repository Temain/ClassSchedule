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
        public Person()
        {
            PersonGuid = Guid.NewGuid();
        }

        /// <summary>
        /// Идентификатор
        /// </summary>
        public int PersonId { get; set; }

        /// <summary>
        /// Идентификатор для систем обмена
        /// </summary>
        public Guid? PersonGuid { get; set; }

        /// <summary>
        /// Идентификатор для общих Guid-ов задублированных персон из Megabase
        /// </summary>
        public Guid? PersonMasterGuid { get; set; }

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
        /// Пол
        /// </summary>
        public byte? Sex { get; set; }

        /// <summary>
        /// Источник данных конкретной записи о персоне
        /// </summary>
        //[ForeignKey("DataSourceType")]
        //public int? DataSourceTypeId { get; set; }
        //public virtual DataSourceType DataSourceType { get; set; }

        /// <summary>
        /// Номер паспорта
        /// </summary>
        public string PassportNumber { get; set; }

        /// <summary>
        /// Серия паспорта
        /// </summary>
        public string PassportSeries { get; set; }

        /// <summary>
        /// Отметка о том, что персона задублирована
        /// </summary>
        public bool? IsMarkedAsDuplicated { get; set; }

        /// <summary>
        /// Отметка об удалении записи
        /// </summary>
        public bool? IsDeleted { get; set; }

        /// <summary>
        /// Дата удалении записи
        /// </summary>
        public DateTime? UpdatedAt { get; set; }

        
        public virtual ICollection<Employee> Employees { get; set; }


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
