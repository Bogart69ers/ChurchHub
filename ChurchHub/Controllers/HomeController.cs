using ChurchHub.Models;
using ChurchHub.Utils;
using ChurchHub.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.IO;


namespace ChurchHub.Controllers
{
    [Authorize(Roles ="Admin, User")]
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
            if (User.Identity.IsAuthenticated) { 
                return RedirectToAction("Index");
            }

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
                var info = _AccManager.GetUserInfoByUsername(username);

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
                        if (info.FirstName == null)
                        {
                            return RedirectToAction("MyProfile");
                        }
                        else
                        {
                            return RedirectToAction("../Admin/AdminDashboard");
                        }
                    case Constant.Role_User:
                        if(info.FirstName == null)
                        {
                          return RedirectToAction("MyProfile");
                        }
                        else
                        {
                          return RedirectToAction("Index");
                        }
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

            return RedirectToAction("MyProfile");
        }
        
        [AllowAnonymous]
        public ActionResult SignUp()
        {

            ViewBag.Role = Utilities.ListRole;

            return View();
        }
        [AllowAnonymous]
        [HttpPost]
        public ActionResult SignUp(User_Account ua, string ConfirmPass)
        {
            if (!ua.Password.Equals(ConfirmPass))
            {
                ModelState.AddModelError(String.Empty, "Password not match");
                var roleList = Utilities.ListRole;
                roleList.FirstOrDefault(x => x.Value == "user").Selected = true;
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

            var username = User.Identity.Name;

            var user = _AccManager.CreateOrRetrieve(username, ref ErrorMessage);

            var userEmail = _AccManager.GetUserByEmail(user.Email);

            ViewBag.userEmail = userEmail.Email;

            return View(user);
        }
        [HttpPost]
        public ActionResult MyProfile(User_Information userInf, HttpPostedFileBase profilePicture)
        {
            var userEmail = _AccManager.GetUserByEmail(userInf.Email); // Assuming User.Identity.Name contains the user's email

            ViewBag.userEmail = userEmail.Email;

            if (profilePicture != null && profilePicture.ContentLength > 0)
            {
                // Save the profile picture to the server
                var fileName = Path.GetFileName(profilePicture.FileName);
                var serverSavePath = Path.Combine(Server.MapPath("~/UploadedFiles/"), fileName);
                profilePicture.SaveAs(serverSavePath);

                // Create a new Image object and associate it with the user
                var user = _AccManager.GetUserInfoByUserId(UserId); // Assuming UserId contains the user's ID
                var image = new Image { imageFile = fileName, userId = user.id };

                // Add the image to the user's collection of images
                user.Image.Add(image);

                if (_AccManager.UpdateUserInformation(userInf, ref ErrorMessage) == ErrorCode.Error)
                {
                    ModelState.AddModelError(String.Empty, ErrorMessage);
                    return View(userInf);
                }
            }
            else
            {
                ModelState.AddModelError(String.Empty, "Please select a valid image file.");
            }


            TempData["Message"] = ModelState.IsValid ? "Profile picture updated successfully." : "Failed to update profile picture.";
            return RedirectToAction("MyProfile"); // Redirect to the MyProfile action to refresh the view
        }

        [AllowAnonymous]
        public ActionResult PageNotFound()
        {
            return Content("Not Found Error 404");
        }
        [Authorize]
        public ActionResult Booking()
        {
            IsUserLoggedSession();
            var username = User.Identity.Name;
            ViewBag.IntentionType = Utilities.ListCategory;
            return View();
        }

        [HttpPost]
        public ActionResult Booking(Intention intent)
        {
            var username = User.Identity.Name;
            var user = _AccManager.GetUserInfoByUserId(UserId); // Get the user info

            if (_intentionMgr.CreateIntention(intent, username, ref ErrorMessage) != ErrorCode.Success)
            {
                ModelState.AddModelError(String.Empty, ErrorMessage);

                ViewBag.IntentionType = Utilities.ListCategory;
                return View(intent);
            }

            // Redirect to the Payment action with the newly created intention's GUID
            return RedirectToAction("Payment", new { intentionGUID = intent.intentionGUID });
        }



        [AllowAnonymous]
        public ActionResult Schedule()
        {
            return View();
        }

        [Authorize]
        public ActionResult MyIntentions()
        {
            IsUserLoggedSession();
            var user = _AccManager.GetUserByUserId(UserId);
            ViewBag.CurrentUserId = user.UserId; // Pass the current user's ID to the view
            List<Intention> IntentionList = _intentionMgr.GetIntentionsByUserId(user.UserId); // Adjust the method to get intentions by UserId
            return View(IntentionList);
        }

        [Authorize]
        public ActionResult Payment(string intentionGUID)
        {
            // Pass intentionGUID to the view for reference
            ViewBag.IntentionGUID = intentionGUID;

            return View();
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult Payment(Payment payment, string intentionGUID)
        {
            if (ModelState.IsValid)
            {
                // Create an instance of PaymentManager
                var paymentManager = new PaymentManager();

                // Call the CreatePayment method to handle payment creation
                var resultCode = paymentManager.CreatePayment(payment, intentionGUID, ref ErrorMessage);

                if (resultCode == ErrorCode.Success)
                {
                    // Redirect to the booking page with the intention GUID
                    return RedirectToAction("Booking", new { intentionGUID });
                }
                else
                {
                    // Handle the case where payment creation fails
                    ModelState.AddModelError(string.Empty, ErrorMessage);
                }
            }

            // If model state is not valid, return to payment view with errors
            return View(payment);
        }
    }
}