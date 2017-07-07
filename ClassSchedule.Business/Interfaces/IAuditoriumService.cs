using System.Collections.Generic;
using ClassSchedule.Business.Models;

namespace ClassSchedule.Business.Interfaces
{
    public interface IAuditoriumService 
    {
        /// <summary>
        /// Аудитории определенного корпуса с проверкой на занятость
        /// Если аудитория занята, в поле Employment будет список групп
        /// </summary>
        List<AuditoriumViewModel> AuditoriumWithEmployment(int housingId, int weekNumber, int dayNumber, int classNumber, int currentGroupId, int? chairId = null);  
    }
}
