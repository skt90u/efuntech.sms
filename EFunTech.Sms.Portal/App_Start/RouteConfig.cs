using EFunTech.Sms.Schema;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using System.Data.Entity;
using EFunTech.Sms.Portal.Controllers.Common;

namespace EFunTech.Sms.Portal
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            // Route 設定教學
            // http://blog.miniasp.com/post/2011/08/01/ASPNET-MVC-Developer-Note-Part-21-Routing-Concepts-and-Skills.aspx
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            // 快取 favicon，似乎沒有作用
            // http://forums.asp.net/t/1253084.aspx?The+controller+for+path+favicon+ico+does+not+implement+IController
            routes.IgnoreRoute("{*favicon}", new {favicon=@"(.*/)?favicon.ico(/.*)?"});
            // routes.IgnoreRoute("{*favicon}", new { favicon = ".*/favicon\\.ico" });
            // http://stackoverflow.com/questions/1003350/why-is-chrome-searching-for-my-favicon-ico-when-i-serve-up-a-file-from-asp-net-m
            // routes.MapRoute("ignore-favicon", "{*path}", null, new { path = ".*/favicon\\.ico" });

            

            foreach (var keyValue in MvcControllerBase.MenuItemMap.Value)
            {
                var role = keyValue.Key;
                var menuItems = keyValue.Value;

                foreach (var menuItem in menuItems)
                {
                    if (string.IsNullOrEmpty(menuItem.MapRouteUrl)) continue;

                    if (routes[menuItem.Name] != null) continue;

                    routes.MapRoute(
                            name: menuItem.Name,
                            url: menuItem.MapRouteUrl,
                            defaults: new
                            {
                                controller = menuItem.ControllerName,
                                action = menuItem.ActionName,
                                id = UrlParameter.Optional
                            }
                        );
                }
            }

            //----------------------------------------
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
