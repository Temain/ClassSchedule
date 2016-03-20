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
                    .GetQ(x => x.IsDeleted != true && x.FacultyId == facultyId,
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
    }
}