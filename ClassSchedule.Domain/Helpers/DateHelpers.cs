using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClassSchedule.Domain.Models;

namespace ClassSchedule.Domain.Helpers
{
    public static class DateHelpers
    {
        /// <summary>
        /// Метод проверяет ситуацию, когда проверяемая дата меньше чем начало учебного года
        /// </summary>
        /// /// <param name="educationYear">Учебный год</param>
        /// <param name="dateCheck">Проверяемая дата</param>
        /// <returns></returns>
        private static bool DateBeforeEducationYear(EducationYear educationYear, DateTime dateCheck)
        {
            return dateCheck < educationYear.DateStart;
        }


        /// <summary>
        /// Метод проверяет вхождение даты в диапазон выбранного учебного года
        /// </summary>
        /// /// <param name="educationYear">Учебный год</param>
        /// <param name="dateCheck">Проверяемая дата</param>
        /// <returns></returns>
        private static bool DateInEducationYear(EducationYear educationYear, DateTime? dateCheck)
        {
            if (dateCheck != null)
                return (educationYear.DateStart <= dateCheck && dateCheck <= educationYear.DateEnd);

            return true;
        }


        /// <summary>
        /// Метод возвращает ситуацию, когда дата больше чем выбранный учебный год
        /// </summary>
        /// <param name="educationYear">Учебный год</param>
        /// <param name="dateCheck">Проверяемая дата</param>
        /// <returns></returns>
        private static bool DateAfterEducationYear(EducationYear educationYear, DateTime? dateCheck)
        {
            if (dateCheck != null)
                return dateCheck > educationYear.DateEnd;

            return true;
        }


        /// <summary>
        /// Метод проверяет вхождение диапазона проверяемых дат в выбранный учебный год
        /// включая ситуации, когда дата окончания диапазона пустая
        /// </summary>
        /// <param name="educationYear">Учебный год</param>
        /// <param name="dateStart">Дата начала проверяемого диапазона</param>
        /// <param name="dateEnd">Дата окончания проверяемого диапазона</param>
        /// <returns></returns>
        public static bool DatesIsActual(EducationYear educationYear, DateTime dateStart, DateTime? dateEnd)
        {
            var actual1 = DateBeforeEducationYear(educationYear, dateStart) || DateInEducationYear(educationYear, dateStart);

            if (dateEnd.HasValue)
            {
                var actual2 = DateInEducationYear(educationYear, dateEnd) || DateAfterEducationYear(educationYear, dateEnd);

                return actual1 && actual2;
            }

            return actual1;
        }
    }
}
