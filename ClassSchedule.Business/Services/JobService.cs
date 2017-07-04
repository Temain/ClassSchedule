using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data.Entity;
using System.Linq;
using ClassSchedule.Business.Interfaces;
using ClassSchedule.Business.Models;
using ClassSchedule.Domain.Context;
using ClassSchedule.Domain.Models;
using ClassSchedule.Domain.Models.QueryResults;

namespace ClassSchedule.Business.Services
{
    public class JobService : IJobService
    {
        private readonly ApplicationDbContext _context;

        public JobService(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Преподаватели, работающие в определенном учебном году
        /// </summary>
        public List<TeacherViewModel> ActualTeachers(int educationYearId, int? chairId, string query = null, int? take = null)
        {
            var plannedChairJobs = _context.PlannedChairJobs
                .Include(x => x.Job.Employee.Person)
                .Include(x => x.Job.Position.PositionReal)
                .Include(x => x.Job.EmploymentType)
                .Where(x => x.EducationYearId == educationYearId && x.DeletedAt == null
                    && x.Job.DeletedAt == null && x.Job.Employee.DeletedAt == null & x.Job.Employee.Person.DeletedAt == null);

            if (chairId != null)
            {
                plannedChairJobs = plannedChairJobs.Where(x => x.ChairId == chairId);
            }

            if (!string.IsNullOrEmpty(query))
            {
                plannedChairJobs = plannedChairJobs.Where(x => x.Job.Employee.Person.LastName.ToLower().Contains(query.ToLower()));
            }

            if (take != null && take != 0)
            {
                plannedChairJobs = plannedChairJobs.Take(take ?? 0);
            }

            var teachers = plannedChairJobs
                .ToList()
                .Select(x => new TeacherViewModel
                {
                    PlannedChairJobId = x.PlannedChairJobId,
                    JobId = x.JobId ?? 0,
                    PersonId = x.Job != null ? x.Job.Employee.PersonId : 0,
                    TeacherFullName = x.Job != null ? x.Job.Employee.Person.FullName + " (" + x.Job.Position.PositionReal.PositionName + ", усл.: " + x.EmploymentType.EmploymentTypeName + ")" : x.PlannedChairJobComment,
                })
                .OrderBy(x => x.TeacherFullName)
                .ToList();

            return teachers;
        }

        /// <summary>
        /// Преподаватели, работающие в определенном учебном году
        /// со списком групп, у которых они ведут занятия на определённой паре
        /// Используется при редактировании занятия (выдача подсказки о занятости преподавателя)
        /// </summary>
        public List<TeacherViewModel> ActualTeachersWithEmployment(int educationYearId, int? chairId,
            int weekNumber, int dayNumber, int classNumber, int currentGroupId)
        {
            var plannedChairJobs = _context.PlannedChairJobs
                .Include(x => x.Job.Employee.Person)
                .Include(x => x.Job.EmploymentType)
                .Include(x => x.LessonDetails.Select(s => s.Lesson.Schedule.Group))
                .Where(x => x.EducationYearId == educationYearId && x.DeletedAt == null
                    && x.Job.DeletedAt == null && x.Job.Employee.DeletedAt == null & x.Job.Employee.Person.DeletedAt == null);

            if (chairId != null)
            {
                plannedChairJobs = plannedChairJobs.Where(x => x.ChairId == chairId);
            }

            var teachers = plannedChairJobs
                .ToList()
                .Select(x => new TeacherViewModel
                {
                    PlannedChairJobId = x.PlannedChairJobId,
                    JobId = x.JobId ?? 0,
                    PersonId = x.Job != null ? x.Job.Employee.PersonId : 0,
                    TeacherFullName = x.Job != null ? x.Job.Employee.Person.FullName : x.PlannedChairJobComment,
                    Employment = string.Join(",", x.LessonDetails
                        .Where(s => s.Lesson.Schedule.WeekNumber == weekNumber 
                            && s.Lesson.Schedule.DayNumber == dayNumber 
                            && s.Lesson.Schedule.ClassNumber == classNumber
                            && s.Lesson.Schedule.GroupId != currentGroupId
                            && s.DeletedAt == null && s.Lesson.DeletedAt == null && s.Lesson.Schedule.DeletedAt == null)
                        .Select(s => s.Lesson.Schedule.Group.GroupName))
                })
                .OrderBy(x => x.TeacherFullName)
                .ToList();

            return teachers;
        }

        /// <summary>
        /// Окна между занятиями у преподавателей на определённую неделю
        /// Преподаватели выбираются в соотвествии с редактируемой пользователем неделей и группами
        /// </summary>
        /// <param name="weekNumber">Номер недели</param>
        /// <param name="teacherId">Идентификатор преподавателя (JobId)</param>
        /// <param name="maxDiff">Размер окна (количество занятий)</param>
        public List<TeacherDowntimeQueryResult> TeachersDowntime(int weekNumber, int? chairJobId = 0, int maxDiff = 1)
        {
            var parameters = new object[]
            {
                new SqlParameter("@weekNumber", weekNumber), 
                new SqlParameter("@chairJobId", chairJobId ?? 0),
                new SqlParameter("@maxDiff", maxDiff)
            };

            var query = @"
                WITH WeekLessons AS (
                  SELECT ld.PlannedChairJobId, s.GroupId, e.PersonId, s.DayNumber, s.ClassNumber,
                    ROW_NUMBER() OVER(PARTITION BY CASE WHEN e.PersonId IS NULL THEN ld.PlannedChairJobId ELSE e.PersonId END, s.DayNumber ORDER BY CASE WHEN e.PersonId IS NULL THEN ld.PlannedChairJobId ELSE e.PersonId END, s.DayNumber, s.ClassNumber) AS Drn,
                    ROW_NUMBER() OVER(ORDER BY CASE WHEN e.PersonId IS NULL THEN ld.PlannedChairJobId ELSE e.PersonId END, s.DayNumber, s.ClassNumber) AS Crn
                  FROM LessonDetail ld
                  LEFT JOIN Lesson l ON ld.LessonId = l.LessonId
                  LEFT JOIN Schedule s ON l.ScheduleId = s.ScheduleId
                  LEFT JOIN PlannedChairJob pcj ON ld.PlannedChairJobId = pcj.PlannedChairJobId
                  LEFT JOIN Job j ON pcj.JobId = j.JobId
                  LEFT JOIN Employee e ON j.EmployeeId = e.EmployeeId
                  WHERE (e.PersonId IN (
                    SELECT DISTINCT e1.PersonId
                    FROM LessonDetail tls
                    LEFT JOIN Lesson l1 ON tls.LessonId = l1.LessonId
                    LEFT JOIN Schedule s1 ON l1.ScheduleId = s1.ScheduleId
                    LEFT JOIN PlannedChairJob pcj1 ON tls.PlannedChairJobId = pcj1.PlannedChairJobId
                    LEFT JOIN Job j1 ON pcj1.JobId = j1.JobId
                    LEFT JOIN Employee e1 ON j1.EmployeeId = e1.EmployeeId
                    WHERE s1.WeekNumber = @weekNumber
                      --AND s1.GroupId IN (233,309,500)
                      AND l1.DeletedAt IS NULL AND s1.DeletedAt IS NULL
                      AND tls.DeletedAt IS NULL
                  )
                  OR ld.PlannedChairJobId IN (
                    SELECT DISTINCT pcj2.PlannedChairJobId
                    FROM LessonDetail tls
                    LEFT JOIN Lesson l1 ON tls.LessonId = l1.LessonId
                    LEFT JOIN Schedule s1 ON l1.ScheduleId = s1.ScheduleId
                    LEFT JOIN PlannedChairJob pcj2 ON tls.PlannedChairJobId = pcj2.PlannedChairJobId
                    WHERE s1.WeekNumber = @weekNumber
                      --AND s1.GroupId IN (233,309,500)
                      AND l1.DeletedAt IS NULL AND s1.DeletedAt IS NULL
                      AND tls.DeletedAt IS NULL
                  ))
                  AND ld.PlannedChairJobId = CASE WHEN @chairJobId <> 0 THEN @chairJobId ELSE ld.PlannedChairJobId END
                  AND s.WeekNumber = @weekNumber
                  AND s.DeletedAt IS NULL AND ld.DeletedAt IS NULL AND l.DeletedAt IS NULL
                )

                SELECT 
                  COALESCE(w2.PersonId, 0) AS PersonId, w2.PlannedChairJobId, w2.GroupId, 
                  w2.DayNumber, w2.ClassNumber, t0.ClassDiff 
                FROM WeekLessons w2
                LEFT JOIN (
                  SELECT w.PersonId, w.PlannedChairJobId, w.GroupId, w.DayNumber, w.ClassNumber, /*prev.*,*/ 
                    w.ClassNumber - prev.ClassNumber - 1 AS ClassDiff
                  FROM WeekLessons w
                  LEFT JOIN WeekLessons prev ON (prev.PersonId = w.PersonId OR prev.PlannedChairJobId = w.PlannedChairJobId) AND prev.Drn = w.Drn - 1 AND prev.Crn = w.Crn - 1
                  WHERE w.ClassNumber - prev.ClassNumber - 1 >= @maxDiff
                ) AS t0 ON (w2.PersonId = t0.PersonId OR w2.PlannedChairJobId = t0.PlannedChairJobId) AND w2.DayNumber = t0.DayNumber 
                  AND (w2.ClassNumber = t0.ClassNumber OR w2.ClassNumber = t0.ClassNumber - t0.ClassDiff - 1)
                WHERE t0.ClassDiff IS NOT NULL
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
        public List<TeacherDowntimeQueryResult> TeachersDowntime(int[] weeks, int? chairJobId = 0, int maxDiff = 1)
        {
            var weeksStr = String.Join(",", weeks);
            var parameters = new object[]
            {
                // new SqlParameter("@weekNumber", weekNumber), 
                new SqlParameter("@chairJobId", chairJobId ?? 0),
                new SqlParameter("@maxDiff", maxDiff)
            };

            var query = String.Format(@"
                WITH WeekLessons AS (
                  SELECT ld.PlannedChairJobId, s.WeekNumber, s.GroupId, e.PersonId, s.DayNumber, s.ClassNumber,
                    ROW_NUMBER() OVER(PARTITION BY CASE WHEN e.PersonId IS NULL THEN ld.PlannedChairJobId ELSE e.PersonId END, s.WeekNumber, s.DayNumber ORDER BY CASE WHEN e.PersonId IS NULL THEN ld.PlannedChairJobId ELSE e.PersonId END, s.DayNumber, s.ClassNumber) AS Drn,
                    ROW_NUMBER() OVER(PARTITION BY s.WeekNumber ORDER BY CASE WHEN e.PersonId IS NULL THEN ld.PlannedChairJobId ELSE e.PersonId END, s.WeekNumber, s.DayNumber, s.ClassNumber) AS Crn
                  FROM LessonDetail ld
                  LEFT JOIN Lesson l ON ld.LessonId = l.LessonId
                  LEFT JOIN Schedule s ON l.ScheduleId = s.ScheduleId
                  LEFT JOIN PlannedChairJob pcj ON ld.PlannedChairJobId = pcj.PlannedChairJobId
                  LEFT JOIN Job j ON pcj.JobId = j.JobId
                  LEFT JOIN Employee e ON j.EmployeeId = e.EmployeeId
                  WHERE s.WeekNumber IN (1,2)
                    AND ld.PlannedChairJobId = CASE WHEN @chairJobId <> 0 THEN @chairJobId ELSE ld.PlannedChairJobId END
                    AND s.DeletedAt IS NULL AND ld.DeletedAt IS NULL AND l.DeletedAt IS NULL
                )

                SELECT 
                  COALESCE(w2.PersonId, 0) AS PersonId, w2.PlannedChairJobId, w2.GroupId, w2.WeekNumber,
                  w2.DayNumber, w2.ClassNumber, t0.ClassDiff 
                FROM WeekLessons w2
                LEFT JOIN (
                  SELECT w.PersonId, w.PlannedChairJobId, w.GroupId, w.WeekNumber, w.DayNumber, w.ClassNumber, /*prev.*,*/ 
                    w.ClassNumber - prev.ClassNumber - 1 AS ClassDiff
                  FROM WeekLessons w
                  LEFT JOIN WeekLessons prev ON (prev.PersonId = w.PersonId OR prev.PlannedChairJobId = w.PlannedChairJobId) AND prev.WeekNumber = w.WeekNumber AND prev.Drn = w.Drn - 1 AND prev.Crn = w.Crn - 1
                  WHERE w.ClassNumber - prev.ClassNumber - 1 >= @maxDiff
                ) AS t0 ON (w2.PersonId = t0.PersonId OR w2.PlannedChairJobId = t0.PlannedChairJobId) AND w2.WeekNumber = t0.WeekNumber AND w2.DayNumber = t0.DayNumber 
                  AND (w2.ClassNumber = t0.ClassNumber OR w2.ClassNumber = t0.ClassNumber - t0.ClassDiff - 1)
                WHERE t0.ClassDiff IS NOT NULL
                ORDER BY w2.PersonId, w2.DayNumber, w2.ClassNumber;", weeksStr);
            var downtimes = _context.Database.SqlQuery<TeacherDowntimeQueryResult>(query, parameters).ToList();

            return downtimes;
        }
    }
}
