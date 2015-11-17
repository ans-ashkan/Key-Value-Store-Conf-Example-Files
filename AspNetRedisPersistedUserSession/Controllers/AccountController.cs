using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;

namespace AspNetRedisPersistedUserSession.Controllers
{

    public class AccountController : Controller
    {
        [AllowAnonymous]
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var sm = HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
                var um = HttpContext.GetOwinContext().Get<ApplicationUserManager>();
                var res = sm.PasswordSignIn(model.Username, model.Password, model.RememberMe, false);
                switch (res)
                {
                    case SignInStatus.Success:
                        return RedirectToAction("Index", "SecuredSection");
                    default:
                        return Content(res.ToString());
                }
            }
            return View(model);
        }

        public ActionResult LogOff()
        {
            var sm = HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            sm.AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Index", "Home");
        }

    }

    public class LoginViewModel
    {
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
        public bool RememberMe { get; set; }
    }
}