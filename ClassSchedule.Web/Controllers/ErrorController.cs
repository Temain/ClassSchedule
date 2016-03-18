using System;
using System.Web.Mvc;

namespace ClassSchedule.Web.Controllers
{
    [Authorize]
    public class ErrorController : BaseController
    {
        // Other
        public ActionResult Error() { return View(); }

        // 400
        public ActionResult BadRequest() { return View(); }

        // 401
        // public ActionResult Unauthorized() { return View(); }

        // 404
        public ActionResult NotFound() { return View(); }

        /// <summary>
        /// Логгирование ошибок клиентской части 
        /// </summary>
        [HttpPost]
        public void LogClientError(string message, string url, string line, string charPos, string stack)
        {
            if (Request.IsAjaxRequest())
            {
                Logger.Error(message, new JavaScriptException(stack));
            }
        }
    }

    public class JavaScriptException : Exception
    {
        public JavaScriptException(string message)
            : base(message)
        {
        }
    }
}