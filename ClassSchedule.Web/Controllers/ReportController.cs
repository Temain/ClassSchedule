using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ClassSchedule.Domain.DataAccess.Interfaces;
using ClassSchedule.Web.Models.Report;

namespace ClassSchedule.Web.Controllers
{
    public class ReportController : BaseController
    {
        public ReportController(IUnitOfWork unitOfWork)
            : base(unitOfWork)
        {
        }

        [HttpGet]
        public ActionResult FillingPercentage()
        {
            var viewModel = new FillingPercentageViewModel();

            var query = @"
                DECLARE @maxDiff AS INT = 2;
                DECLARE @groupid AS INT = 0;

                -- Последние загруженные учебные планы для каждой программы обучения
                -- на просматриваемый учебный год
                WITH UploadedPlans AS (
                    SELECT 
                    ap.ProgramOfEducationId, 
                    ap.AcademicPlanId
                    FROM (
                    SELECT 
                        ap.ProgramOfEducationId, 
                        ap.AcademicPlanId, 
                        ROW_NUMBER() OVER(PARTITION BY ap.ProgramOfEducationId ORDER BY ap.UploadedAt DESC) AS Rn
                    FROM [plan].AcademicPlan ap 
                    WHERE YEAR(ap.UploadedAt) <= @viewedYear + 1
                    ) AS t0
                    LEFT JOIN [plan].AcademicPlan ap ON ap.AcademicPlanId = t0.AcademicPlanId
                    WHERE t0.Rn = 1
                ),

                -- План для каждой группы на каждый семестр по каждой дисциплине
                GroupSemesterPlan AS (
                    SELECT 
                    c.FacultyId, 
                    g.GroupId, 
                    upl.*, 
                    cs.CourseNumber, 
                    ss.SemesterNumber, 
                    dsp.DisciplineId, 
                    dsp.HoursOfLectures AS PlanHoursOfLectures,
                    dsp.HoursOfPractice AS PlanHoursOfPractice, 
                    dsp.HoursOfLaboratory AS PlanHoursOfLaboratory,
                    ss.NumberOfFirstWeek, ss.NumberOfFirstWeek + LEN(ss.Schedule) - 1 AS NumberOfLastWeek
                    FROM dict.[Group] g
                    LEFT JOIN Course c ON g.CourseId = c.CourseId
                    LEFT JOIN UploadedPlans AS upl ON upl.ProgramOfEducationId = g.ProgramOfEducationId
                    LEFT JOIN [plan].CourseSchedule cs ON cs.AcademicPlanId = upl.AcademicPlanId AND cs.CourseNumber = c.CourseNumber
                    LEFT JOIN [plan].SemesterSchedule ss ON cs.CourseScheduleId = ss.CourseScheduleId
                    LEFT JOIN [plan].DisciplineSemesterPlan dsp ON ss.SemesterScheduleId = dsp.SemesterScheduleId
                    WHERE g.IsDeleted = 0 OR g.IsDeleted IS NULL
                    AND c.YearStart <= @viewedYear
                    AND g.GroupId = CASE WHEN @groupId <> 0 THEN @groupId ELSE g.GroupId END
                ),

                -- Процент заполнения расписания для каждой группы
                GroupFilledPercent AS (
                    SELECT 
                    t4.FacultyId,
                    t4.GroupId, 
                    t4.ProgramOfEducationId, 
                    t4.AcademicPlanId, 
                    t4.CourseNumber, 
                    t4.SemesterNumber,
                    CAST(SUM(t4.FilledPercent) / COUNT(*) AS NUMERIC(5,2)) AS FilledPercent
                    FROM (
                    SELECT 
                        t3.FacultyId, 
                        t3.GroupId, 
                        t3.ProgramOfEducationId,
                        t3.AcademicPlanId,
                        t3.CourseNumber,
                        t3.SemesterNumber,
                        t3.FilledPercent/t3.FilledDivider AS FilledPercent
                    FROM (
                        SELECT 
                        t2.FacultyId, 
                        t2.GroupId, 
                        t2.ProgramOfEducationId, 
                        t2.AcademicPlanId,
                        t2.CourseNumber, 
                        t2.SemesterNumber, 
                        t2.DisciplineId,  
                        (CASE WHEN t2.PlanHoursOfLectures > 0 THEN 1 ELSE 0 END + CASE WHEN t2.PlanHoursOfPractice > 0 THEN 1 ELSE 0 END + CASE WHEN t2.PlanHoursOfLaboratory > 0 THEN 1 ELSE 0 END) AS FilledDivider,
                        (CASE WHEN t2.PlanHoursOfLectures = 0 THEN 0 ELSE ROUND(t2.FilledHoursOfLectures * 100.00/t2.PlanHoursOfLectures, 2) END
                            + CASE WHEN t2.PlanHoursOfPractice = 0 THEN 0 ELSE ROUND(t2.FilledHoursOfPractice * 100.00/t2.PlanHoursOfPractice, 2) END
                            + CASE WHEN t2.PlanHoursOfLaboratory = 0 THEN 0 ELSE ROUND(t2.FilledHoursOfLaboratory * 100.00/t2.PlanHoursOfLaboratory, 2) END) AS FilledPercent
                        FROM (
                        SELECT DISTINCT t1.*,
                            SUM(CASE WHEN lsf.LessonTypeId = 1 THEN 1 ELSE 0 END) OVER(PARTITION BY t1.GroupId,t1.CourseNumber, t1.SemesterNumber, t1.DisciplineId) * 2 AS FilledHoursOfLectures,
                            SUM(CASE WHEN lsf.LessonTypeId = 2 OR lsf.LessonTypeId = 4 OR lsf.LessonTypeId = 5 THEN 1 ELSE 0 END) OVER(PARTITION BY t1.GroupId,t1.CourseNumber, t1.SemesterNumber, t1.DisciplineId) * 2 AS FilledHoursOfPractice,
                            SUM(CASE WHEN lsf.LessonTypeId = 3 THEN 1 ELSE 0 END) OVER(PARTITION BY t1.GroupId,t1.CourseNumber, t1.SemesterNumber, t1.DisciplineId) * 2 AS FilledHoursOfLaboratory
                        FROM GroupSemesterPlan AS t1
                        LEFT JOIN (
                            -- Занятия по каждой дисциплине
                            SELECT DISTINCT
                            ls.GroupId,
                            ls.LessonTypeId, 
                            ls.WeekNumber, 
                            ls.DayNumber, 
                            ls.ClassNumber,
                            ls.DisciplineId
                            FROM Lesson ls
                            LEFT JOIN dict.[Group] g ON ls.GroupId = g.GroupId
                            WHERE ls.DeletedAt IS NULL
                            AND ls.EducationYearId = @educationYear
                            AND g.GroupId = CASE WHEN @groupId <> 0 THEN @groupId ELSE g.GroupId END
                        ) AS lsf ON lsf.GroupId = t1.GroupId AND lsf.DisciplineId = t1.DisciplineId AND (lsf.WeekNumber >= t1.NumberOfFirstWeek AND lsf.WeekNumber <= t1.NumberOfLastWeek)
                        ) AS t2
                    ) AS t3
                    WHERE t3.FilledDivider <> 0 -- Проверить загрузку учебных планов, не могут быть одни нули
                    ) AS t4
                    GROUP BY 
                    t4.FacultyId,
                    t4.GroupId,
                    t4.ProgramOfEducationId,
                    t4.AcademicPlanId, 
                    t4.CourseNumber, 
                    t4.SemesterNumber
                ),

                -- Количество уч. планов, которое должно быть загружено по каждому факультету
                MustBeUploaded AS (
                    SELECT t1.FacultyId, f.DivisionName AS FacultyName, COUNT(t1.ProgramOfEducationId) AS MustBeUploaded, SUM(t1.AcademicPlanUploaded) AS Uploaded
                    FROM (
                    SELECT t0.*, CASE WHEN upl.ProgramOfEducationId IS NOT NULL THEN 1 ELSE 0 END AS AcademicPlanUploaded
                    FROM (
                        SELECT DISTINCT c.FacultyId, g.ProgramOfEducationId
                        FROM dict.[Group] g
                        LEFT JOIN ProgramOfEducation poe ON g.ProgramOfEducationId = poe.ProgramOfEducationId
                        LEFT JOIN Course c ON g.CourseId = c.CourseId
                        WHERE g.IsDeleted = 0 OR g.IsDeleted IS NULL
                        AND c.YearStart <= @viewedYear
                    ) AS t0
                    LEFT JOIN (
                        SELECT DISTINCT ap.ProgramOfEducationId
                        FROM [plan].AcademicPlan ap 
                    ) AS upl ON upl.ProgramOfEducationId = t0.ProgramOfEducationId
                    ) AS t1
                    LEFT JOIN dict.Faculty f ON f.FacultyId = t1.FacultyId
                    GROUP BY t1.FacultyId, f.DivisionName
                ),

                -- Эффективность расписания

                -- Все занятия
                WeekLessons AS (
                    SELECT 
                    ls.GroupId, 
                    ls.WeekNumber, 
                    ls.DayNumber, 
                    ls.ClassNumber, 
                    ROW_NUMBER() OVER(PARTITION BY ls.GroupId, ls.WeekNumber, ls.DayNumber ORDER BY ls.GroupId, ls.DayNumber, ls.ClassNumber) AS Drn,
                    ROW_NUMBER() OVER(PARTITION BY ls.WeekNumber ORDER BY ls.GroupId, ls.WeekNumber, ls.DayNumber, ls.ClassNumber) AS Crn
                    FROM Lesson ls
                    WHERE ls.DeletedAt IS NULL
                ),

                -- Количество окон у каждой группы на каждую неделю
                WindowsCount AS (
                    SELECT 
                    t1.GroupId,
                    t1.WeekNumber,
                    CAST((1.00 / COUNT(*)) AS NUMERIC(5,2)) AS Efficiency
                    FROM (
                    SELECT DISTINCT
                        w2.GroupId, 
                        w2.WeekNumber,
                        w2.DayNumber,
                        w2.ClassNumber,
                        t0.ClassDiff 
                    FROM WeekLessons w2
                    LEFT JOIN (
                        SELECT w.GroupId, 
                        w.WeekNumber,
                        w.DayNumber,
                        w.ClassNumber, /*prev.*,*/ 
                        w.ClassNumber - prev.ClassNumber - 1 AS ClassDiff
                        FROM WeekLessons w
                        LEFT JOIN WeekLessons prev ON prev.GroupId = w.GroupId AND prev.WeekNumber = w.WeekNumber AND prev.Drn = w.Drn - 1 AND prev.Crn = w.Crn - 1
                        WHERE w.ClassNumber - prev.ClassNumber - 1 >= @maxDiff
                    ) AS t0 ON w2.GroupId = t0.GroupId AND w2.WeekNumber = t0.WeekNumber AND w2.DayNumber = t0.DayNumber 
                        AND w2.ClassNumber = t0.ClassNumber - t0.ClassDiff - 1
                    WHERE t0.GroupId IS NOT NULL
                    ) AS t1
                    GROUP BY 
                    t1.GroupId, 
                    t1.WeekNumber
                ),

                -- Эффективность расписания групп на каждый семестр
                GroupEfficiency AS (
                    SELECT 
                    t3.FacultyId, 
                    t3.GroupId, 
                    t3.ProgramOfEducationId,
                    t3.AcademicPlanId,
                    t3.CourseNumber, 
                    t3.SemesterNumber, 
                    t3.WeeksCount,
                    COALESCE(CAST((t3.WeeksCount - COUNT(*) + SUM(t3.Efficiency)) / t3.WeeksCount AS NUMERIC(5,2)), 1) AS Efficiency
                    FROM (
                    SELECT 
                        apg.FacultyId,
                        apg.GroupId, 
                        apg.ProgramOfEducationId, 
                        apg.AcademicPlanId, 
                        apg.CourseNumber, 
                        apg.SemesterNumber,
                        apg.NumberOfLastWeek - apg.NumberOfFirstWeek + 1 AS WeeksCount, 
                        t2.WeekNumber, 
                        t2.Efficiency
                    FROM WindowsCount AS t2
                    RIGHT JOIN (
                        SELECT DISTINCT FacultyId, GroupId, ProgramOfEducationId, AcademicPlanId, CourseNumber, SemesterNumber, NumberOfFirstWeek, NumberOfLastWeek
                        FROM GroupSemesterPlan
                    ) AS apg ON apg.GroupId = t2.GroupId AND (t2.WeekNumber >= apg.NumberOfFirstWeek AND t2.WeekNumber <= apg.NumberOfLastWeek)
                    ) AS t3
                    WHERE t3.AcademicPlanId IS NOT NULL
                    GROUP BY 
                    t3.FacultyId,
                    t3.GroupId, 
                    t3.ProgramOfEducationId, 
                    t3.AcademicPlanId,
                    t3.CourseNumber, 
                    t3.SemesterNumber, 
                    t3.WeeksCount
                )

                -- Основной запрос
                -------------------------
                SELECT 
                    apu.*,
                    COALESCE(pr.SemesterNumber, 0) AS SemesterNumber, 
                    COALESCE(pr.FilledPercent, 0) AS FilledPercent,
                    ef.Efficiency
                FROM MustBeUploaded AS apu
                -- Процент заполнения расписания для каждого факультета
                LEFT JOIN (
                    SELECT 
                    t6.FacultyId, 
                    t6.SemesterNumber, 
                    CAST(SUM(t6.FilledPercent) / COUNT(*) AS NUMERIC(5,2)) AS FilledPercent
                    FROM (
                    SELECT 
                        t5.FacultyId,
                        t5.CourseNumber,
                        t5.SemesterNumber,
                        CAST(SUM(t5.FilledPercent) / COUNT(*) AS NUMERIC(5,2)) AS FilledPercent
                    FROM GroupFilledPercent AS t5
                    GROUP BY 
                        t5.FacultyId,
                        t5.ProgramOfEducationId, 
                        t5.AcademicPlanId, 
                        t5.CourseNumber, 
                        t5.SemesterNumber
                    ) AS t6
                    GROUP BY 
                    t6.FacultyId, 
                    t6.SemesterNumber
                ) AS pr ON pr.FacultyId = apu.FacultyId
                -- Эффективность расписания для каждого факультета
                LEFT JOIN (                    
                    SELECT 
                    fe.FacultyId, 
                    fe.SemesterNumber,
                    CAST(SUM(fe.Efficiency) / COUNT(*) AS NUMERIC(5,2)) AS Efficiency
                    FROM (
                    SELECT 
                        ge.FacultyId, 
                        ge.CourseNumber, 
                        ge.SemesterNumber,
                        CAST(SUM(COALESCE(ge.Efficiency,0)) / COUNT(*) AS NUMERIC(5,2)) AS Efficiency
                    FROM GroupEfficiency AS ge
                    GROUP BY 
                        ge.FacultyId,
                        ge.CourseNumber, 
                        ge.SemesterNumber
                    ) AS fe
                    GROUP BY 
                    fe.FacultyId,
                    fe.SemesterNumber
                ) AS ef ON ef.FacultyId = pr.FacultyId AND ef.SemesterNumber = pr.SemesterNumber
            ";

            object[] parameters = {
                new SqlParameter("@educationYear", UserProfile.EducationYearId),
                new SqlParameter("@viewedYear", UserProfile.EducationYear.YearStart)
            };

            var facultyPercentage = UnitOfWork.Execute<FacultyPercentageViewModel>(query, parameters).ToList();
            viewModel.Faculties = facultyPercentage;

            // viewModel.TotalFilledPercent = facultyPercentage.

            return View(viewModel);
        }
    }
}