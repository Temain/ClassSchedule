using System;
using System.Collections.Generic;
using System.Web.Mvc;
using ClassSchedule.Business.Models.Schedule;

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
            string[] days = { "Пн", "Вт", "Ср", "Чт", "Пт", "Сб", "Вс" };

            return days[dayNumber];
        }

        ///// <summary>
        ///// Определение даты занятия по номеру недели, дня и дате начала учебного года
        ///// </summary>
        //public static DateTime DateOfLesson(DateTime yearStartDate, int weekNumber, int dayNumber)
        //{
        //    int delta = System.DayOfWeek.Monday - yearStartDate.DayOfWeek;
        //    DateTime firstMonday = yearStartDate.AddDays(delta);
        //    return firstMonday.AddDays(((weekNumber - 1) * 7) + dayNumber - 1);
        //}

        ///// <summary>
        ///// Определение недели занятия по дате
        ///// </summary>
        //public static int WeekOfLesson(DateTime yearStartDate, DateTime date)
        //{
        //    int delta = System.DayOfWeek.Monday - yearStartDate.DayOfWeek;
        //    DateTime firstMonday = yearStartDate.AddDays(delta);
        //    return (date - firstMonday).Days / 7 + 1;
        //}

        /// <summary>
        /// Возвращает преподавателей и аудиторию занятия
        /// </summary>
        [Obsolete]
        public static MvcHtmlString TeachersWithAuditorium(IEnumerable<LessonPartViewModel> lessonDetails)
        {
            var teachers = new List<string>();
            foreach (var lessonDetail in lessonDetails)
            {
                var hasDowntime = lessonDetail.TeacherHasDowntime ? "red" : "";
                var teacher = @"<span class='teacher " + hasDowntime + "' data-teacher='" + lessonDetail.TeacherId + "'>" + ScheduleHelpers.PersonShortName(lessonDetail.TeacherLastName, lessonDetail.TeacherFirstName, lessonDetail.TeacherMiddleName) + "</span>"
                    + "<span class='auditorium' data-auditorium='" + lessonDetail.AuditoriumId + "'>" +"(" + lessonDetail.AuditoriumName + ")</span>";
                teachers.Add(teacher);
            }

            var result = string.Join(",", teachers);

            return MvcHtmlString.Create(result);
        }

        /// <summary>
        /// Инициалы и фамилия: И.И. Иванов
        /// </summary>
        public static string PersonShortName(string lastName, string firstName, string middleName)
        {
            const string initialTerminator = ".";
            var shortName = lastName;

            if (!string.IsNullOrEmpty(firstName))
            {
                shortName += " " + firstName[0] + initialTerminator;
            }

            if (!string.IsNullOrEmpty(middleName))
            {
                shortName += middleName[0] + initialTerminator;
            }

            return shortName;
        }

        /// <summary>
        /// Возвращает тип расписания в графике учебного плана по сокращению
        /// Например, Т - Теоретическое обучение, К - Каникулы и т.д.
        /// </summary>
        public static Dictionary<string, string> ScheduleTypeByAbbr(char abbreviation)
        {
            var types = new Dictionary<char, Dictionary<string,string>>
            {
                {'Т', new Dictionary<string, string> { {"Name" , "Теоретическое обучение"}, { "Color" , "#7cb5ec"} } },
                {'Э', new Dictionary<string, string> { {"Name" , "Экзаменационные сессии"}, { "Color" , "#7798BF"} } },
                {'К', new Dictionary<string, string> { {"Name" , "Каникулы"}, { "Color" , "#f7a35c"} } },
                {'Н', new Dictionary<string, string> { {"Name" , "Научно-исследовательская работа"}, { "Color" , "#DDDF00"} } },
                {'У', new Dictionary<string, string> { {"Name" , "Учебная практика"}, { "Color" , "#90ee7e"} } },
                {'П', new Dictionary<string, string> { {"Name" , "Производственная практика"}, { "Color" , "#eeaaee"} } },
                {'Д', new Dictionary<string, string> { {"Name" , "Выпускная квалификационная работа"}, { "Color" , "#FF9655"} } },
                {'Г', new Dictionary<string, string> { {"Name" , "Гос. Экзамены и/или защита ВКР"}, { "Color" , "#FFF263"} } }
            };

            return types[abbreviation];
        }
    }
}