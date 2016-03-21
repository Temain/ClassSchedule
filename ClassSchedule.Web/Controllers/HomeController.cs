using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ClassSchedule.Domain.DataAccess.Interfaces;
using ClassSchedule.Domain.Models;
using ClassSchedule.Web.Models;
using ClassSchedule.Web.Models.Schedule;
using Microsoft.AspNet.Identity;

namespace ClassSchedule.Web.Controllers
{
    [Authorize]
    public class HomeController : BaseController
    {
        public HomeController(IUnitOfWork unitOfWork)
            : base(unitOfWork)
        {
        }

        public ActionResult Index()
        {
            var viewModel = new ScheduleViewModel();
            var groups = UnitOfWork.Repository<Group>()
                .GetQ(g => g.IsDeleted != true);

            if (UserProfile.GroupId != null)
            {
                groups = groups.Where(g => g.GroupId == UserProfile.GroupId);                
            }
            else if (UserProfile.CourseId != null)
            {
                groups = groups.Where(g => g.CourseId == UserProfile.CourseId);
                viewModel.FacultyName = UserProfile.Course.Faculty.DivisionName;
            }
            
            var groupLessons = groups
                .Select(x => new GroupLessonsViewModel()
                {
                    GroupId = x.GroupId,
                    GroupName = x.DivisionName
                })
                .ToList();
            viewModel.GroupLessons = groupLessons;
            viewModel.WeekNumber = UserProfile.WeekNumber;

            // Вычисление дат занятий по номеру недели
            DateTime yearStartDate = UserProfile.EducationYear.DateStart;
            int delta = DayOfWeek.Monday - yearStartDate.DayOfWeek;
            DateTime firstMonday = yearStartDate.AddDays(delta);
            viewModel.FirstDayOfWeek = firstMonday.AddDays(UserProfile.WeekNumber*7);
            viewModel.LastDayOfWeek = viewModel.FirstDayOfWeek.AddDays(6);

            return View(viewModel);
        }

        [HttpGet]
        public ActionResult SelectFlow()
        {
            var faculties = UnitOfWork.Repository<Faculty>()
                .GetQ(f => f.IsDeleted != true, orderBy: o => o.OrderBy(n => n.DivisionName))
                .Select(x => new {x.FacultyId, FacultyName = x.DivisionName});
            ViewBag.Faculties = new SelectList(faculties, "FacultyId", "FacultyName");

            return View();
        }

        [HttpPost]
        public ActionResult SelectFlow(SelectFlowViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            if (viewModel.CourseId != null || viewModel.GroupId != null || viewModel.FlowId != null)
            {
                UserProfile.CourseId = viewModel.CourseId;
                UserProfile.GroupId = viewModel.GroupId;

                UserManager.Update(UserProfile);
            }

            return RedirectToAction("Index");
        }
    }
}