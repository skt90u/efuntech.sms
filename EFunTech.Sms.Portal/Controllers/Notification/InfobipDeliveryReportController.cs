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

namespace EFunTech.Sms.Portal.Controllers
{
    public class InfobipDeliveryReportController : ApiControllerBase
    {
        public InfobipDeliveryReportController(DbContext context, ILogService logService)
            : base(context, logService)
        {

        }

        /// <summary>
        /// https://dev.infobip.com/docs/notify-url
        /// 
        /// https://weblog.west-wind.com/posts/2013/dec/13/accepting-raw-request-body-content-with-aspnet-web-api
        /// </summary>
        //[HttpPost]
        //public HttpResponseMessage Post([FromBody] deliveryreportlist deliveryreportlist)
        //{
        //    try
        //    {
        //        string json = JsonConvert.SerializeObject(deliveryreportlist);

        //        this.logService.Debug("Notification::InfobipDeliveryReportController，Post({0})", json);

        //        return this.Request.CreateResponse(HttpStatusCode.OK, json);
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

        [HttpPost]
        public string PostRawBuffer(string raw)
        {
            return raw;
        }

        public class deliveryreportlist
        {
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
