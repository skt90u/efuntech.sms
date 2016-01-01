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
using System.Transactions;
using System.Data.Entity;
using System.Threading.Tasks;

namespace EFunTech.Sms.Portal.Controllers
{
    [IdentityBasicAuthentication]
    public class EraseBookingController : ApiControllerBase
    {
        private SendMessageRuleService sendMessageRuleService;

        public EraseBookingController(DbContext context, ILogService logService)
            : base(context, logService)
        {
            this.sendMessageRuleService = new SendMessageRuleService(new UnitOfWork(context), logService);
        }

        /// <summary>
        /// 刪除預約簡訊
        /// </summary>
        /// <param name="sendMessageRuleId">要刪除的預約簡訊編號</param>
        /// <returns>
        /// totalReceiverCount: 刪除的筆數
        /// totalMessageCost: 回補的點數
        /// </returns>
        /// <example>
        /// Method:
        ///     Delete
        /// Url:
        ///     http://localhost:20155/api/EraseBooking
        /// Header:
        ///     Authorization = Basic YWRtaW46MTIzNDU2 (舉例說明)
        ///     Content-Type  = application/json
        /// Body:
        ///     {"sendTime":"2015-10-16T10:34:27.8680576+08:00","subject":"S","content":"C","mobiles":["0921859698","0921859698","0921859698","0921859698"]}
        /// </example>
        [HttpDelete]
        public HttpResponseMessage Delete(int id)
        //public HttpResponseMessage Delete(int id)
        {
            try
            {
                this.logService.Debug("BasicAuthApi::EraseBookingController，Delete({0})", id);

                int sendMessageRuleId = id;

                HttpResponseMessage returnVal = null;

                using (TransactionScope scope = context.CreateTransactionScope())
                {
                    //int sendMessageRuleId = id;

                    SendMessageRule sendMessageRule = CurrentUser.SendMessageRules.Where(p => p.Id == sendMessageRuleId).FirstOrDefault();

                    if (sendMessageRule == null)
                        throw new Exception(string.Format("無法刪除預約簡訊，目前使用者 {0} 沒有對應的簡訊識別碼 {1}",
                            CurrentUserName,
                            sendMessageRuleId));

                    this.sendMessageRuleService.RemoveSendMessageRule(CurrentUser, sendMessageRuleId);

                    var result = new
                    {
                        smsCount = sendMessageRule.TotalReceiverCount, // 刪除的筆數
                        price = sendMessageRule.TotalMessageCost, // 回補的點數
                    };

                    returnVal = this.Request.CreateResponse(HttpStatusCode.OK, result);

                    scope.Complete();
                }

                return returnVal;
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
