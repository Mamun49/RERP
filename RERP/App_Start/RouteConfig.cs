using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace RERP
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Return",
                url: "Return",
                defaults: new { controller = "Sell", action = "SellList" }
            );
            routes.MapRoute(
                name: "Sell",
                url: "Sell",
                defaults: new { controller = "Sell", action = "Index" }
            );
            routes.MapRoute(
                name: "SubCategory",
                url: "SubCategory",
                defaults: new { controller = "SubCategory", action = "Index" }
            );
            routes.MapRoute(
                name: "ExpenceType",
                url: "ExpenceType",
                defaults: new { controller = "ExpenceType", action = "Index" }
            );
            routes.MapRoute(
                name: "Expence",
                url: "Expence",
                defaults: new { controller = "Expence", action = "Index" }
            );
            routes.MapRoute(
                name: "Stock",
                url: "Stock",
                defaults: new { controller = "Report", action = "Index" }
            );
            routes.MapRoute(
                name: "ExpenceReport",
                url: "ExpenceReport",
                defaults: new { controller = "Report", action = "ExpenceReport" }
            ); 
            routes.MapRoute(
                name: "SellReport",
                url: "SellReport",
                defaults: new { controller = "Report", action = "SellReport" }
            );
            routes.MapRoute(
                name: "Color",
                url: "Color",
                defaults: new { controller = "Color", action = "Index" }
            );
            routes.MapRoute(
                name: "Size",
                url: "Size",
                defaults: new { controller = "Size", action = "Index" }
            );
            routes.MapRoute(
                name: "ProductList",
                url: "ProductList",
                defaults: new { controller = "Product", action = "Index" }
            );
            routes.MapRoute(
                name: "Category",
                url: "Category",
                defaults: new { controller = "Category", action = "Index" }
            );
            routes.MapRoute(
                name: "logout",
                url: "logout",
                defaults: new { controller = "Auth", action = "Logout" }
            );
            routes.MapRoute(
                name: "login",
                url: "login",
                defaults: new { controller = "Auth", action = "Login" }
            );
            routes.MapRoute(
                name: "Dashboard",
                url: "Dashboard",
                defaults: new { controller = "Home", action = "Index" }
            );
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Auth", action = "Login", id = UrlParameter.Optional }
            );
        }
    }
}
