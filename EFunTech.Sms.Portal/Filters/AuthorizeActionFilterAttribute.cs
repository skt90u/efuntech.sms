using EFunTech.Sms.Portal.Controllers.Common;
using EFunTech.Sms.Portal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EFunTech.Sms.Portal.Filters
{
    [Obsolete("20151015 Norman, 僅供參考，目前沒有在專案中使用")]
    public class AuthorizeActionFilterAttribute : ActionFilterAttribute
    {
        //public override void OnActionExecuting(FilterExecutingContext filterContext)
        //{
        //    HttpSessionStateBase session = filterContext.HttpContext.Session;
        //    Controller controller = filterContext.Controller as Controller;

        //    if (controller != null)
        //    {
        //        if (session["Login"] == null)
        //        {
        //            filterContext.Cancel = true;
        //            controller.HttpContext.Response.Redirect("./Login");
        //        }
        //    }

        //    base.OnActionExecuting(filterContext);
        //}
    }
}