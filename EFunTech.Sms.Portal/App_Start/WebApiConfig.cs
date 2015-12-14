using Newtonsoft.Json.Converters;
using System.Web.Http;
using System.Web.Http.Cors;
using System;
using System.Web.Http.Controllers;
using System.Web.Http.ModelBinding;
using JUtilSharp.Database;
using EFunTech.Sms.Schema;

namespace EFunTech.Sms.Portal
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // http://enable-cors.org/server_aspnet.html
            // https://www.nuget.org/packages/Microsoft.AspNet.WebApi.Cors
            // http://www.dotblogs.com.tw/kinanson/archive/2015/03/27/150850.aspx
            var corsAttr = new EnableCorsAttribute("*", "*", "*");
            config.EnableCors(corsAttr);

            //http://localhost:7333/index.html
            //config.EnableCors();


            // http://benfoster.io/blog/aspnet-web-api-compression

            // https://github.com/azzlack/Microsoft.AspNet.WebApi.MessageHandlers.Compression
            // https://www.nuget.org/packages/Microsoft.AspNet.WebApi.MessageHandlers.Compression/
            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();

            // TODO: GZip 壓縮會導致 angularjs $http.get 執行失敗
            // 方法一
            //config.MessageHandlers.Insert(0, new ServerCompressionHandler(new GZipCompressor(), new DeflateCompressor()));

            config.Formatters.JsonFormatter.SerializerSettings.Converters.Add(
                new IsoDateTimeConverter() { DateTimeFormat = "yyyy-MM-dd HH:mm:ss" });

            //config.Formatters.XmlFormatter.UseXmlSerializer = true;

            // 20151024 Norman, 根據 Dino 要求，將外部 api 獨立出來
            config.Routes.MapHttpRoute(
                name: "balance",
                routeTemplate: "docs/balance/{id}",
                defaults: new { controller = "balance", id = RouteParameter.Optional }
            );
            config.Routes.MapHttpRoute(
                name: "erasebooking",
                routeTemplate: "docs/erasebooking/{id}",
                defaults: new { controller = "erasebooking", id = RouteParameter.Optional }
            );
            config.Routes.MapHttpRoute(
                name: "report",
                routeTemplate: "docs/report/{id}",
                defaults: new { controller = "report", id = RouteParameter.Optional }
            );
            config.Routes.MapHttpRoute(
                name: "sendparamsms",
                routeTemplate: "docs/sendparamsms/{id}",
                defaults: new { controller = "sendparamsms", id = RouteParameter.Optional }
            );
            config.Routes.MapHttpRoute(
                name: "sendsms",
                routeTemplate: "docs/sendsms/{id}",
                defaults: new { controller = "sendsms", id = RouteParameter.Optional }
            );

            // 預設值要放在最後
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            // 20151014 Norman, 不清楚為什麼 "HttpGet" 取到的DateTime為LocalTime, HttpPost為UtcTime
            // 以下網頁似乎在敘述跟我一樣的問題http://www.28im.com/csharp/a4040263.html
            //
            // 解決方式：將所有 ISO 8601 格式的 DateTime 都轉換成 UtcTime
            // https://github.com/Elders/Pandora/blob/master/src/Elders.Pandora.UI/App_Start/WebApiConfig.cs
            config.BindParameter(typeof(DateTime?), new UTCDateTimeModelBinder());
            config.BindParameter(typeof(DateTime), new UTCDateTimeModelBinder());
        }

        public class UTCDateTimeModelBinder : IModelBinder
        {
            public bool BindModel(HttpActionContext actionContext, ModelBindingContext bindingContext)
            {
                var value = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);

                // Check if the DateTime property being parsed is not null or "" (for JSONO
                if (value != null && value.AttemptedValue != null && value.AttemptedValue != "")
                {
                    // 1. value.AttemptedValue 是 ISO 8601 格式
                    // 2. (我猜測) DateTime.Parse 根據會轉成 Server TimeZone 轉換成 Server 端的本地時間
                    var localTimeInServer = DateTime.Parse(value.AttemptedValue);

                    // utcTime1, utcTime2 兩者應該是一樣是UTC
                    //var utcTime1 = DateTime.FromFileTimeUtc(localTimeInServer.ToFileTimeUtc());
                    //var utcTime2 = utcTime.ToUniversalTime();

                    var utcTime = DateTime.FromFileTimeUtc(localTimeInServer.ToFileTimeUtc());
                    bindingContext.Model = utcTime;

                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
    }
}
