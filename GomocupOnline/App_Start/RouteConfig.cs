using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Http;
using System.Web.Routing;
using System.Net.Http;

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

            //routes.MapHttpRoute("DefaultApiGet", "Api/{controller}/notify", new { action = "Get" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            //routes.MapHttpRoute("DefaultApiPost", "Api/{controller}/upload", new { action = "Post" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
