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

namespace EFunTech.Sms.Portal.Controllers
{
    [IdentityBasicAuthentication]
    public class BalanceController : ApiControllerBase
    {
        private SendMessageRuleService sendMessageRuleService;

        public BalanceController(DbContext context, ILogService logService)
            : base(context, logService)
        {
            this.sendMessageRuleService = new SendMessageRuleService(new UnitOfWork(context), logService);
        }

        /// <summary>
        /// (06) 餘額查詢
        /// </summary>
        /// <example>
        /// Method:
        ///     Get
        /// Url:
        ///     http://localhost:20155/api/Balance
        /// Header:
        ///     Authorization = Basic YWRtaW46MTIzNDU2 (舉例說明)
        /// </example>
        [HttpGet]
        public async Task<HttpResponseMessage> Get()
        //public HttpResponseMessage Get()
        {
            try
            {
                this.logService.Debug("BasicAuthApi::BalanceController，Get()");

                return this.Request.CreateResponse(HttpStatusCode.OK, new
                {
                    balance = this.CurrentUser.SmsBalance // 目前使用者餘額
                });
            }
            catch(Exception ex)
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
