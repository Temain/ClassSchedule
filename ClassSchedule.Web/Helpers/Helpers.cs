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

        public static string DayOfWeek(int dayNumber)
        {
            string[] days = {"Пн","Вт","Ср","Чт","Пт","Сб","Вс"};

            return days[dayNumber];
        }

        public static DateTime DateOfLesson(DateTime yearStartDate, int weekNumber, int dayNumber)
        {
            int delta = System.DayOfWeek.Monday - yearStartDate.DayOfWeek;
            DateTime firstMonday = yearStartDate.AddDays(delta);
            return firstMonday.AddDays((weekNumber * 7) + dayNumber - 1);
        }
    }
}