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
    /// <summary>
    /// Infobip Document Demo, But Not Actually Correct Format
    /// 
    /// https://dev.infobip.com/docs/notify-url
    /// </summary>
    public class InfobipDeliveryReportSampleController : ApiControllerBase
    {
        public InfobipDeliveryReportSampleController(DbContext context, ILogService logService): base(context, logService){}

        [HttpPost]
        public HttpResponseMessage Post([FromBody] deliveryreportlist deliveryreportlist)
        {
            try
            {
                string body = JsonConvert.SerializeObject(deliveryreportlist);

                this.logService.Debug("{0}", body);

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

        public class deliveryreportlist
        {
            //{
            //  "results": [
            //    {
            //      "bulkId": "BULK-ID-123-xyz",
            //      "messageId": "c9823180-94d4-4ea0-9bf3-ec907e7534a6",
            //      "to": "41793026731",
            //      "sentAt": "2015-06-04T13:01:52.933+0000",
            //      "doneAt": "2015-06-04T13:02:00.134+0000",
            //      "smsCount": 1,
            //      "price": {
            //        "pricePerMessage": 0.0001000000,
            //        "currency": "EUR"
            //      },
            //      "status": {
            //        "groupId": 3,
            //        "groupName": "DELIVERED",
            //        "id": 5,
            //        "name": "DELIVERED_TO_HANDSET",
            //        "description": "Message delivered to handset"
            //      },
            //      "error": {
            //        "groupId": 0,
            //        "groupName": "OK",
            //        "id": 0,
            //        "name": "NO_ERROR",
            //        "description": "No Error",
            //        "permanent": false
            //      },
            //      "callbackData":"There's no place like home."
            //    },
            //    {
            //      "bulkId": "BULK-ID-123-xyz",
            //      "messageId": "MESSAGE-ID-123-xyz",
            //      "to": "41793026727",
            //      "sentAt": "2015-06-04T13:01:52.937+0000",
            //      "doneAt": "2015-06-04T13:02:01.204+0000",
            //      "smsCount": 1,
            //      "price": {
            //        "pricePerMessage": 0.0001000000,
            //        "currency": "EUR"
            //      },
            //      "status": {
            //        "groupId": 3,
            //        "groupName": "DELIVERED",
            //        "id": 5,
            //        "name": "DELIVERED_TO_HANDSET",
            //        "description": "Message delivered to handset"
            //      },
            //      "error": {
            //        "groupId": 0,
            //        "groupName": "OK",
            //        "id": 0,
            //        "name": "NO_ERROR",
            //        "description": "No Error",
            //        "permanent": false
            //      },
            //      "callbackData":"There's no place like home."
            //    }
            //  ]
            //}
            public deliveryreport[] results { get; set; }
        }
        public class deliveryreport
        {
            public string bulkId { get; set; }
            public string messageId { get; set; }
            public string to { get; set; }
            public string sentAt { get; set; }
            public string doneAt { get; set; }
            public int smsCount { get; set; }

            public price price { get; set; }
            public status status { get; set; }
            public error error { get; set; }
            public string callbackData { get; set; }
        }

        public class price
        {
            public decimal pricePerMessage { get; set; }
            public string EUR { get; set; }
        }

        public class status
        {
            public int groupId { get; set; }
            public string groupName { get; set; }
            public int id { get; set; }
            public string name { get; set; }
            public string description { get; set; }
        }

        public class error
        {
            public int groupId { get; set; }
            public string groupName { get; set; }
            public int id { get; set; }
            public string name { get; set; }
            public string description { get; set; }
            public bool permanent { get; set; }
        }
    }
}
