using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using ClassSchedule.Business.Interfaces;
using ClassSchedule.Business.Models;
using ClassSchedule.Domain.Context;
using ClassSchedule.Domain.Models.QueryResults;

namespace ClassSchedule.Business.Services
{
    public class AuditoriumService : IAuditoriumService
    {
        private readonly ApplicationDbContext _context;

        public AuditoriumService(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Аудитории определенного корпуса с проверкой на занятость
        /// Если аудитория занята, в поле Employment будет список групп
        /// Длинна AuditoriumName + ' ' + AuditoriumTypeName равна во всех строках (для выпадающего списка)
        /// </summary>
        public List<AuditoriumViewModel>AuditoriumWithEmployment(int housingId, int weekNumber,
            int dayNumber, int classNumber, int currentGroupId, int? chairId = null)
        {
            var parameters = new object[]
            {
                new SqlParameter("@chairId", chairId ?? 0),
                new SqlParameter("@housingId", housingId),
                new SqlParameter("@weekNumber", weekNumber),
                new SqlParameter("@dayNumber", dayNumber),
                new SqlParameter("@classNumber", classNumber),
                new SqlParameter("@groupId", currentGroupId)
            };

            var query = @"
                DECLARE @auditoriums TABLE(
                  AuditoriumId INT NOT NULL,  
                  AuditoriumNumber VARCHAR(MAX),
                  ChairId INT,
                  Places INT NOT NULL,
                  AuditoriumTypeName VARCHAR(MAX),
                  Comment VARCHAR(MAX)
                )

                INSERT INTO @auditoriums (AuditoriumId, AuditoriumNumber, ChairId, Places, AuditoriumTypeName, Comment)
                SELECT a.AuditoriumId, a.AuditoriumNumber, CASE a.ChairId WHEN @chairId THEN a.ChairId ELSE NULL END AS ChairId, a.Places, at.AuditoriumTypeName, a.Comment
                FROM Auditorium a
                LEFT JOIN dict.AuditoriumType at ON a.AuditoriumTypeId = at.AuditoriumTypeId
                LEFT JOIN dict.Housing h ON a.HousingId = h.HousingId
                WHERE a.HousingId = @housingId AND a.DeletedAt IS NULL;

                DECLARE @auditoriumsLessons TABLE(
                  AuditoriumId INT NOT NULL,
                  GroupName VARCHAR(MAX)
                )

                INSERT INTO @auditoriumsLessons (AuditoriumId, GroupName)
                SELECT a2.AuditoriumId, g.GroupName AS GroupName
                FROM LessonDetail ld
                LEFT JOIN Lesson l ON ld.LessonId = l.LessonId
                LEFT JOIN Schedule s ON l.ScheduleId = s.ScheduleId
                LEFT JOIN dbo.[Group] g ON s.GroupId = g.GroupId
                INNER JOIN @auditoriums AS a2 ON a2.AuditoriumId = ld.AuditoriumId
                WHERE s.DeletedAt IS NULL
                  AND s.WeekNumber = @weekNumber AND s.DayNumber = @dayNumber
                  AND s.ClassNumber = @classNumber
                  AND s.GroupId <> @groupId
                  AND ld.DeletedAt IS NULL;

                DECLARE @MaxLength INT;
                SELECT @MaxLength = (SELECT MAX(LEN(am.AuditoriumNumber + at.AuditoriumTypeName))
                  FROM Auditorium am
                  LEFT JOIN dict.AuditoriumType at ON am.AuditoriumTypeId = at.AuditoriumTypeId
                );

                SELECT aud.AuditoriumId, aud.AuditoriumNumber, aud.ChairId, aud.Places, aud.Comment, als.Employment,
                  LEFT(aud.AuditoriumTypeName + space(@maxLength), @MaxLength - LEN(aud.AuditoriumNumber) + 10) AS AuditoriumTypeName
                FROM (
                  SELECT les.AuditoriumId, COUNT(*) AS [Count]
                      ,STUFF((
                          SELECT ',' + les2.GroupName
                          from @auditoriumsLessons les2
                          where les2.AuditoriumId = les.AuditoriumId
                          FOR XML PATH(''), TYPE
                      ).value('.', 'varchar(max)'), 1, 1, '') Employment
                  FROM @auditoriumsLessons les
                  GROUP BY les.AuditoriumId
                ) AS als
                RIGHT JOIN @auditoriums aud ON aud.AuditoriumId = als.AuditoriumId
                ORDER BY 
                  CASE COALESCE(aud.ChairId, '') 
                    WHEN @chairId THEN 1 
                    WHEN '' THEN 3
                    ELSE 2
                  END, 
                  aud.AuditoriumNumber;";
            var auditoriums = _context.Database.SqlQuery<AuditoriumViewModel>(query, parameters).ToList();

            return auditoriums;
        }
    }
}
