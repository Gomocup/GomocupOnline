using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace GomocupOnline
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");


          //  routes.MapRoute(
          //  "home", // Name
          //  "", // URL
          //  new { controller = "Home", action = "Index" }, // Homepage
          //  new { controller = "Home", action = "Index" } // Homepage
          //);

            routes.MapRoute(
         "home", // Name
         "", // URL
         new { controller = "Online", action = "Index" } // Homepage         
       );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
