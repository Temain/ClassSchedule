using System.Net;
using System.Web.Mvc;

namespace ClassSchedule.Web.Helpers
{
    public class JsonErrorResult : JsonResult
    {
        private readonly HttpStatusCode _statusCode;

        public JsonErrorResult(HttpStatusCode statusCode)
        {
            _statusCode = statusCode;
        }

        public override void ExecuteResult(ControllerContext context)
        {
            context.HttpContext.Response.StatusCode = (int)_statusCode;
            base.ExecuteResult(context);
        }
    }
}