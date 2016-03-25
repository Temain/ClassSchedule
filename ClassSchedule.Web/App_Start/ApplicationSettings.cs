using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ClassSchedule.Web
{
    public class ApplicationSettings
    {
    }

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
}