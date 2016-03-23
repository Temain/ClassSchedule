using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ClassSchedule.Domain.DataAccess.Interfaces;
using ClassSchedule.Domain.Models;

namespace ClassSchedule.Web.Controllers
{
    public class DictionaryController : BaseController
    {
        public DictionaryController(IUnitOfWork unitOfWork)
            : base(unitOfWork)
        {
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
    }
}