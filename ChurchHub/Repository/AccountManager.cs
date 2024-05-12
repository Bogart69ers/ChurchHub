using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ChurchHub.Utils;

namespace ChurchHub.Repository
{
    public class AccountManager
    {
        private BaseRepository<User_Account> _userAcc;
        private BaseRepository<User_Information> _userInfo;
        private ChurchConnectEntities _dbContext; // Add this field for DbContext


        public AccountManager()
        {
            _userAcc = new BaseRepository<User_Account>();
            _userInfo = new BaseRepository<User_Information>();
            _dbContext = new ChurchConnectEntities(); // Initialize DbContext

        }

        public User_Account GetUserById(int Id)
        {
            return _userAcc.Get(Id);
        }
        public User_Account GetUserByUserId(String userId)
        {
            return _userAcc._table.Where(m => m.UserId == userId).FirstOrDefault();
        }   
        public User_Account GetUserByUsername(String username)
        {
            return _userAcc._table.Where(m => m.Username == username).FirstOrDefault();
        }
        public User_Account GetUserByEmail(String email)
        {
            return _userAcc._table.Where(m => m.Email == email).FirstOrDefault();
        }
        // CHECKS THE USER IF EXIST 
        public ErrorCode SignIn(String username, String password, ref String errMsg)
        {
            var userSignIn = GetUserByUsername(username);
            if (userSignIn == null)
            {
                errMsg = "User not exist!";
                return ErrorCode.Error;
            }

            if (!userSignIn.Password.Equals(password))
            {
                errMsg = "Password is Incorrect";
                return ErrorCode.Error;
            }

            // user exist
            errMsg = "Login Successful";
            return ErrorCode.Success;
        }
       
        public ErrorCode SignUp(User_Account userac, ref String errMsg)
        {
            userac.UserId = Utilities.gUid;
            userac.VerCode = Utilities.code.ToString();
            userac.Date_created = DateTime.Now;
            userac.AccountStatus = (Int32)Status.InActive;

            // if the user already exist this will execute
            if (GetUserByUsername(userac.Username) != null)
            {
                errMsg = "Username Already Exist";
                return ErrorCode.Error;
            }
            // if the email already exist this will execute
            if (GetUserByEmail(userac.Email) != null)
            {
                errMsg = "Email Already Exist";
                return ErrorCode.Error;
            }

            if (_userAcc.Create(userac, out errMsg) != ErrorCode.Success)
            {
                return ErrorCode.Error;
            }

            // use the generated code for OTP "ua.code"
            // send email or sms here...........

            return ErrorCode.Success;
        }

        public ErrorCode UpdateUser(User_Account userac, ref String errMsg)
        {
            return _userAcc.Update(userac.id, userac, out errMsg);
        }
        public ErrorCode UpdateUserInformation(User_Information userinf, ref String errMsg)
        {
            return _userInfo.Update(userinf.id, userinf, out errMsg);
        }
        public User_Information GetUserInfoById(int id)
        {
            return _userInfo.Get(id);
        }
        public User_Information GetUserInfoByUsername(String username)
        {
            var userAcc = GetUserByUsername(username);
            return _userInfo._table.Where(m => m.userId == userAcc.UserId).FirstOrDefault();
        }
        public User_Information GetUserInfoByUserId(String userId)
        {
            return _userInfo._table.Where(m => m.userId == userId).FirstOrDefault();
        }
        public User_Information CreateOrRetrieve(String username, ref String err)
        {
            var User = GetUserByUsername(username);
            var UserInfo = GetUserInfoByUserId(User.UserId);
            if (UserInfo != null)
                return UserInfo;

            UserInfo = new User_Information();
            UserInfo.userId = User.UserId;
            UserInfo.Email = User.Email;
            UserInfo.Status = (Int32)Status.Active;

            var userEmail = User.Email;
            if(userEmail != null)
            {
                UserInfo.Email = userEmail;
            }
            _userInfo.Create(UserInfo, out err);

            return GetUserInfoByUserId(User.UserId);
        }

        public User_Account Retrieve(int id, ref String err)
        {
            var User = GetUserById(id);

            return GetUserByUserId(User.UserId);
        }


        public User_Information GetUserProfile(int userId)
        {
            // Assuming dbContext is your Entity Framework DbContext
            return _userInfo._table.Include("Image").FirstOrDefault(u => u.id == userId);
        }
        public void AddImageToUser(int userId, Image newImage)
        {
            var user = _dbContext.Set<User_Information>().FirstOrDefault(u => u.id == userId);
            if (user != null)
            {
                user.Image.Add(newImage);
                _dbContext.SaveChanges();
            }
        }
    }
}