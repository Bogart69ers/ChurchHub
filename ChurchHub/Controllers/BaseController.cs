using ChurchHub.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ChurchHub.Controllers
{
    public class BaseController : Controller
    {
        public String ErrorMessage;
        public AccountManager _AccManager;
        public IntentionManager _intentionMgr;
        public BaseRepository<User_Account> _UserAcc;

        public String Username { get { return User.Identity.Name; } }
        public String UserId { get { return _AccManager.GetUserByUsername(Username).UserId; } }

        public BaseController()
        {
            ErrorMessage = String.Empty;
            _AccManager = new AccountManager();
            _intentionMgr = new IntentionManager();
            _UserAcc = new BaseRepository<User_Account>();
        }

        public void IsUserLoggedSession()
        {
            UserLogged userLogged = new UserLogged();
            if (User != null)
            {
                if (User.Identity.IsAuthenticated)
                {
                    userLogged.User_Account = _AccManager.GetUserByUsername(User.Identity.Name);
                    userLogged.User_Information = _AccManager.CreateOrRetrieve(userLogged.User_Account.Username, ref ErrorMessage);
                }
            }
            Session["User"] = userLogged;
        }
    }
}