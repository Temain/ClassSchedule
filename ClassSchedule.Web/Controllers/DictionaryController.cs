using System.Linq;
using System.Web.Mvc;
using System.Data.Entity;
using ClassSchedule.Domain.Context;
using ClassSchedule.Business.Models.Schedule;
using ClassSchedule.Business.Interfaces;
using System.Collections.Generic;
using ClassSchedule.Business.Models;
using System;

namespace ClassSchedule.Web.Controllers
{
    public class DictionaryController : BaseController
    {
        private readonly ApplicationDbContext _context;
        private readonly IDictionaryService _dictionaryService;

        public DictionaryController(ApplicationDbContext context, IDictionaryService dictionaryService)
        {
            _context = context;
            _dictionaryService = dictionaryService;
        }

        [HttpPost]
        public ActionResult Course(int facultyId)
        {
            if (Request.IsAjaxRequest())
            {
                var courses = _dictionaryService.GetCourses(facultyId);

                return Json(courses);
            }

            return null;
        }

        [HttpPost]
        public ActionResult CourseNumber(int facultyId, int? educationFormId, int? educationLevelId)
        {
            if (Request.IsAjaxRequest())
            {
                var courses = _dictionaryService.GetCourseNumbers(facultyId, educationFormId, educationLevelId);

                return Json(courses);
            }

            return null;
        }

        [HttpPost]
        public ActionResult Group(int facultyId, int? courseId, int? educationFormId, int? educationLevelId, int? courseNumber)
        {
            if (Request.IsAjaxRequest())
            {
                var groups = _dictionaryService.GetGroups(facultyId, courseId, educationFormId, educationLevelId, courseNumber);

                return Json(groups);
            }

            return null;
        }

        [HttpPost]
        public ActionResult EducationForm()
        {
            if (Request.IsAjaxRequest())
            {
                var forms = _dictionaryService.GetEducationForms();

                return Json(forms);
            }

            return null;
        }

        [HttpPost]
        public ActionResult EducationLevel()
        {
            if (Request.IsAjaxRequest())
            {
                var levels = _dictionaryService.GetEducationLevels();

                return Json(levels);
            }

            return null;
        }

        [HttpPost]
        [Obsolete("В версии 2.0.0 больше нет функционала загрузки учебных планов")]
        public ActionResult AcademicPlanYear()
        {
            if (Request.IsAjaxRequest())
            {
                return Json(new { Message = "Depracated" });
            }

            return null;
        }

        [HttpPost]
        public ActionResult Direction()
        {
            if (Request.IsAjaxRequest())
            {
                var directions = _dictionaryService.GetEducationDirections();

                return Json(directions);
            }

            return null;
        }

        [HttpPost]
        public ActionResult EducationProfile(int educationFormId, int educationDirectionId, int yearStart)
        {
            if (Request.IsAjaxRequest())
            {
                var profiles = _dictionaryService.GetEducationProfiles(educationFormId, educationDirectionId, yearStart);

                return Json(profiles);
            }

            return null;
        }

        [HttpPost]
        public ActionResult Discipline(string query, int? chairId)
        {
            if (Request.IsAjaxRequest())
            {
                var disciplines = _dictionaryService.GetDisciplines(query, chairId);

                return Json(disciplines);
            }

            return null;
        }

        [HttpPost]
        public ActionResult Teacher(int? chairId, string query)
        {
            if (Request.IsAjaxRequest())
            {
                var teachers = _dictionaryService.GetTeachers(UserProfile.EducationYearId ?? 0, chairId, query);

                return Json(teachers);
            }

            return null;
        }

        [HttpPost]
        public ActionResult TeacherWithEmployment(int chairId, int weekNumber, int dayNumber, int classNumber, int groupId)
        {
            if (Request.IsAjaxRequest())
            {
                var chairTeachers = _dictionaryService.GetTeacherWithEmployment(UserProfile.EducationYearId ?? 0, chairId, weekNumber, dayNumber, classNumber, groupId);

                return Json(chairTeachers);
            }

            return null;
        }

        [HttpPost]
        public ActionResult Housing()
        {
            if (Request.IsAjaxRequest())
            {
                var housings = _dictionaryService.GetHousings();

                return Json(housings);
            }

            return null;
        }

        [HttpPost]
        public ActionResult HousingEqualLength()
        {
            if (Request.IsAjaxRequest())
            {
                var housings = _dictionaryService.GetHousingEqualLength();

                return Json(housings);
            }

            return null;
        }

        [HttpPost]
        public ActionResult Auditorium(int? chairId, int? housingId, string query, bool shortResult = false)
        {
            if (Request.IsAjaxRequest())
            {
                var auditoriums = _dictionaryService.GetAuditoriums(chairId, housingId, query, shortResult);

                return Json(auditoriums);
            }

            return null;
        }

        [HttpPost]
        public ActionResult AuditoriumWithEmployment(int chairId, int housingId, int weekNumber, int dayNumber, int classNumber, int groupId)
        {
            if (Request.IsAjaxRequest())
            {
                var auditoriums = _dictionaryService.GetAuditoriumWithEmployment(chairId, housingId, weekNumber, dayNumber, classNumber, groupId);

                return Json(auditoriums);
            }

            return null;
        }
    }
}