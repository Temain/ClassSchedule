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
                DECLARE @groupId AS INT = 0;

                SELECT apu.*, COALESCE(pr.SemesterNumber, 0) AS SemesterNumber, COALESCE(pr.FilledPercent, 0) AS FilledPercent
                FROM (
                -- Количество уче. планов, которое должно быть загружено
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
                ) AS apu
                -- Процент заполнения расписания
                LEFT JOIN (
                  SELECT t6.FacultyId, t6.SemesterNumber, CAST(SUM(t6.FilledPercent) / COUNT(*) AS NUMERIC(5,2)) AS FilledPercent
                  FROM (
                    SELECT t5.FacultyId, t5.ProgramOfEducationId, t5.AcademicPlanId, t5.CourseNumber, t5.SemesterNumber, CAST(SUM(t5.FilledPercent) / COUNT(*) AS NUMERIC(5,2)) AS FilledPercent
                      FROM (
                      SELECT t4.FacultyId, t4.GroupId, t4.ProgramOfEducationId, t4.AcademicPlanId, t4.CourseNumber, t4.SemesterNumber, CAST(SUM(t4.FilledPercent) / COUNT(*) AS NUMERIC(5,2)) AS FilledPercent
                      FROM (
                        SELECT t3.FacultyId, t3.GroupId, t3.ProgramOfEducationId,t3.AcademicPlanId,t3.CourseNumber,t3.SemesterNumber,
                            t3.FilledPercent/t3.FilledDivider AS FilledPercent
                          FROM (
                            SELECT 
                              t2.FacultyId, t2.GroupId, t2.ProgramOfEducationId, t2.AcademicPlanId, t2.CourseNumber, t2.SemesterNumber, t2.DisciplineId,  
                              (CASE WHEN t2.PlanHoursOfLectures > 0 THEN 1 ELSE 0 END 
                                + CASE WHEN t2.PlanHoursOfPractice > 0 THEN 1 ELSE 0 END + CASE WHEN t2.PlanHoursOfLaboratory > 0 THEN 1 ELSE 0 END) AS FilledDivider,
                              (CASE WHEN t2.PlanHoursOfLectures = 0 THEN 0 ELSE ROUND(t2.FilledHoursOfLectures * 100.00/t2.PlanHoursOfLectures, 2) END
                                + CASE WHEN t2.PlanHoursOfPractice = 0 THEN 0 ELSE ROUND(t2.FilledHoursOfPractice * 100.00/t2.PlanHoursOfPractice, 2) END
                                + CASE WHEN t2.PlanHoursOfLaboratory = 0 THEN 0 ELSE ROUND(t2.FilledHoursOfLaboratory * 100.00/t2.PlanHoursOfLaboratory, 2) END) AS FilledPercent
                            FROM (
                            SELECT DISTINCT t1.*, --lsf.LessonTypeId
                              SUM(CASE WHEN lsf.LessonTypeId = 1 THEN 1 ELSE 0 END) OVER(PARTITION BY t1.GroupId,t1.CourseNumber, t1.SemesterNumber, t1.DisciplineId) * 2 AS FilledHoursOfLectures,
                              SUM(CASE WHEN lsf.LessonTypeId = 2 OR lsf.LessonTypeId = 4 OR lsf.LessonTypeId = 5 THEN 1 ELSE 0 END) OVER(PARTITION BY t1.GroupId,t1.CourseNumber, t1.SemesterNumber, t1.DisciplineId) * 2 AS FilledHoursOfPractice,
                              SUM(CASE WHEN lsf.LessonTypeId = 3 THEN 1 ELSE 0 END) OVER(PARTITION BY t1.GroupId,t1.CourseNumber, t1.SemesterNumber, t1.DisciplineId) * 2 AS FilledHoursOfLaboratory
                            FROM (
                              SELECT c.FacultyId, g.GroupId, upl.*, 
                                cs.CourseNumber, ss.SemesterNumber, dsp.DisciplineId, dsp.HoursOfLectures AS PlanHoursOfLectures,
                                dsp.HoursOfPractice AS PlanHoursOfPractice, dsp.HoursOfLaboratory AS PlanHoursOfLaboratory,
                                ss.NumberOfFirstWeek, ss.NumberOfFirstWeek + LEN(ss.Schedule) - 1 AS NumberOfLastWeek
                              FROM dict.[Group] g
                              LEFT JOIN Course c ON g.CourseId = c.CourseId
                              LEFT JOIN (
                                  -- Последние загруженные учебные планы для каждой программы обучения
                                  -- на просматриваемый учебный год
                                  SELECT ap.ProgramOfEducationId, ap.AcademicPlanId
                                    FROM (
                                      SELECT ap.ProgramOfEducationId, ap.AcademicPlanId, ROW_NUMBER() OVER(PARTITION BY ap.ProgramOfEducationId ORDER BY ap.UploadedAt DESC) AS Rn
                                      FROM [plan].AcademicPlan ap 
                                      WHERE YEAR(ap.UploadedAt) <= @viewedYear + 1
                                    ) AS t0
                                    LEFT JOIN [plan].AcademicPlan ap ON ap.AcademicPlanId = t0.AcademicPlanId
                                    WHERE t0.Rn = 1
              
                              ) AS upl ON upl.ProgramOfEducationId = g.ProgramOfEducationId
                              LEFT JOIN [plan].CourseSchedule cs ON cs.AcademicPlanId = upl.AcademicPlanId AND cs.CourseNumber = c.CourseNumber
                              LEFT JOIN [plan].SemesterSchedule ss ON cs.CourseScheduleId = ss.CourseScheduleId
                              LEFT JOIN [plan].DisciplineSemesterPlan dsp ON ss.SemesterScheduleId = dsp.SemesterScheduleId
                              WHERE g.IsDeleted = 0 OR g.IsDeleted IS NULL
                                AND c.YearStart <= @viewedYear
                                AND g.GroupId = CASE WHEN @groupId <> 0 THEN @groupId ELSE g.GroupId END
                            ) AS t1
                            LEFT JOIN (
                              -- Занятия по каждой дисциплине
                              SELECT DISTINCT ls.GroupId, ls.LessonTypeId, ls.WeekNumber, ls.DayNumber, ls.ClassNumber, ls.DisciplineId
                              FROM Lesson ls
                              LEFT JOIN dict.[Group] g ON ls.GroupId = g.GroupId
                              WHERE ls.DeletedAt IS NULL
                                AND ls.EducationYearId = @educationYear
                                AND g.GroupId = CASE WHEN @groupId <> 0 THEN @groupId ELSE g.GroupId END
                            ) AS lsf ON lsf.GroupId = t1.GroupId AND lsf.DisciplineId = t1.DisciplineId 
                              AND (lsf.WeekNumber >= t1.NumberOfFirstWeek AND lsf.WeekNumber <= t1.NumberOfLastWeek)
                          ) AS t2
                        ) AS t3
                        WHERE t3.FilledDivider <> 0 -- Проверить загрузку учебных планов, не могут быть одни нули
                      ) AS t4
                      GROUP BY t4.FacultyId, t4.GroupId, t4.ProgramOfEducationId, t4.AcademicPlanId, t4.CourseNumber, t4.SemesterNumber
                    ) AS t5
                    GROUP BY t5.FacultyId, t5.ProgramOfEducationId, t5.AcademicPlanId, t5.CourseNumber, t5.SemesterNumber
                  ) AS t6
                  GROUP BY t6.FacultyId, t6.SemesterNumber
                ) AS pr ON pr.FacultyId = apu.FacultyId
            ";

            object[] parameters = {
                new SqlParameter("@educationYear", UserProfile.EducationYearId),
                new SqlParameter("@viewedYear", UserProfile.EducationYear.YearStart)
            };

            var facultyPercentage = UnitOfWork.Execute<FacultyPercentageViewModel>(query, parameters).ToList();
            viewModel.Faculties = facultyPercentage;

            return View(viewModel);
        }
    }
}