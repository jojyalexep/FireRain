using EP.BulkMessage.Service.Domain.UserModule;
using EP.BulkMessage.Service.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Security;

namespace EP.BulkMessage.Presentation.Web.Modules
{
    public static class UserModule
    {
        public static bool ValidateUser(string username, string password)
        {
            var user = GetUser(username);
            if (user != null && user.Username == username && user.Password == password)
                return true;
            return false;
        }

        public static EPUser GetUser(string username)
        {
            UserService userService = new UserService();
            return userService.GetUser(username);
        }

        public static bool ChangePassword(string username, string oldPassword, string newPassword)
        {
            if (ValidateUser(username, oldPassword))
            {
                new UserService().UpdateUserPassword(username, newPassword);
                return true;
            }
            return false;
        }

        public static string GetLoggedUserRole()
        {
            return GetLoggedUserData().Role;
        }

        public static bool UserInRole(string role)
        {
            if (GetLoggedUserData().Role == role)
                return true;
            return false;
        }

        public static int GetUsersDepartment()
        {
            return GetLoggedUserData().Department.Id;
        }

        public static string GetUsersFullName()
        {
            return GetLoggedUserData().Name;
        }


        public static EPUser GetLoggedUserData()
        {
            var cookie = HttpContext.Current.Request.Cookies[FormsAuthentication.FormsCookieName];
            var userArray = FormsAuthentication.Decrypt(cookie.Value).UserData;
            var userData = userArray.Split(','); // [0] - Username, [1] - Role
            int departmentId = 0;
            Int32.TryParse(userData[2], out departmentId);
            return new EPUser { Username = userData[0], Role = userData[1], Department = new Department { Id = departmentId }, Name = userData[3] };
        }

        static bool IsValidEmail(string strIn)
        {
            
            // Return true if strIn is in valid e-mail format.
            return Regex.IsMatch(strIn, @"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$");
        }
    }


    public static class UserPermission
    {

        public const string CreateCampaignPermission = "CreateCampaignPermission";
        public const string DeleteCampaignPermission = "DeleteCampaignPermission";
        public const string EditCampaignPermission = "EditCampaignPermission";
        public const string ApproveCampaignPermission = "ApproveCampaignPermission";


        public static bool UserHasPermission(string permission)
        {
            string role = UserModule.GetLoggedUserRole();
            if (UserPermissions(role).Contains(permission))
                return true;
            return false;
        }

        public static List<string> UserPermissions(string role)
        {
            List<string>  permissions = new  List<string>();
            if(role == Role.Approver)
            {
                permissions.Add(ApproveCampaignPermission);
            }
            else if (role == Role.User)
            {
                permissions.Add(CreateCampaignPermission);
                permissions.Add(DeleteCampaignPermission);
                permissions.Add(EditCampaignPermission);
            }
            else if (role == Role.Viewer)
            {
                
            }
            else if (role == Role.Admin)
            {
                permissions.Add(CreateCampaignPermission);
                permissions.Add(DeleteCampaignPermission);
                permissions.Add(EditCampaignPermission);
                permissions.Add(ApproveCampaignPermission);
            }
            return permissions;
        }
    }

  
}