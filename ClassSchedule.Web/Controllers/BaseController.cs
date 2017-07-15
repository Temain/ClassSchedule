using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using ClassSchedule.Domain.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using NLog;

namespace ClassSchedule.Web.Controllers
{
    public abstract class BaseController : Controller
    {
        protected BaseController() { }
       
        private Logger _logger;
        public Logger Logger
        {
            get
            {
                if (_logger == null)
                {
                    _logger = LogManager.GetLogger(this.GetType().FullName);
                }
                return _logger;
            }
        }

        private ApplicationUserManager _userManager;
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            set
            {
                _userManager = value;
            }
        }

        private ApplicationRoleManager _roleManager;
        public ApplicationRoleManager RoleManager
        {
            get
            {
                return _roleManager ?? HttpContext.GetOwinContext().Get<ApplicationRoleManager>();
            }
            private set
            {
                _roleManager = value;
            }
        }

        public ApplicationUser UserProfile { get; set; }
        public ApplicationRole UserRole { get; set; }

        protected override void Initialize(RequestContext requestContext)
        {
            base.Initialize(requestContext);

            var userId = User.Identity.GetUserId();
            if (userId != null)
            {
                UserProfile = UserManager.FindById(userId);
                ViewBag.UserProfile = UserProfile;
                ViewBag.UserName = UserProfile.ShortName;

                var roles = UserManager.GetRoles(userId);
                var currentRole = roles.LastOrDefault(r => r != "Admin") ?? "Admin";
                UserRole = RoleManager.FindByName(currentRole);
                ViewBag.RoleName = UserRole.FullName ?? "";
            }           
        }

        protected override void OnException(ExceptionContext filterContext)
        {
            if (filterContext.ExceptionHandled || filterContext.HttpContext.IsCustomErrorEnabled)
            {
                return;
            }

            if (new HttpException(null, filterContext.Exception).GetHttpCode() != 500)
            {
                return;
            }

            // if the request is AJAX return JSON else view.
            if (filterContext.HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                filterContext.Result = new JsonResult
                {
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                    Data = new
                    {
                        error = true,
                        message = filterContext.Exception.Message
                    }
                };
            }
            else
            {
                var controllerName = (string)filterContext.RouteData.Values["controller"];
                var actionName = (string)filterContext.RouteData.Values["action"];
                var model = new HandleErrorInfo(filterContext.Exception, controllerName, actionName);

                filterContext.Result = new ViewResult
                {
                    ViewName = "Error",
                    // MasterName = Master,
                    ViewData = new ViewDataDictionary<HandleErrorInfo>(model),
                    TempData = filterContext.Controller.TempData
                };
            }

            Logger.Error(filterContext.Exception, filterContext.Exception.Message, new {});

            filterContext.ExceptionHandled = true;
            filterContext.HttpContext.Response.Clear();
            filterContext.HttpContext.Response.StatusCode = 500;

            filterContext.HttpContext.Response.TrySkipIisCustomErrors = true;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }
            }

            base.Dispose(disposing);
        }
    }
}