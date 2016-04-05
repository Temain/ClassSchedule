using System.Collections.Generic;
using ClassSchedule.Domain.DataAccess.Repositories;
using ClassSchedule.Domain.Models;
using ClassSchedule.Domain.Models.QueryResults;

namespace ClassSchedule.Domain.DataAccess.Interfaces
{
    public interface IHousingRepository : IGenericRepository<Housing>
    {
        /// <summary>
        /// Наименования корпусов одинаковой длинны
        /// Используется для выпадающих списков, чтобы выстроить всё ровно в несколько колонок
        /// </summary>
        List<HousingQueryResult> HousingEqualLength();  
    }
}
