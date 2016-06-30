using EFunTech.Sms.Portal.Models;
using EFunTech.Sms.Schema;

using JUtilSharp.Database;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using EFunTech.Sms.Portal.Controllers.Common;
using Every8dApi;
using EFunTech.Sms.Portal.Filters;
using BasicAuthentication.Filters;
using EFunTech.Sms.Core;
using System.Data.Entity;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;
using System.Web;

namespace EFunTech.Sms.Portal.Controllers
{
    public class InfobipDeliveryReportController : ApiControllerBase
    {
        public InfobipDeliveryReportController(DbContext context, ILogService logService)
            : base(context, logService)
        {

        }

        #region 檢視 Post Body 資料
        /// <summary>
        /// https://dev.infobip.com/docs/notify-url
        /// https://weblog.west-wind.com/posts/2013/dec/13/accepting-raw-request-body-content-with-aspnet-web-api
        /// http://blog.darkthread.net/post-2015-07-03-streamreader-and-inputstream.aspx
        /// http://stackoverflow.com/questions/10127803/cannot-read-request-content-in-asp-net-webapi-controller
        /// </summary>
        //[HttpPost]
        //public HttpResponseMessage Post()
        //{
        //    try
        //    {
        //        string body = Request.Content.ReadAsStringAsync().Result;

        //        logService.Debug("{0}", body);

        //        return this.Request.CreateResponse(HttpStatusCode.OK, body);
        //    }
        //    catch (Exception ex)
        //    {
        //        this.logService.Error(ex);

        //        return this.Request.CreateResponse(HttpStatusCode.InternalServerError, new
        //        {
        //            ErrorMessage = ex.Message,
        //        });
        //    }
        //}
        #endregion

        public class deliveryInfoNotification
        {
            public deliveryInfo deliveryInfo { get; set; }
            public string callbackData { get; set; }
        }

        public class deliveryInfo
        {
            public string address { get; set; }
            public string messageId { get; set; }
            public string deliveryStatus { get; set; }
            public string clientCorrelator { get; set; }
            public decimal price { get; set; }

        }

        /// <summary>
        /// 20160701 測試好像跑不到這裡，可能要用 Post() <--- 沒有參數的版本
        /// </summary>
        /// <param name="deliveryInfoNotification"></param>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage Post(deliveryInfoNotification deliveryInfoNotification)
        {
            //{  
            //   "deliveryInfoNotification":{  
            //      "deliveryInfo":{  
            //         "address":"886921859698", // 發送門號
            //         "messageId":"1673019360160545114",
            //         "deliveryStatus":"DeliveredToTerminal", // 狀態
            //         "clientCorrelator":"1673019360160545112", // sendMessageResult.ClientCorrelator
            //         "price":61.9048
            //      },
            //      "callbackData":"I_AM_CallbackData"
            //   }
            //}

            try
            {
                var body = JsonConvert.SerializeObject(deliveryInfoNotification);

                logService.Debug("{0}", body);

                return this.Request.CreateResponse(HttpStatusCode.OK, body);
            }
            catch (Exception ex)
            {
                this.logService.Error(ex);

                return this.Request.CreateResponse(HttpStatusCode.InternalServerError, new
                {
                    ErrorMessage = ex.Message,
                });
            }
        }
    }
}
