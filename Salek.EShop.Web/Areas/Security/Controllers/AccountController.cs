using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Salek.EShop.Web.Controllers;
using Salek.EShop.Web.Models.ApplicationServices.Abstraction;
using Salek.EShop.Web.Models.Identity;
using Salek.EShop.Web.Models.ViewModels;

namespace Salek.EShop.Web.Areas.Security.Controllers
{
    [Area("Security")]
    public class AccountController : Controller
    {
        ISecurityApplicationService security;

        public AccountController(ISecurityApplicationService security)
        {
            this.security = security;
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel registerVM)
        {
            if (ModelState.IsValid)
            {
                string[] errors = await security.Register(registerVM, RolesEnum.Customer);

                if (errors == null)
                {
                    LoginViewModel loginVM = new LoginViewModel()
                    {
                        UserName = registerVM.UserName,
                        Password = registerVM.Password
                    };
                    return await Login(loginVM);
                }
                //return RedirectToAction(nameof(HomeController.Index), nameof(HomeController).Replace("Controller", ""), new { area = "" });
            }

            return View(registerVM);
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel loginVM)
        {
            if (ModelState.IsValid)
            {
                loginVM.LoginFailed = !await security.Login(loginVM);
                if (loginVM.LoginFailed == false)
                {
                    return RedirectToAction(nameof(HomeController.Index), nameof(HomeController).Replace("Controller", ""), new { area = "" });
                }
            }

            return View(loginVM);
        }

        public async Task<IActionResult> Logout()
        {
            await security.Logout();
            return RedirectToAction(nameof(Login));
        }
    }
}
