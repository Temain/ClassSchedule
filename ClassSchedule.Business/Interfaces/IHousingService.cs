using System.Collections.Generic;
using ClassSchedule.Domain.Models.QueryResults;

namespace ClassSchedule.Business.Interfaces
{
    public interface IHousingService
    {
        /// <summary>
        /// Наименования корпусов одинаковой длинны
        /// Используется для выпадающих списков, чтобы выстроить всё ровно в несколько колонок
        /// </summary>
        List<HousingQueryResult> HousingEqualLength();  
    }
}
