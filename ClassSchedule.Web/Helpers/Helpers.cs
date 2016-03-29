using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;

namespace ClassSchedule.Web.Helpers
{    
    public static class ScheduleHelpers
    {
        /// <summary>
        /// Возвращает время проведения занятия по номеру дня и номеру занятия
        /// </summary>
        public static MvcHtmlString TimeOfLesson(int lessonDay, int lessonNumber)
        {
            string[][] weekdaysTime = new[]
            {
                new[] {"08:00", "09:30"},
                new[] {"09:45", "11:15"},
                new[] {"11:30", "13:00"},
                new[] {"13:50", "15:20"},
                new[] {"15:35", "17:05"},
                new[] {"17:20", "18:50"}
            };

            string[][] saturdayTime = new[]
            {
                new[] {"08:00", "09:30"},
                new[] {"09:45", "11:15"},
                new[] {"11:30", "13:00"},
                new[] {"13:15", "14:45"},
                new[] {"15:00", "16:30"},
                new[] {"16:45", "18:15"}
            };

            string result;
            if (lessonDay != 5) // Не суббота
            {
                result = weekdaysTime[lessonNumber][0] + "<br>" + weekdaysTime[lessonNumber][1];
            }
            else
            {
                result = saturdayTime[lessonNumber][0] + "<br>" + saturdayTime[lessonNumber][1];
            }

            return MvcHtmlString.Create(result);
        }

        /// <summary>
        /// Возвращает сокращение дня недели по его номеру
        /// </summary>
        public static string DayOfWeek(int dayNumber)
        {
            string[] days = {"Пн","Вт","Ср","Чт","Пт","Сб","Вс"};

            return days[dayNumber];
        }

        /// <summary>
        /// Определение даты занятия по номеру недели, дня и дате начала учебного года
        /// </summary>
        public static DateTime DateOfLesson(DateTime yearStartDate, int weekNumber, int dayNumber)
        {
            int delta = System.DayOfWeek.Monday - yearStartDate.DayOfWeek;
            DateTime firstMonday = yearStartDate.AddDays(delta);
            return firstMonday.AddDays((weekNumber * 7) + dayNumber - 1);
        }

        /// <summary>
        /// Инициалы и фамилия: И.И. Иванов
        /// </summary>
        public static string PersonShortName(string lastName, string firstName, string middleName)
        {
            const string initialTerminator = ".";
            var shortName = lastName;

            if (!String.IsNullOrEmpty(firstName))
            {
                shortName += " " + firstName[0] + initialTerminator;
            }

            if (!String.IsNullOrEmpty(middleName))
            {
                shortName += middleName[0] + initialTerminator;
            }

            return shortName;
        }
    }
}