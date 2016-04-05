using System.Collections.Generic;
using ClassSchedule.Domain.DataAccess.Repositories;
using ClassSchedule.Domain.Models;
using ClassSchedule.Domain.Models.QueryResults;

namespace ClassSchedule.Domain.DataAccess.Interfaces
{
    public interface IAuditoriumRepository : IGenericRepository<Auditorium>
    {
        /// <summary>
        /// Аудитории определенного корпуса с проверкой на занятость
        /// Если аудитория занята, в поле Employment будет список групп
        /// </summary>
        List<AuditoriumQueryResult> AuditoriumWithEmployment(int? chairId, int housingId,
            int weekNumber, int dayNumber, int classNumber, int currentGroupId);  
    }
}
