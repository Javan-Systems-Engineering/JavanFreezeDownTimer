using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;


namespace FreezeDownTimer.Filters
{
    public class FreezeDownAttribute : AuthorizeAttribute
    {
        public override void OnAuthorization(AuthorizationContext filterContext)
        {


            string UserName = FreezeDown_ExtendedMethods.GetUID();

            //Create permission string based on the requested controller name and action name in the format 'controllername-action'
            string requiredPermission = String.Format("{0}-{1}", filterContext.ActionDescriptor.ControllerDescriptor.ControllerName, filterContext.ActionDescriptor.ActionName);

            //FreezeDownUser requestingUser = new FreezeDownUser(filterContext.RequestContext.HttpContext.User.Identity.Name);
            FreezeDownUser requestingUser = new FreezeDownUser(UserName);

            //Check if the requesting user has the permission to run the controller's action
            if (!requestingUser.HasPermission(requiredPermission) & !requestingUser.IsSysAdmin)
            {
                //User doesn't have the required permission and is not a SysAdmin, return our custom “401 Unauthorized” access error
                //Since we are setting filterContext.Result to contain an ActionResult page, the controller's action will not be run.
                //The custom “401 Unauthorized” access error will be returned to the browser in response to the initial request.
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary { { "action", "Index" }, { "controller", "Unauthorized" } });
            }
            //If the user has the permission to run the controller's action, then filterContext.Result will be uninitialized and
            //executing the controller's action is dependant on whether filterContext.Result is uninitialized.
        }
    }
}