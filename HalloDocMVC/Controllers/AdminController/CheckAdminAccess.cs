using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace HalloDocMVC.Controllers.AdminController
{
    public class CheckAdminAccess : ActionFilterAttribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext filterContext)
        {
            var rd = filterContext.RouteData;
            string currentAction = rd.Values["action"].ToString();
            string currentController = rd.Values["controller"].ToString();
            //string currentArea = rd.DataTokens["area"].ToString();

            if (filterContext.HttpContext.Session.GetString("UserID") == null || filterContext.HttpContext.Session.GetString("RoleId") == "3") //patient role id
            {
                filterContext.Result = new RedirectResult("../Home/Index");
            }
        }

        public override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            filterContext.HttpContext.Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
            filterContext.HttpContext.Response.Headers["Expires"] = "-1";
            filterContext.HttpContext.Response.Headers["Pragma"] = "no-cache";
            base.OnResultExecuting(filterContext);
        }
    }
}
