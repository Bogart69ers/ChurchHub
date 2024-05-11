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
    [Authorize(Roles ="Admin")]
    public class AdminController : BaseController
    {
        // GET: Admin
        public ActionResult Index()
        {
            return View();
        }

        [Authorize]
        public ActionResult AdminCreateAccount()
        {

            ViewBag.Role = Utilities.ListRole;

            return View();
        }
        [Authorize]
        [HttpPost]
        public ActionResult AdminCreateAccount(User_Account ua, string ConfirmPass)
        {
            if (!ua.Password.Equals(ConfirmPass))
            {
                ModelState.AddModelError(String.Empty, "Password not match");
                var roleList = Utilities.ListRole;
                ViewBag.Role = roleList;
                return View(ua);
            }

            if (_AccManager.SignUp(ua, ref ErrorMessage) != ErrorCode.Success)
            {
                ModelState.AddModelError(String.Empty, ErrorMessage);

                ViewBag.Role = Utilities.ListRole;
                return View(ua);
            }

            var user = _AccManager.GetUserByEmail(ua.Email);
            string verificationCode = ua.VerCode;

            string emailBody = $"Your verification code is: {verificationCode}";
            string errorMessage = "";

            var mailManager = new MailManager();
            bool emailSent = mailManager.SendEmail(ua.Email, "Verification Code", emailBody, ref errorMessage);

            if (!emailSent)
            {
                ModelState.AddModelError(String.Empty, errorMessage);
                ViewBag.Role = Utilities.ListRole;
                return View(ua);
            }
            TempData["username"] = ua.Username;
            return RedirectToAction("AdminCreateAccount");

        }

        [Authorize]
        public ActionResult AdminDashboard()
        {
            return View();
        }
        [Authorize]
        public ActionResult AccountManagement()
        {
            List<User_Account> UserList = _UserAcc.GetAll();
            return View(UserList);
        }

        [Authorize]
        public ActionResult Edit(int id)
        {
            IsUserLoggedSession();
            
            var user = _AccManager.Retrieve(id, ref ErrorMessage);

            ViewBag.Role = Utilities.ListRole;
            return View(user);
        }

        [Authorize]
        [HttpPost]
        public ActionResult Edit(User_Account ua,int id)
        {
            ViewBag.Role = Utilities.ListRole;
            var user = _AccManager.GetUserById(id);

            if(_AccManager.UpdateUser(ua, ref ErrorMessage) == ErrorCode.Error)
            {
                ModelState.AddModelError(String.Empty, ErrorMessage);
                return View(ua);
            }
            TempData["Message"] = $"User Account {ErrorMessage}!";
            return View(ua);
        }



        [Authorize]
        [HttpPost]
        public ActionResult Delete(int id)
        {
            string errorMsg; // Declare a variable to store the error message
                             // Logic to delete the item with the provided id
            var result = _UserAcc.Delete(id, out errorMsg); // Pass an empty string or null as the second argument
            if (result == ErrorCode.Success)
            {
                // Redirect to the AccountManagement action after successful deletion
                return RedirectToAction("AccountManagement");
            }
            else
            {
                // Handle deletion failure
                TempData["ErrorMessage"] = "Failed to delete item: " + errorMsg; // Display the error message to the user
                return RedirectToAction("AccountManagement");
            }
        }



    }
}