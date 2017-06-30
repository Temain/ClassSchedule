﻿using System.Collections.Generic;
using ClassSchedule.Domain.Models.QueryResults;

namespace ClassSchedule.Business.Interfaces
{
    public interface IAuditoriumService 
    {
        /// <summary>
        /// Аудитории определенного корпуса с проверкой на занятость
        /// Если аудитория занята, в поле Employment будет список групп
        /// </summary>
        List<AuditoriumQueryResult> AuditoriumWithEmployment(int? chairId, int housingId,
            int weekNumber, int dayNumber, int classNumber, int currentGroupId);  
    }
}