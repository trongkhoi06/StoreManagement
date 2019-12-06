using StoreManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StoreManagement.Class
{
    public class ValidationBeforeCommandDAO
    { 
        private UserModel db = new UserModel();

        public bool IsValidUser(string userID, string Role)
        {
            SystemUser user = db.SystemUsers.Find(userID);
            if (user == null) return false;
            if (user.RoleName != Role) return false;
            return true;
        }

        public bool IsValidUser(string userID, string deviceCode, string Role)
        {
            SystemUser user = db.SystemUsers.Find(userID);
            if (user == null) return false;
            if (user.RoleName != Role) return false;

            // còn phần kiểm trùng thiết bị
            //if ()
            return true;
        }
    }
}