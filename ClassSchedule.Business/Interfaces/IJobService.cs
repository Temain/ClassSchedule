using System.Collections.Generic;
using ClassSchedule.Business.Models;
using ClassSchedule.Domain.Models;
using ClassSchedule.Domain.Models.QueryResults;

namespace ClassSchedule.Business.Interfaces
{
    public interface IJobService
    {
        /// <summary>
        /// Преподаватели, работающие в определенном учебном году
        /// </summary>
        List<TeacherViewModel> ActualTeachers(int educationYearId, int? chairId, string query = null, int? take = null);

        /// <summary>
        /// Преподаватели, работающие в определенном учебном году
        /// со списком групп, у которых они ведут занятия на определённой паре
        /// Используется при редактировании занятия (выдача подсказки о занятости преподавателя)
        /// </summary>
        List<TeacherViewModel> ActualTeachersWithEmployment(int educationYearId, int weekNumber, int dayNumber, int classNumber
            , int currentGroupId, int? disciplineId = null, int? chairId = null);

        /// <summary>
        /// Окна между занятиями у преподавателей
        /// </summary>
        /// <param name="weekNumber">Номер недели</param>
        /// <param name="teacherId">Идентификатор преподавателя (JobId)</param>
        /// <param name="maxDiff">Размер окна (количество занятий)</param>
        List<TeacherDowntimeQueryResult> TeachersDowntime(int weekNumber, int? chairJobId = 0, int maxDiff = 1);

        /// <summary>
        /// Окна между занятиями у преподавателей на несколько недель
        /// Преподаватели выбираются независимо от редактируемой пользователем недели и групп
        /// </summary>
        /// <param name="weeks">Номера недель</param>
        /// <param name="teacherId">Идентификатор преподавателя (JobId)</param>
        /// <param name="maxDiff">Размер окна (количество занятий)</param>
        List<TeacherDowntimeQueryResult> TeachersDowntime(int[] weeks, int? chairJobId = 0, int maxDiff = 1);
    }
}
