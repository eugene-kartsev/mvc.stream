using System.Web.Mvc;
using System.Web.Routing;

namespace Mvc.Stream.Web.App_Start
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: null,
                url: "Stream/480p/old/download",
                defaults: new { controller = "Home", action = "Stream480pOldDownload", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: null,
                url: "Stream/480p/old",
                defaults: new { controller = "Home", action = "Stream480pOld", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: null,
                url: "Stream/480p/download",
                defaults: new { controller = "Home", action = "Stream480pDownload", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: null,
                url: "Stream/480p",
                defaults: new { controller = "Home", action = "Stream480p", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: null,
                url: "Stream/720p/old/download",
                defaults: new { controller = "Home", action = "Stream720pOldDownload", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: null,
                url: "Stream/720p/old",
                defaults: new { controller = "Home", action = "Stream720pOld", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: null,
                url: "Stream/720p/download",
                defaults: new { controller = "Home", action = "Stream720pOldDownload", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: null,
                url: "Stream/720p",
                defaults: new { controller = "Home", action = "Stream720p", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: null,
                url: "Stream/1080p/old/download",
                defaults: new { controller = "Home", action = "Stream1080pOldDownload", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: null,
                url: "Stream/1080p/old",
                defaults: new { controller = "Home", action = "Stream1080pOld", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: null,
                url: "Stream/1080p/download",
                defaults: new { controller = "Home", action = "Stream1080pDownload", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: null,
                url: "Stream/1080p",
                defaults: new { controller = "Home", action = "Stream1080p", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}