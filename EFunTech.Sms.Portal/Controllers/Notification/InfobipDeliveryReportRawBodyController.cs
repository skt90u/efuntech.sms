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
    /// 檢視 Post Body 資料
    /// </summary>
    public class InfobipDeliveryReportRawBodyController : ApiControllerBase
    {
        public InfobipDeliveryReportRawBodyController(DbContext context, ILogService logService)
            : base(context, logService)
        {

        }

        /// <summary>
        /// https://weblog.west-wind.com/posts/2013/dec/13/accepting-raw-request-body-content-with-aspnet-web-api
        /// http://blog.darkthread.net/post-2015-07-03-streamreader-and-inputstream.aspx
        /// http://stackoverflow.com/questions/10127803/cannot-read-request-content-in-asp-net-webapi-controller
        /// </summary>
        [HttpPost]
        public HttpResponseMessage Post()
        {
            try
            {
                string body = Request.Content.ReadAsStringAsync().Result;

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
