using front11.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace front11.Controllers
{
    public class AccountController : BaseController
    {
        private readonly _2109060123DbContext _context;

        public AccountController(_2109060123DbContext context)
        {
            _context = context;
        }
        // GET: /Account/ChooseRole
        [HttpGet]
        public IActionResult ChooseRole()
        {
            return View(); // 使用完整路径
        }

        // POST: /Account/ChooseRole
        [HttpPost]
        public IActionResult ChooseRole(string role)
        {
            if (role == "Admin")
            {
                return RedirectToAction("AdminLogin");
            }
            else if (role == "Student")
            {
                return RedirectToAction("StudentLogin");
            }
            else
            {
                // 处理无效角色选择的情况
                return RedirectToAction("ChooseRole");
            }
        }

        // 假设 AdminLogin 和 StudentLogin 动作方法已经存在
        [HttpGet]
        public IActionResult AdminLogin()
        {
            return View();// 会查找 Views/Account/AdminLogin.cshtml
        }
		[HttpPost]
		public IActionResult AdminLogin(LoginUserModel model)
		{
			if (ModelState.IsValid)
			{
				var admin = _context.Admins
					.FirstOrDefault(s => s.Aaccount == model.Account && s.Apassword == model.Apassword);

				if (admin != null)
                {
                    // 登录成功
                    HttpContext.Session.SetString("UserRole", "Admin");
                    HttpContext.Session.SetString("Aaccount", model.Account);
                    return RedirectToAction("AdminStart", "Account");
				}
				else
				{
					// 登录失败
					ModelState.AddModelError(string.Empty, "Invalid login attempt.");
				}
			}
			return View(model);
		}

		[HttpGet]
        public IActionResult StudentLogin()
        {
            return View();// 会查找 Views/Account/StudentLogin.cshtml
        }
        [HttpPost]
        public IActionResult StudentLogin(LoginUserModel model)
        {
            if (ModelState.IsValid)
            {
                var student = _context.Students
                    .FirstOrDefault(s => s.Sid == model.Sid && s.Spassword == model.Spassword);

                if (student != null)
                {
					// 登录成功
					HttpContext.Session.SetString("UserRole", "Student");
                    HttpContext.Session.SetString("Sid", model.Sid);
                    return RedirectToAction("StuStart", "Account");
                }
                else
                {
                    // 登录失败
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                }
            }
            return View(model);
        }

        public IActionResult Logout()
        {
			HttpContext.Session.Clear();
			return RedirectToAction("ChooseRole");
		}

        public IActionResult StuStart()
        {
			return View();
		}
        public IActionResult AdminStart()
        {
                       return View();
        }
	} 
}
