﻿using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Data.Entity;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using ClassSchedule.Domain.Context;
using FastReport;
using ClassSchedule.Business.Interfaces;

namespace ClassSchedule.Web.Controllers
{
    public class PrintController : BaseController
    {
        private readonly ApplicationDbContext _context;
        private readonly IGroupService _groupService;

        public PrintController(ApplicationDbContext context, IGroupService groupService)
        {
            _context = context;
            _groupService = groupService;
        }

        [HttpGet]
        public ActionResult GroupSetSchedule()
        {
            const int maxGroupCount = 6;

            var groups = _groupService.GetEditableGroups(UserProfile.Id).ToList();

            if (!groups.Any())
            {
                throw new HttpException(422, "Список групп пуст.");
            }

            var report = new Report();
            if (groups.Count() > 1)
            {
                report.Load(Server.MapPath(@"~\App_Data\Reports\GroupSetSchedule.frx"));
            }
            else
            {
                report.Load(Server.MapPath(@"~\App_Data\Reports\GroupSchedule.frx"));
            }

            // Подзапросы
            var query = @"
                WITH Numbers AS (
                    SELECT 1 AS Number
                    UNION ALL
                    SELECT Number + 1
                    FROM Numbers
                    WHERE Number < 6
                ),
                ScheduleDays AS (
                  SELECT d.Number AS DayNumber, c.Number AS ClassNumber
                  FROM Numbers d
                  CROSS JOIN Numbers c
                ),";

            for (int i = 1; i <= groups.Count(); i++)
            {
                var groupLessons = String.Format(@"
                    Group{0}Lessons AS (
                      SELECT s.DayNumber, s.ClassNumber, 
                        l.LessonTypeId, l.DisciplineId, 
                        ld.PlannedChairJobId, ld.AuditoriumId
                      FROM LessonDetail ld 
                      LEFT JOIN Lesson l ON ld.LessonId = l.LessonId
                      LEFT JOIN Schedule s ON l.ScheduleId = s.ScheduleId
                      WHERE s.WeekNumber = @weekNumber
                        AND s.GroupId={1}
                        AND l.DeletedAt IS NULL 
                        AND s.DeletedAt IS NULL
                        AND ld.DeletedAt IS NULL
                    )", i, groups[i - 1].GroupId);
                query += groupLessons + (i != groups.Count() ? ", " : "");
            }

            // Сам запрос
            // Запрос рассчитан на печать расписания максимум для 6 групп
            query += "\r\nSELECT sch.DayNumber AS den, CONVERT(VARCHAR(5), ct.StartTime, 108) + ' - ' + CONVERT(VARCHAR(5), ct.EndTime, 108) AS num,";
            for (int i = 1; i <= maxGroupCount; i++)
            {
                if (i <= groups.Count())
                {
                    query += String.Format("gsch{0}.Group{0}Lesson AS rasp{0}", i);
                }
                else
                {
                    query += String.Format("'' AS rasp{0}", i);
                }

                query += (i < maxGroupCount ? ", " : "");
            }
            query += "\r\nFROM ScheduleDays sch";

            for (int i = 1; i <= groups.Count(); i++)
            {
                var groupSchedule = String.Format(@"
                    LEFT JOIN (
                      SELECT DISTINCT ls.DayNumber, ls.ClassNumber,
                        STUFF((SELECT DISTINCT ',' + CHAR(10) + dn.Name + CASE WHEN tmp1.LessonTypeId = 1 THEN ' (лек.)' ELSE '' END + ' - ' +
                            STUFF((SELECT DISTINCT ', ' 
                                + COALESCE(CASE WHEN pcj.JobId IS NULL THEN pcj.PlannedChairJobComment ELSE (p.LastName + COALESCE(' ' + LEFT(p.FirstName, 1) + '.', '') + COALESCE(' ' + LEFT(p.MiddleName, 1) + '.', '')) END, '')
                                + ' (' + CASE WHEN a.AuditoriumNumber = h.Abbreviation THEN h.Abbreviation ELSE a.AuditoriumNumber + h.Abbreviation END + ')'
                              FROM Group{0}Lessons tmp2 
                              LEFT JOIN PlannedChairJob pcj ON pcj.PlannedChairJobId = tmp2.PlannedChairJobId
                              LEFT JOIN Job j ON pcj.JobId = j.JobId
                              LEFT JOIN Employee e ON e.EmployeeId = j.EmployeeId
                              LEFT JOIN Person p ON p.PersonId = e.PersonId 
                              LEFT JOIN Auditorium a ON a.AuditoriumId = tmp2.AuditoriumId
                              LEFT JOIN dict.Housing h ON h.HousingId = a.HousingId
                              WHERE tmp2.DayNumber = tmp1.DayNumber AND tmp2.ClassNumber = tmp1.ClassNumber AND tmp2.DisciplineId = tmp1.DisciplineId
                              FOR XML PATH('')), 1, 2, '') 
                          FROM Group{0}Lessons tmp1
                          LEFT JOIN dbo.Discipline d ON tmp1.DisciplineId = d.DisciplineId
                          LEFT JOIN dict.DisciplineName dn ON d.DisciplineNameId = dn.DisciplineNameId
                          WHERE tmp1.DayNumber = ls.DayNumber AND tmp1.ClassNumber = ls.ClassNumber
                          FOR XML PATH('')), 1, 2, '') AS Group{0}Lesson
                      FROM Group{0}Lessons ls
                      GROUP BY ls.DayNumber, ls.ClassNumber, ls.DisciplineId, ls.PlannedChairJobId
                    ) AS gsch{0} ON sch.DayNumber = gsch{0}.DayNumber AND sch.ClassNumber = gsch{0}.ClassNumber
                    ", i);
                query += groupSchedule;
            }
            query += @"
                    LEFT JOIN dict.ClassTime ct ON sch.DayNumber = ct.DayNumber AND sch.ClassNumber = ct.ClassNumber
                    ORDER BY sch.DayNumber, sch.ClassNumber";

            var table = new DataTable();
            var connectionString = WebConfigurationManager.ConnectionStrings["ClassScheduleConnection"].ConnectionString;
            using (var connection = new SqlConnection(connectionString))
            using (var command = new SqlCommand(query, connection))
            {
                if (connection.State != ConnectionState.Open)
                    connection.Open();
                var parameters = new[] { new SqlParameter("@weekNumber", UserProfile.WeekNumber) };
                command.Parameters.AddRange(parameters);
                var da = new SqlDataAdapter(command);
                da.Fill(table);
                connection.Close();
            }

            report.RegisterData(table, "Potok");
            var bandDataTable = report.FindObject("Data2") as DataBand;
            if (bandDataTable != null) bandDataTable.DataSource = report.Dictionary.DataSources.FindByName("Potok");

            var someGroup = groups.FirstOrDefault();
            if (someGroup != null)
            {
                report.SetParameterValue("KURS", someGroup.Course.CourseNumber);
                report.SetParameterValue("FAK", someGroup.Course.Faculty.DivisionName.ToUpper());
            }
            report.SetParameterValue("NEDEL", UserProfile.WeekNumber % 2 == 0 ? 2 : 1);

            for (int i = 1; i <= maxGroupCount; i++)
            {
                report.SetParameterValue("GR" + i, i <= groups.Count() ? groups[i - 1].GroupName : "");
            }

            report.SetParameterValue("GRC", groups.Count());

            // Изменение размера для каждой страницы
            foreach (PageBase page in report.Pages)
            {
                if (page is ReportPage)
                {
                    (page as ReportPage).PaperWidth = 420;
                    (page as ReportPage).PaperHeight = 297;
                }
            }

            report.Prepare();

            var exportedReport = ExportReport(report, (int)ExportTypes.Pdf, fileName: "GroupSetSchedule");

            return exportedReport;
        }

        [HttpGet]
        public ActionResult ChairSchedule()
        {
            return View();
        }

        protected enum ExportTypes
        {
            Pdf,
            Excel,
            Word
        }

        private FileStreamResult ExportReport(Report report, int exportType, string fileName)
        {
            string mimeType = "application/pdf";
            string extension = "pdf";
            dynamic frExportType = new FastReport.Export.Pdf.PDFExport();
            switch ((ExportTypes)exportType)
            {
                case ExportTypes.Excel:
                    {
                        mimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                        frExportType = new FastReport.Export.OoXML.Excel2007Export();
                        extension = "xlsx";
                        break;
                    }
                case ExportTypes.Word:
                    {
                        mimeType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
                        frExportType = new FastReport.Export.OoXML.Word2007Export();
                        extension = "docx";
                        break;
                    }
            }

            var msx = new MemoryStream();
            report.Export(frExportType, msx);
            msx.Position = 0;
            HttpContext.Response.AddHeader("content-disposition", "inline; filename=\"" + fileName + "." + extension + "\"");

            return new FileStreamResult(msx, mimeType);
        }
    }
}