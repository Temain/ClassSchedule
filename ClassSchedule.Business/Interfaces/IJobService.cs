using System.Collections.Generic;
using ClassSchedule.Domain.Models;
using ClassSchedule.Domain.Models.QueryResults;

namespace ClassSchedule.Business.Interfaces
{
    public interface IJobService
    {
        /// <summary>
        /// Преподаватели, работающие в определенном учебном году
        /// </summary>
        List<TeacherQueryResult> ActualTeachers(EducationYear educationYear, int? chairId, string query = "");

        /// <summary>
        /// Преподаватели, работающие в определенном учебном году
        /// со списком групп, у которых они ведут занятия на определённой паре
        /// Используется при редактировании занятия (выдача подсказки о занятости преподавателя)
        /// </summary>
        List<TeacherQueryResult> ActualTeachersWithEmployment(EducationYear educationYear, int? chairId,
            int weekNumber, int dayNumber, int classNumber, int currentGroupId);

        /// <summary>
        /// Окна между занятиями у преподавателей
        /// </summary>
        /// <param name="weekNumber">Номер недели</param>
        /// <param name="teacherId">Идентификатор преподавателя (JobId)</param>
        /// <param name="maxDiff">Размер окна (количество занятий)</param>
        List<TeacherDowntimeQueryResult> TeachersDowntime(int weekNumber, int? teacherId = 0, int maxDiff = 2);
    }
}
