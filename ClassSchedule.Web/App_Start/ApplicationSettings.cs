using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ClassSchedule.Web
{
    public class ApplicationSettings
    {
    }

    /// <summary>
    /// Тип контроля
    /// </summary>
    public enum SessionControlType
    {
        /// <summary>
        /// Экзамен
        /// </summary>
        Exam = 1,

        /// <summary>
        /// Зачёт
        /// </summary>
        Check = 2
    }

    /// <summary>
    /// Типы занятий
    /// </summary>
    public enum LessonTypes
    {
        /// <summary>
        /// Лекция
        /// </summary>
        Lection = 1
    }
}