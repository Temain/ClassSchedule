using System;
using ClassSchedule.Domain.Models;

namespace ClassSchedule.Business.Helpers
{    
    public static class DateHelpers
    {     
        /// <summary>
        /// Определение даты занятия по номеру недели, дня и дате начала учебного года
        /// </summary>
        public static DateTime DateOfLesson(DateTime yearStartDate, int weekNumber, int dayNumber)
        {
            int delta = System.DayOfWeek.Monday - yearStartDate.DayOfWeek;
            DateTime firstMonday = yearStartDate.AddDays(delta);
            return firstMonday.AddDays(((weekNumber - 1) * 7) + dayNumber - 1);
        }

        /// <summary>
        /// Определение недели занятия по дате
        /// </summary>
        public static int WeekOfLesson(DateTime yearStartDate, DateTime date)
        {
            int delta = System.DayOfWeek.Monday - yearStartDate.DayOfWeek;
            DateTime firstMonday = yearStartDate.AddDays(delta);
            return (date - firstMonday).Days / 7 + 1;
        }

        /// <summary>
        /// Метод проверяет ситуацию, когда проверяемая дата меньше чем начало учебного года
        /// </summary>
        /// <param name="educationYear">Учебный год</param>
        /// <param name="dateCheck">Проверяемая дата</param>
        /// <returns></returns>
        private static bool DateBeforeEducationYear(EducationYear educationYear, DateTime dateCheck)
        {
            return dateCheck < educationYear.DateStart;
        }


        /// <summary>
        /// Метод проверяет вхождение даты в диапазон выбранного учебного года
        /// </summary>
        /// <param name="educationYear">Учебный год</param>
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