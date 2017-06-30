using System.Collections.Generic;

namespace ClassSchedule.Domain.DataAccess.Interfaces
{
    public interface IHousingRepository
    {
        /// <summary>
        /// Наименования корпусов одинаковой длинны
        /// Используется для выпадающих списков, чтобы выстроить всё ровно в несколько колонок
        /// </summary>
        List<HousingQueryResult> HousingEqualLength();  
    }
}
