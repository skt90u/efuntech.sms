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


        [HttpPost]
        public HttpResponseMessage Post(deliveryreport deliveryreport)
        {
            try
            {
                var body = JsonConvert.SerializeObject(deliveryreport);

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

        public class deliveryreport
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
            public deliveryInfoNotification deliveryInfoNotification { get; set; }
        }

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


    }
}
