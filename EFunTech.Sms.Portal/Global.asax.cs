using EFunTech.Sms.Portal.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace EFunTech.Sms.Portal
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            System.Data.Entity.Database.SetInitializer(new ApplicationDbContextInitializer());

            // 如何強迫 .Net 應用程式輸出英文的例外訊息

            ////////////////////////////////////////
            // Remove extra View Engines，加速網站
            ////////////////////////////////////////

            // Removing all the view engines
            ViewEngines.Engines.Clear();
            //Add Razor Engine (which we are using)
            ViewEngines.Engines.Add(new RazorViewEngine());

            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes); // 20150928 Norman, 將 RouteConfig.RegisterRoutes(RouteTable.Routes); 放在 ConfigureDatabase 之後才設定 似乎有時候會出問題
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            
            //啟動 BackgroundJob
            //HangfireBootstrapper.Instance.Start();

            // http://stackoverflow.com/questions/14374656/mvc-bundle-client-caching
            //this.EndRequest += MvcApplication_EndRequest;
        }

        //void MvcApplication_EndRequest(object sender, EventArgs e)
        //{
        //    var request = this.Request;
        //    var response = this.Response;

        //    if (request.RawUrl.Contains("Content/"))
        //    {
        //        response.Cache.SetCacheability(HttpCacheability.NoCache);
        //    }
        //}

        protected void Application_End(object sender, EventArgs e)
        {
            //停止 BackgroundJob
            HangfireBootstrapper.Instance.Stop();
        }
        /*
        protected void Application_Error(object sender, EventArgs e)
        {
            // TODO: 檢查 加入 Application_Error 是否會導致 elmah 失效
         
            //return;
            var exception = Server.GetLastError();
            // Log the exception.
            Response.Clear();

            var httpException = exception as HttpException;

            var routeData = new RouteData();
            routeData.Values.Add("controller", "Error");

            if (httpException == null)
            {
                routeData.Values.Add("action", "Index");
            }
            else //It's an Http Exception, Let's handle it.
            {
                switch (httpException.GetHttpCode())
                {
                    case 404:
                        // Page not found.
                        routeData.Values.Add("action", "HttpError404");
                        break;
                    case 505:
                        // Server error.
                        routeData.Values.Add("action", "HttpError505");
                        break;

                    // Here you can handle Views to other error codes.
                    // I choose a General error template  
                    default:
                        routeData.Values.Add("action", "Index");
                        break;
                }
            }

            // Pass exception details to the target error View.
            routeData.Values.Add("error", exception);

            // Clear the error on server.
            Server.ClearError();

            // Call target Controller and pass the routeData.
            IController errorController = new ErrorController();
            errorController.Execute(new RequestContext(
                 new HttpContextWrapper(Context), routeData));

            //var factory = ControllerBuilder.Current.GetControllerFactory();
            //var controller = factory.CreateController()

            // http://stackoverflow.com/questions/1171035/asp-net-mvc-custom-error-handling-application-error-global-asax

            //HttpContext.Current.ClearError();
            //Response.Redirect("~/Error.aspx", false);
            //return;

            //HttpContext.Current.Server.ClearError();
            //HttpContext.Current.Response.Redirect("~/ErrorPage.aspx");

            // it's working
            //HttpContext.Current.Server.ClearError();
            //HttpContext.Current.Response.Redirect("~/Error/HttpError404");

            //HttpContext ctx = HttpContext.Current;
            //ctx.Response.Clear();
            //RequestContext rc = ((MvcHandler)ctx.CurrentHandler).RequestContext;
            //rc.RouteData.Values["action"] = "HttpError404";
            //rc.RouteData.Values["controller"] = "Error";

            
            //rc.RouteData.Values["newActionName"] = "WrongRequest";

            
            //IControllerFactory factory = ControllerBuilder.Current.GetControllerFactory();
            //IController controller = factory.CreateController(rc, "Error");
            //controller.Execute(rc);
            //ctx.Server.ClearError();

            //HttpContext.Current.Response.RedirectToRoute(...)
        }
        */
    }
}
