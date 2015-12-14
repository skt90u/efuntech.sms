using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using EFunTech.Sms.Schema;
using System.Web.Mvc;
using Microsoft.Practices.Unity;
using JUtilSharp.Database;

namespace EFunTech.Sms.Portal.Filters
{
    // http://stackoverflow.com/questions/9298466/can-i-return-custom-error-from-jsonresult-to-jquery-ajax-error-method
    public class JsonExceptionFilterAttribute : FilterAttribute, IExceptionFilter // System.Web.Mvc.AuthorizeAttribute
    {
        public void OnException(ExceptionContext filterContext)
        {
            if (filterContext.RequestContext.HttpContext.Request.IsAjaxRequest())
            {
                filterContext.HttpContext.Response.StatusCode = 500;
                filterContext.ExceptionHandled = true;
                filterContext.Result = new JsonResult
                {
                    Data = new
                    {
                        // obviously here you could include whatever information you want about the exception
                        // for example if you have some custom exceptions you could test
                        // the type of the actual exception and extract additional data
                        // For the sake of simplicity let's suppose that we want to
                        // send only the exception message to the client
                        //errorMessage = filterContext.Exception.Message
                        ExceptionMessage = filterContext.Exception.Message
                    },
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                };
            }
        }
    }
}