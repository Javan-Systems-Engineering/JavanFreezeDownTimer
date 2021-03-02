using System;
using System.Web.Mvc;
using System.Web.Routing;

namespace FreezeDownTimer
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");


            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Login", id = UrlParameter.Optional },
                namespaces: new string[] { "FreezeDownTimer.Controllers" });

            routes.MapRoute(
                name: "AuditReport",
                url: "{controller}/{action}/{StartDate}/{EndDate}",
                defaults: new { controller = "Report", action = "AuditReport", StartDate = UrlParameter.Optional, EndDate = UrlParameter.Optional },
                namespaces: new string[] { "FreezeDownTimer.Controllers" });

            routes.MapRoute(
                name: "GetAuditReport",
                url: "{controller}/{action}/{StartDate}/{EndDate}",
                defaults: new { controller = "Report", action = "GetAuditReport", StartDate = "", EndDate = ""},
                namespaces: new string[] { "FreezeDownTimer.Controllers" });



        }



        //    routes.MapRoute(
        //        name: "Default",
        //        url: "{controller}/{action}/{id}",
        //        defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional },
        //        namespaces: new string[] { "FreezeDownTimer.Controllers" });
        //}
    }
    }
