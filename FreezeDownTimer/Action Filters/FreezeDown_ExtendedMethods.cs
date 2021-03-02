using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace FreezeDownTimer.Filters
{
    //Get requesting user's roles/permissions from database tables...      
    public static class FreezeDown_ExtendedMethods
    {
        public static bool HasRole(this ControllerBase controller, string role)
        {
            bool bFound = false;
            try
            {
                //Check if the requesting user has the specified role...
                bFound = new FreezeDownUser(controller.ControllerContext.HttpContext.User.Identity.Name).HasRole(role);
            }
            catch { }
            return bFound;
        }

        public static bool HasRoles(this ControllerBase controller, string roles)
        {
            bool bFound = false;
            try
            {
                //Check if the requesting user has any of the specified roles...
                //Make sure you separate the roles using ; (ie "Sales Manager;Sales Operator"
                bFound = new FreezeDownUser(controller.ControllerContext.HttpContext.User.Identity.Name).HasRoles(roles);
            }
            catch { }
            return bFound;
        }

        public static bool HasPermission(this ControllerBase controller, string permission)
        {
            bool bFound = false;
            try
            {
                //Check if the requesting user has the specified application permission...
                bFound = new FreezeDownUser(controller.ControllerContext.HttpContext.User.Identity.Name).HasPermission(permission);
            }
            catch { }
            return bFound;
        }

        public static bool HasAdminPermission(this ControllerBase controller)
        {
            bool bFound = false;
            try
            {
                //Check if the requesting user has the specified application permission...
                bFound = new FreezeDownUser(controller.ControllerContext.HttpContext.User.Identity.Name).HasAdminPermission();
            }
            catch { }
            return bFound;
        }


        public static bool IsSysAdmin(this ControllerBase controller)
        {
            bool bIsSysAdmin = false;
            try
            {

                string UserName = GetUID();

                //Check if the requesting user has the System Administrator privilege...
                //bIsSysAdmin = new FreezeDownUser(controller.ControllerContext.HttpContext.User.Identity.Name).IsSysAdmin;
                bIsSysAdmin = new FreezeDownUser(UserName).IsSysAdmin;

            }
            catch (Exception ex)
            { var x = ex.Message; }
            return bIsSysAdmin;
        }

        public static string GetUID()
        {
            //HttpCookie authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];
            //FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(authCookie.Value);

            //string cookiePath = ticket.CookiePath;
            //DateTime expiration = ticket.Expiration;
            //bool expired = ticket.Expired;
            //bool isPersistent = ticket.IsPersistent;
            //DateTime issueDate = ticket.IssueDate;
            //string name = ticket.Name;
            //string userData = ticket.UserData;
            //int version = ticket.Version;

            string cookieName = FormsAuthentication.FormsCookieName; //Find cookie name
            HttpContext context = HttpContext.Current;
            HttpCookie authCookie = context.Request.Cookies[cookieName]; //Get the cookie by it's name
            string UserName = "";
            if (authCookie != null)
            {
                FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(authCookie.Value); //Decrypt it
                UserName = ticket.Name; //You have the UserName!
            }

            return UserName;

        }

    }

}