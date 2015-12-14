using EFunTech.Sms.Portal.Controllers.Common;
using EFunTech.Sms.Portal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EFunTech.Sms.Portal.Filters
{
    /// <summary>
    /// 在每個controller都加上ViewBag.MenuItems屬性，提供 _Menu.cshtml 作為產生選單的依據
    /// 
    /// http://stackoverflow.com/questions/5453327/how-to-set-viewbag-properties-for-all-views-without-using-a-base-class-for-contr
    /// </summary>
    public class UserMenuActionFilter : ActionFilterAttribute
    {
        public override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            var controller = filterContext.Controller as MvcControllerBase;
            if (controller != null)
            {
                filterContext.Controller.ViewBag.MenuItems = controller.GetMenuItems();
            }
            else
            {
                filterContext.Controller.ViewBag.MenuItems = new List<MenuItemModel>();
            }
        }
    }
}