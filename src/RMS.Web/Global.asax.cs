using RequestsManagementSystem.Infrastructure;
using RMS.Core;
using System.Configuration;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace RequestsManagementSystem
{
    public class MvcApplication : System.Web.HttpApplication
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js",
                        "~/Scripts/jquery.dataTables.min.js",
                        "~/Scripts/dataTables.scroller.min.js",
                        "~/Scripts/index.js"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/fontawesome/font-awesome.css",
                      "~/Content/site.css",
                      "~/Content/jquery.dataTables.min.css",
                      "~/Content/dataTables.scroller.min.css"));
        }

        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }

        /// <summary>
        ///  Handles the start event of the Application.
        /// </summary>
#pragma warning disable CA1822 // Mark members as static
        protected void Application_Start()
#pragma warning restore CA1822 // Mark members as static
        {
            var appConfig = ConfigurationManager.GetSection("appConfig") as AppConfig;
            var engine = (WebAppEngine)EngineContext.Create<WebAppEngine>();
            engine.Initialize(appConfig);
            engine.SetResolver();

            // Standard MVC setup:
            AreaRegistration.RegisterAllAreas();
            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);
            RegisterBundles(BundleTable.Bundles);
        }
    }
}
