using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.SessionState;
using ClassSchedule.Domain.DataAccess.Interfaces;
using ClassSchedule.Domain.DataAccess.Repositories;
using ClassSchedule.Domain.Helpers;
using ClassSchedule.Domain.Models;
using ClassSchedule.Web.Models.Schedule;

namespace ClassSchedule.Web.Controllers
{
    public class DictionaryController : BaseController
    {
        public DictionaryController(IUnitOfWork unitOfWork)
            : base(unitOfWork)
        {
        }

        public enum ResultType
        {
            Dictionary,
            Json
        }

        [HttpPost]
        public ActionResult Course(int facultyId)
        {
            if (Request.IsAjaxRequest())
            {
                var courses = UnitOfWork.Repository<Course>()
                    .GetQ(
                        filter: x => x.IsDeleted != true && x.FacultyId == facultyId && x.YearStart != null 
                            && x.YearStart + x.CourseNumber == UserProfile.EducationYear.YearEnd,
                        orderBy: o => o.OrderBy(n => n.CourseName))
                    .Select(x => new {x.CourseId, x.CourseName});

                return Json(courses);
            }

            return null;
        }

        [HttpPost]
        public ActionResult Group(int facultyId, int? courseId)
        {
            if (Request.IsAjaxRequest())
            {
                var groups = UnitOfWork.Repository<Group>()
                    .GetQ(x => x.IsDeleted != true && x.Course.FacultyId == facultyId,
                        orderBy: o => o.OrderBy(n => n.DivisionName));

                if (courseId != null)
                {
                    groups = groups.Where(x => x.CourseId == courseId);
                }

                var result = groups.Select(x => new {x.GroupId, GroupName = x.DivisionName});

                return Json(result);
            }

            return null;
        }

        [HttpPost]
        public ActionResult EducationForm()
        {
            if (Request.IsAjaxRequest())
            {
                var forms = UnitOfWork.Repository<EducationForm>()
                    .GetQ(x => x.IsDeleted != true, orderBy: o => o.OrderBy(n => n.EducationFormName))
                    .Select(x => new { x.EducationFormId, x.EducationFormName });

                return Json(forms);
            }

            return null;
        }

        [HttpPost]
        public ActionResult EducationLevel()
        {
            if (Request.IsAjaxRequest())
            {
                var levels = UnitOfWork.Repository<EducationLevel>()
                    .GetQ(x => x.IsDeleted != true, orderBy: o => o.OrderBy(n => n.EducationLevelName))
                    .Select(x => new { x.EducationLevelId, x.EducationLevelName });

                return Json(levels);
            }

            return null;
        }

        [HttpPost]
        public ActionResult AcademicPlanYear()
        {
            if (Request.IsAjaxRequest())
            {
                var years = UnitOfWork.Repository<Course>()
                .GetQ(x => x.IsDeleted != true)
                .Select(x => new
                {
                    Value = x.YearStart
                })
                .Distinct()
                .OrderBy(x => x.Value)
                .ToList();

                return Json(years);
            }

            return null;
        }

        [HttpPost]
        public ActionResult Direction()
        {
            if (Request.IsAjaxRequest())
            {
                var directions = UnitOfWork.Repository<EducationDirection>()
                    .GetQ(x => x.IsDeleted != true, orderBy: o => o.OrderBy(n => n.EducationDirectionCode))
                    .Select(
                        x =>
                            new
                            {
                                EducationDirection = x.EducationDirectionId,
                                EducationDirectionName = x.EducationDirectionCode + " " + x.EducationDirectionName
                            });

                return Json(directions);
            }

            return null;
        }

        [HttpPost]
        public ActionResult EducationProfile(int educationFormId, int educationDirectionId, int yearStart)
        {
            if (Request.IsAjaxRequest())
            {
                var profiles = UnitOfWork.Repository<ProgramOfEducation>()
                    .GetQ(x => x.IsDeleted != true && x.EducationFormId == educationFormId 
                        && x.EducationProfile.EducationDirectionId == educationDirectionId
                        && x.Groups.Any(g => g.Course.YearStart == yearStart))
                    .Select(x => new
                    {
                        ProgramOfEducationid = x.ProgramOfEducationId,
                        EducationProfileId = x.EducationProfileId,
                        EducationLevelId = x.EducationLevelId,
                        EducationProfileName =
                            x.EducationProfile.EducationDirection.EducationDirectionCode + " " +
                            x.EducationProfile.EducationProfileName + (x.EducationLevelId == 2 ? " (прикладной бакалавриат)" : "")
                    })
                    .OrderBy(n => n.EducationLevelId)
                    .ThenBy(x => x.EducationProfileName)
                    .ToList();

                return Json(profiles);
            }

            return null;
        }

        [HttpPost]
        public ActionResult Discipline(string query, int? chairId)
        {
            if (Request.IsAjaxRequest())
            {
                var disciplines = UnitOfWork.Repository<Discipline>()
                    .GetQ(x => x.DisciplineName.StartsWith(query))
                    .Take(20);

                if (chairId != null)
                {
                    disciplines = disciplines.Where(d => d.ChairId == chairId);
                }

                var result = disciplines
                    .Select(x => new
                    {
                        x.DisciplineId,
                        x.DisciplineName,
                        x.ChairId,
                        ChairName = x.Chair.DivisionName
                    })
                    .ToList();

                return Json(result);
            }

            return null;
        }

        [HttpPost]
        public ActionResult Teacher(int chairId)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();

            if (Request.IsAjaxRequest())
            {
                var jobRepository = UnitOfWork.Repository<Job>() as JobRepository;
                if (jobRepository != null)
                {
                    var chairTeachers = jobRepository.ActualTeachers(UserProfile.EducationYear, chairId);
                    var result = chairTeachers
                        .Select(
                            x =>
                                new 
                                {
                                    TeacherId = x.JobId,
                                    TeacherFullName = x.Employee.Person.FullName
                                })
                        .OrderBy(n => n.TeacherFullName)
                        .ToList();

                    watch.Stop();
                    var elapsedMs = watch.ElapsedMilliseconds;

                    return Json(result);
                }              
            }

            return null;
        }

        [HttpPost]
        public ActionResult Housing()
        {
            if (Request.IsAjaxRequest())
            {
                var housings = UnitOfWork.Repository<Housing>()
                    .GetQ()
                    .Select(x => new
                    {
                        x.HousingId,
                        HousingName = x.Abbreviation
                    });

                return Json(housings);
            }

            return null;
        }

        [HttpPost]
        public ActionResult Auditorium(int chairId, int housingId)
        {
            if (Request.IsAjaxRequest())
            {
                var auditoriums = UnitOfWork.Repository<Auditorium>()
                    .GetQ(filter: x => (x.ChairId == chairId || x.ChairId == null) && x.HousingId == housingId,
                        orderBy: o => o.OrderByDescending(n => n.ChairId)
                            .ThenBy(n => n.AuditoriumNumber))
                    .Select(x => new
                    {
                        x.AuditoriumId,
                        x.AuditoriumNumber,
                        x.AuditoriumType.AuditoriumTypeName,
                        x.Places
                    }).ToList();

                return Json(auditoriums);
            }

            return null;
        }
    }
}