using System.Web;
using System.Web.Optimization;

namespace FreezeDownTimer
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {

            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                "~/Content/bootstrap.min.css",
                "~/Content/bootstrap-responsive.min.css",
                "~/Content/site.css"));

            var scriptBundle = new ScriptBundle("~/bundles/scripts")
                          .Include("~/Scripts/jquery-{version}.js")
                          .Include("~/Scripts/jquery-ui-1.10.3.custom.min.js")
                          .Include("~/Scripts/jquery.validate*")
                          .Include("~/Scripts/bootstrap.js")
                          .Include("~/Scripts/jquery-ui-{version}.js");
            //.Include("~/Scripts/application.js");
            bundles.Add(scriptBundle);

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at https://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            var stylesBundle = new StyleBundle("~/bundles/css")
                .Include("~/Content/themes/base/jquery-ui.min.css")
                //.Include("~/Content/themes/base/jquery-ui.min.css")
                //.Include("~/Content/themes/base/all.css")
                .Include("~/Content/bootstrap.css")
                .Include("~/Content/site.css")
                .Include("~/Content/themes/ui-lightness/jquery-ui-1.10.3.min.css");
            //.Include("~/Content/application.css");
            bundles.Add(stylesBundle);

            BundleTable.EnableOptimizations = true;

        }
    }
}
