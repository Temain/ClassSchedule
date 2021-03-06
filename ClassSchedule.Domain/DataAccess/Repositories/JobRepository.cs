﻿using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using ClassSchedule.Domain.Context;
using ClassSchedule.Domain.DataAccess.Interfaces;
using ClassSchedule.Domain.Helpers;
using ClassSchedule.Domain.Models;
using ClassSchedule.Domain.Models.QueryResults;

namespace ClassSchedule.Domain.DataAccess.Repositories
{
    public class JobRepository : GenericRepository<Job>, IJobRepository
    {
        private readonly ApplicationDbContext _context;

        public JobRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        /// <summary>
        /// Преподаватели, работающие в определенном учебном году
        /// </summary>
        public List<TeacherQueryResult> ActualTeachers(EducationYear educationYear, int? chairId, string query = "")
        {
            var parameters = new object[]
            {
                new SqlParameter("@chairId", chairId ?? 0),
                new SqlParameter("@query", query),
                new SqlParameter("@startDate", educationYear.DateStart),
                new SqlParameter("@endDate", educationYear.DateEnd)
            };

            var sql = @"
                SELECT t0.PersonId, t0.JobId, t0.FullName
                FROM (
                  SELECT p.PersonId, j.JobId, p.LastName + COALESCE(' ' + p.FirstName, '') + COALESCE(' ' + p.MiddleName, '') AS FullName,
                    ROW_NUMBER() OVER(PARTITION BY e.PersonId ORDER BY j.JobDateStart DESC) as Rn 
                  FROM Job j 
                  LEFT JOIN Employee e ON j.EmployeeId = e.EmployeeId
                  LEFT JOIN Person p ON e.PersonId = p.PersonId
                  WHERE j.ChairId = CASE WHEN @chairId = 0 THEN j.ChairId ELSE @chairId END 
                    AND (j.IsDeleted = 0 OR j.IsDeleted IS NULL)
                    AND (e.IsDeleted = 0 OR e.IsDeleted IS NULL)
                    AND (p.IsDeleted = 0 OR p.IsDeleted IS NULL)

                    -- Проверка что преподаватель работал в определенном учебном году
                    AND (
                      (j.JobDateEnd IS NULL AND (j.JobDateStart < @startDate OR (j.JobDateStart >= @startDate AND j.JobDateStart <= @endDate))) 
                      OR 
                      (j.JobDateEnd IS NOT NULL AND (j.JobDateStart < @startDate OR (j.JobDateStart >= @startDate AND j.JobDateStart <= @endDate))
                        AND (j.JobDateEnd IS NOT NULL AND ((j.JobDateEnd >= @startDate AND j.JobDateEnd <= @endDate) OR j.JobDateEnd > @endDate)))
                    )
                ) AS t0
                WHERE t0.Rn = 1
                  AND t0.FullName LIKE (CASE WHEN @query = '' THEN t0.FullName ELSE @query + '%' END)
                ORDER BY t0.FullName";
            var teachers = _context.Database.SqlQuery<TeacherQueryResult>(sql, parameters).ToList();

            return teachers;
        }

        /// <summary>
        /// Преподаватели, работающие в определенном учебном году
        /// со списком групп, у которых они ведут занятия на определённой паре
        /// Используется при редактировании занятия (выдача подсказки о занятости преподавателя)
        /// </summary>
        public List<TeacherQueryResult> ActualTeachersWithEmployment(EducationYear educationYear, int? chairId,
            int weekNumber, int dayNumber, int classNumber, int currentGroupId)
        {
            var parameters = new object[]
            {
                new SqlParameter("@chairId", chairId),
                new SqlParameter("@startDate", educationYear.DateStart),
                new SqlParameter("@endDate", educationYear.DateEnd),
                new SqlParameter("@weekNumber", weekNumber),
                new SqlParameter("@dayNumber", dayNumber),
                new SqlParameter("@classNumber", classNumber),
                new SqlParameter("@groupId", currentGroupId)
            };

            var query = @"
                DECLARE @teachers TABLE(
                  JobId INT NOT NULL,
                  PersonId INT NOT NULL,
                  FullName VARCHAR(MAX)
                )

                -- Список актуальных на учебный год преподавателей
                INSERT INTO @teachers (JobId, PersonId, FullName)
                SELECT t0.JobId, t0.PersonId, t0.LastName + COALESCE(' ' + t0.FirstName, '') + COALESCE(' ' + t0.MiddleName, '') AS [Value]
                FROM (
                  SELECT j.JobId, p.PersonId, p.LastName, p.FirstName, p.MiddleName,
                    ROW_NUMBER() OVER(PARTITION BY e.PersonId ORDER BY j.JobDateStart DESC) as Rn 
                  FROM Job j 
                  LEFT JOIN Employee e ON j.EmployeeId = e.EmployeeId
                  LEFT JOIN Person p ON e.PersonId = p.PersonId
                  WHERE j.ChairId = @chairId 
                    AND (j.IsDeleted = 0 OR j.IsDeleted IS NULL)
                    AND (e.IsDeleted = 0 OR e.IsDeleted IS NULL)
                    AND (p.IsDeleted = 0 OR p.IsDeleted IS NULL)

                    -- Проверка что преподаватель работал в определенном учебном году
                    AND (
                      (j.JobDateEnd IS NULL AND (j.JobDateStart < @startDate OR (j.JobDateStart >= @startDate AND j.JobDateStart <= @endDate))) 
                      OR 
                      (j.JobDateEnd IS NOT NULL AND (j.JobDateStart < @startDate OR (j.JobDateStart >= @startDate AND j.JobDateStart <= @endDate))
                        AND (j.JobDateEnd IS NOT NULL AND ((j.JobDateEnd >= @startDate AND j.JobDateEnd <= @endDate) OR j.JobDateEnd > @endDate)))
                    )
                ) AS t0
                WHERE t0.Rn = 1;

                -- Занятия, которые ведут эти преподаватели в определенный день на определённой паре
                DECLARE @teachersLessons TABLE(
                  JobId INT NOT NULL,
                  PersonId INT NOT NULL,
                  GroupName VARCHAR(MAX)
                )
                INSERT INTO @teachersLessons (JobId, PersonId, GroupName)
                SELECT j2.JobId, p2.PersonId, g.DivisionName AS GroupName
                FROM Lesson ls
                LEFT JOIN Job j2 ON ls.JobId = j2.JobId
                LEFT JOIN Employee e2 ON j2.EmployeeId = e2.EmployeeId
                LEFT JOIN Person p2 ON e2.PersonId = p2.PersonId
                LEFT JOIN dict.[Group] g ON ls.GroupId = g.GroupId
                INNER JOIN @teachers AS t2 ON p2.PersonId = t2.PersonId
                WHERE ls.DeletedAt IS NULL
                  AND ls.WeekNumber = @weekNumber AND ls.DayNumber = @dayNumber
                  AND ls.ClassNumber = @classNumber
                  AND ls.GroupId <> @groupId;

                SELECT tch.JobId, tch.FullName, tls.Employment
                FROM (
                  SELECT les.PersonId, COUNT(*) AS [Count]
                      ,STUFF((
                          SELECT ',' + les2.GroupName
                          from @teachersLessons les2
                          where les2.PersonId = les.PersonId
                          FOR XML PATH(''), TYPE
                      ).value('.', 'varchar(max)'), 1, 1, '') AS Employment
                  FROM @teachersLessons les
                  GROUP BY les.PersonId
                ) AS tls
                RIGHT JOIN @teachers tch ON tch.PersonId = tls.PersonId
                ORDER BY tch.FullName;";
            var teachers = _context.Database.SqlQuery<TeacherQueryResult>(query, parameters).ToList();

            return teachers;
        }

        /// <summary>
        /// Окна между занятиями у преподавателей на определённую неделю
        /// Преподаватели выбираются в соотвествии с редактируемой пользователем неделей и группами
        /// </summary>
        /// <param name="weekNumber">Номер недели</param>
        /// <param name="teacherId">Идентификатор преподавателя (JobId)</param>
        /// <param name="maxDiff">Размер окна (количество занятий)</param>
        public List<TeacherDowntimeQueryResult> TeachersDowntime(int weekNumber, int? teacherId = 0, int maxDiff = 1)
        {
            var parameters = new object[]
            {
                new SqlParameter("@weekNumber", weekNumber), 
                new SqlParameter("@teacherId", teacherId ?? 0),
                new SqlParameter("@maxDiff", maxDiff)
            };

            var query = @"
                WITH WeekLessons AS (
                  SELECT j.JobId, ls.GroupId, e.PersonId, ls.DayNumber, ls.ClassNumber, 
                    ROW_NUMBER() OVER(PARTITION BY e.PersonId, ls.DayNumber ORDER BY e.PersonId, ls.DayNumber,ls.ClassNumber) AS Drn,
                    ROW_NUMBER() OVER(ORDER BY e.PersonId, ls.DayNumber, ls.ClassNumber) AS Crn
                  FROM Lesson ls
                  LEFT JOIN Job j ON ls.JobId = j.JobId
                  LEFT JOIN Employee e ON j.EmployeeId = e.EmployeeId
                  WHERE e.PersonId IN (
                    SELECT DISTINCT e2.PersonId
                    FROM Lesson tls
                    LEFT JOIN Job j2 ON tls.JobId = j2.JobId
                    LEFT JOIN Employee e2 ON j2.EmployeeId = e2.EmployeeId
                    WHERE tls.WeekNumber = @weekNumber
                      AND tls.DeletedAt IS NULL
                      AND tls.GroupId IN (233,309,500)
                  )
                  AND ls.JobId = CASE WHEN @teacherId <> 0 THEN @teacherId ELSE ls.JobId END
                  AND ls.WeekNumber = @weekNumber
                  AND ls.DeletedAt IS NULL
                )
                SELECT 
                  w2.PersonId, w2.JobId, w2.GroupId, 
                  w2.DayNumber, w2.ClassNumber, t0.ClassDiff 
                FROM WeekLessons w2
                LEFT JOIN (
                  SELECT w.PersonId, w.JobId, w.GroupId, w.DayNumber, w.ClassNumber, /*prev.*,*/ 
                    w.ClassNumber - prev.ClassNumber - 1 AS ClassDiff
                  FROM WeekLessons w
                  LEFT JOIN WeekLessons prev ON prev.PersonId = w.PersonId AND prev.Drn = w.Drn - 1 AND prev.Crn = w.Crn - 1
                  WHERE w.ClassNumber - prev.ClassNumber - 1 >= @maxDiff
                ) AS t0 ON w2.PersonId = t0.PersonId AND w2.DayNumber = t0.DayNumber 
                  AND (w2.ClassNumber = t0.ClassNumber OR w2.ClassNumber = t0.ClassNumber - t0.ClassDiff - 1)
                WHERE t0.PersonId IS NOT NULL
                ORDER BY w2.PersonId, w2.DayNumber, w2.ClassNumber;";
            var downtimes = _context.Database.SqlQuery<TeacherDowntimeQueryResult>(query, parameters).ToList();

            return downtimes;
        }

        /// <summary>
        /// Окна между занятиями у преподавателей на несколько недель
        /// Преподаватели выбираются независимо от редактируемой пользователем недели и групп
        /// </summary>
        /// <param name="weeks">Номера недель</param>
        /// <param name="teacherId">Идентификатор преподавателя (JobId)</param>
        /// <param name="maxDiff">Размер окна (количество занятий)</param>
        public List<TeacherDowntimeQueryResult> TeachersDowntime(int[] weeks, int? teacherId = 0, int maxDiff = 1)
        {
            var weeksStr = String.Join(",", weeks);
            var parameters = new object[]
            {
                // new SqlParameter("@weekNumber", weekNumber), 
                new SqlParameter("@teacherId", teacherId ?? 0),
                new SqlParameter("@maxDiff", maxDiff)
            };

            var query = String.Format(@"
                WITH WeekLessons AS (
                  SELECT j.JobId, ls.GroupId, e.PersonId, ls.WeekNumber, ls.DayNumber, ls.ClassNumber, 
                    ROW_NUMBER() OVER(PARTITION BY e.PersonId, ls.WeekNumber, ls.DayNumber ORDER BY e.PersonId, ls.DayNumber, ls.ClassNumber) AS Drn,
                    ROW_NUMBER() OVER(PARTITION BY ls.WeekNumber ORDER BY e.PersonId, ls.WeekNumber, ls.DayNumber, ls.ClassNumber) AS Crn
                  FROM Lesson ls
                  LEFT JOIN Job j ON ls.JobId = j.JobId
                  LEFT JOIN Employee e ON j.EmployeeId = e.EmployeeId
                  WHERE ls.JobId = CASE WHEN @teacherId <> 0 THEN @teacherId ELSE ls.JobId END
                    AND ls.WeekNumber IN ({0})
                    AND ls.DeletedAt IS NULL
                )

                SELECT 
                  w2.PersonId, w2.JobId, w2.GroupId, w2.WeekNumber,
                  w2.DayNumber, w2.ClassNumber, t0.ClassDiff 
                FROM WeekLessons w2
                LEFT JOIN (
                  SELECT w.PersonId, w.JobId, w.GroupId, w.WeekNumber, w.DayNumber, w.ClassNumber, /*prev.*,*/ 
                    w.ClassNumber - prev.ClassNumber - 1 AS ClassDiff
                  FROM WeekLessons w
                  LEFT JOIN WeekLessons prev ON prev.PersonId = w.PersonId AND prev.WeekNumber = w.WeekNumber AND prev.Drn = w.Drn - 1 AND prev.Crn = w.Crn - 1
                  WHERE w.ClassNumber - prev.ClassNumber - 1 >= @maxDiff
                ) AS t0 ON w2.PersonId = t0.PersonId AND w2.WeekNumber = t0.WeekNumber AND w2.DayNumber = t0.DayNumber 
                  AND (w2.ClassNumber = t0.ClassNumber OR w2.ClassNumber = t0.ClassNumber - t0.ClassDiff - 1)
                WHERE t0.PersonId IS NOT NULL
                ORDER BY w2.PersonId, w2.DayNumber, w2.ClassNumber;", weeksStr);
            var downtimes = _context.Database.SqlQuery<TeacherDowntimeQueryResult>(query, parameters).ToList();

            return downtimes;
        }
    }
}
