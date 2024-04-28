using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ChurchHub.Repository;

namespace ChurchHub.Utils
{
    public enum ErrorCode
    {
        Success,
        Error
    }


    public enum RoleType
    {
        Admin,
        User
    }

    public enum Status
    {
        InActive,
        Active
    }

    public enum IntentionStatus
    {
        Pending,
        Done
    }
    public class Constant
    {
        public const string Role_Admin = "Admin";
        public const string Role_User = "User";

        public const int ERROR = 1;
        public const int SUCCESS = 0;
    
    }
    public class Utilities
    {
        public static String gUid
        {
            get
            {
                return Guid.NewGuid().ToString();
            }
        }
        // Return random number for OTP
        public static int code
        {
            get
            {
                Random r = new Random();
                return r.Next(100000, 999999);
            }
        }

        public static List<SelectListItem> ListRole
        {
            get
            {
                BaseRepository<Role> role = new BaseRepository<Role>();
                var list = new List<SelectListItem>();
                foreach (var item in role.GetAll())
                {
                    var r = new SelectListItem
                    {
                        Text = item.roleName,
                        Value = item.roleId.ToString()
                    };

                    list.Add(r);
                }

                return list;
            }
        } 
        public static List<SelectListItem> ListCategory
        {
            get
            {
                BaseRepository<IntentionCategory> intcat = new BaseRepository<IntentionCategory>();
                var list = new List<SelectListItem>();
                foreach (var item in intcat.GetAll())
                {
                    var i = new SelectListItem
                    {
                        Text = item.intentionName,
                        Value = item.intentionId.ToString()
                    };
                    list.Add(i);
                }
                return list;
            }
        } 
    }
}