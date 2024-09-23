using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace front11.Controllers
{
    public class BaseController : Controller
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            ViewData["UserRole"] = HttpContext.Session.GetString("UserRole");
            //ViewData["UserName"] = HttpContext.Session.GetString("UserName");
            base.OnActionExecuting(context);
        }
    }
}
