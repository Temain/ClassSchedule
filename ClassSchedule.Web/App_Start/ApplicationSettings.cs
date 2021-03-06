﻿using System;
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
        Lection = 1,

        /// <summary>
        /// Семинар
        /// </summary>
        Seminar = 2,

        /// <summary>
        /// Лабораторная работа
        /// </summary>
        LaboratoryWork = 3,

        /// <summary>
        /// Тренировка
        /// </summary>
        Training = 4,

        /// <summary>
        /// Практическое занятие
        /// </summary>
        PracticalLesson = 5,

        /// <summary>
        /// Консультация
        /// </summary>
        Consultation = 6
    }

    /// <summary>
    /// Формы обучения
    /// </summary>
    public enum EducationForms
    {
        /// <summary>
        /// Очная
        /// </summary>
        FullTime = 5,

        /// <summary>
        /// Заочная
        /// </summary>
        Extramural = 3
    }
}