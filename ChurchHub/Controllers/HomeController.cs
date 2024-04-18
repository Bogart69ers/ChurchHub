using ChurchHub.Models;
using ChurchHub.Utils;
using ChurchHub.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace ChurchHub.Controllers
{
    
    public class HomeController : BaseController
    {
        // GET: Home
        [AllowAnonymous]
        public ActionResult Index()
        {
            IsUserLoggedSession();

            return View();
        }

        [AllowAnonymous]
        public ActionResult Login(String ReturnUrl)
        {
            if (User.Identity.IsAuthenticated)
                return RedirectToAction("Index");

            ViewBag.Error = String.Empty;
            ViewBag.ReturnUrl = ReturnUrl;

            return View();
        }
        [AllowAnonymous]
        [HttpPost]
        public ActionResult Login(String username, String password, String ReturnUrl)
        {
            if (_AccManager.SignIn(username, password, ref ErrorMessage) == ErrorCode.Success)
            {
                var user = _AccManager.GetUserByUsername(username);

                if (user.AccountStatus != (Int32)Status.Active)
                {
                    TempData["username"] = username;
                    return RedirectToAction("Verify");
                }
                //
                FormsAuthentication.SetAuthCookie(username, false);
                //
                if (!String.IsNullOrEmpty(ReturnUrl))
                    return Redirect(ReturnUrl);

                switch (user.Role1.roleName)
                {
                    case Constant.Role_Admin:
                        return RedirectToAction("Index");
                    case Constant.Role_User:
                        return RedirectToAction("Index", "Dashboard");
                    default:
                        return RedirectToAction("Index");
                }
            }
            ViewBag.Error = ErrorMessage;

            return View();
        }
        [AllowAnonymous]
        public ActionResult Verify()
        {
            if (String.IsNullOrEmpty(TempData["username"] as String))
                return RedirectToAction("Login");

            return View();
        }
        [AllowAnonymous]
        [HttpPost]
        public ActionResult Verify(String code, String username)
        {
            if (String.IsNullOrEmpty(username))
                return RedirectToAction("Login");

            TempData["username"] = username;

            var user = _AccManager.GetUserByUsername(username);

            if (!user.VerCode.Equals(code))
            {
                TempData["error"] = "Incorrect Code";
                return View();
            }

            user.AccountStatus = (Int32)Status.Active;
            _AccManager.UpdateUser(user, ref ErrorMessage);

            return RedirectToAction("Login");
        }
        [AllowAnonymous]
        public ActionResult SignUp()
        {
            if (User.Identity.IsAuthenticated)
                return RedirectToAction("Index");

            ViewBag.Role = Utilities.ListRole;

            return View();
        }
        [AllowAnonymous]
        [HttpPost]
        public ActionResult SignUp(User_Account ua, String ConfirmPass)
        {
            if (!ua.Password.Equals(ConfirmPass))
            {
                ModelState.AddModelError(String.Empty, "Password not match");
                ViewBag.Role = Utilities.ListRole;
                return View(ua);
            }

            if (_AccManager.SignUp(ua, ref ErrorMessage) != ErrorCode.Success)
            {
                ModelState.AddModelError(String.Empty, ErrorMessage);

                ViewBag.Role = Utilities.ListRole;
                return View(ua);
            }
            TempData["username"] = ua.Username;
            return RedirectToAction("Verify");
        }
        [AllowAnonymous]
        public ActionResult Logout()
        {
            Session.Clear();
            FormsAuthentication.SignOut();
            return RedirectToAction("Login");
        }
        [Authorize]
        public ActionResult MyProfile()
        {
            IsUserLoggedSession();
            var user = _AccManager.CreateOrRetrieve(User.Identity.Name, ref ErrorMessage);

            return View(user);
        }
        [HttpPost]
        public ActionResult MyProfile(User_Information userInf)
        {
            if (_AccManager.UpdateUserInformation(userInf, ref ErrorMessage) == Utils.ErrorCode.Error)
            {
                //
                ModelState.AddModelError(String.Empty, ErrorMessage);
                //
                return View(userInf);
            }
            TempData["Message"] = $"User Information {ErrorMessage}!";
            return View(userInf);
        }

        [AllowAnonymous]
        public ActionResult PageNotFound()
        {
            return Content("Not Found Error 404");
        }
    }
}