using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace ClassSchedule.Domain.Models
{
    // Чтобы добавить данные профиля для пользователя, можно добавить дополнительные свойства в класс ApplicationUser. Дополнительные сведения см. по адресу: http://go.microsoft.com/fwlink/?LinkID=317594.
    public class ApplicationUser : IdentityUser
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Обратите внимание, что authenticationType должен совпадать с типом, определенным в CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Здесь добавьте утверждения пользователя
            return userIdentity;
        }

        /// <summary>
        /// Фамилия
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// Имя
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Отчество
        /// </summary>
        public string MiddleName { get; set; }

        /// <summary>
        /// Номер редактируемой недели (по порядку)
        /// </summary>
        public int WeekNumber { get; set; }

        /// <summary>
        /// Курс для которого редактируется расписание
        /// </summary>
        public int? CourseId { get; set; }
        public virtual Course Course { get; set; }

        /// <summary>
        /// Группа для которой редактируется расписание
        /// </summary>
        public int? GroupId { get; set; }
        public virtual Group Group { get; set; }

        /// <summary>
        /// Учебный год
        /// </summary>
        public int? EducationYearId { get; set; }
        public virtual EducationYear EducationYear { get; set; }

        /// <summary>
        /// Список факультетов за расписание которые ответственнен пользователь
        /// </summary>
        public virtual ICollection<Faculty> Faculties { get; set; } 

        /// <summary>
        /// Инициалы и фамилия: Иванов И.И.
        /// </summary>
        public string ShortName
        {
            get
            {
                const string initialTerminator = ".";
                var shortName = LastName + " " + FirstName[0] + initialTerminator;
                if (!String.IsNullOrEmpty(MiddleName))
                {
                    shortName += MiddleName[0] + initialTerminator;
                }

                return shortName;
            }
        }      
    }

    public class ApplicationRole : IdentityRole
    {
        public ApplicationRole() : base() { }
        public ApplicationRole(string name) : base(name) { }
        public string FullName { get; set; }
    }
}