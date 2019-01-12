using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace MrChorder
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Chord", action = "Index", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "GetFile",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Chord", action = "GetFile", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "SendFile",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Chord", action = "SendFile", id = UrlParameter.Optional }
            );
        }
    }
}
