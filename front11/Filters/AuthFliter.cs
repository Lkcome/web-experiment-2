using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Http;

namespace front11.Filters
{
	public class AuthFilter : ActionFilterAttribute
	{
		public override void OnActionExecuting(ActionExecutingContext context)
		{
			var controller = context.RouteData.Values["controller"].ToString();
			var action = context.RouteData.Values["action"].ToString();

			// 检查用户是否登录
			bool isLoggedIn = context.HttpContext.Session.GetString("UserRole") != null;

			// 如果用户没有登录，且请求的不是 ChooseRole 页面，重定向到角色选择页面
			if (!isLoggedIn && !(controller == "Account" && action == "ChooseRole"))
			{
				context.Result = new RedirectToActionResult("ChooseRole", "Account", null);
			}
		}
	}
}
