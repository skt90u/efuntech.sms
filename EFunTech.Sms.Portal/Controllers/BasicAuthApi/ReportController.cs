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

namespace EFunTech.Sms.Portal.Controllers
{
    [IdentityBasicAuthentication]
    public class ReportController : ApiControllerBase
    {
        private SendMessageRuleService sendMessageRuleService;

        public ReportController(IUnitOfWork unitOfWork, ILogService logService)
            : base(unitOfWork, logService) 
        {
            this.sendMessageRuleService = new SendMessageRuleService(unitOfWork, logService);
        }

        /// <summary>
        /// (05) 發送狀態查詢
        /// </summary>
        /// <example>
        /// Method:
        ///     Get
        /// Url:
        ///     http://localhost:20155/api/DeliveryStatus
        /// Header:
        ///     Authorization = Basic YWRtaW46MTIzNDU2 (舉例說明)
        /// </example>
        [HttpGet]
        public HttpResponseMessage Get(int id)
        {
            try
            {
                this.logService.Debug("BasicAuthApi::ReportController，Get({0})", id);

                int sendMessageRuleId = id;

                HttpResponseMessage returnVal = null;

                SendMessageRule sendMessageRule = CurrentUser.SendMessageRules.Where(p => p.Id == sendMessageRuleId).FirstOrDefault();

                if (sendMessageRule == null)
                    throw new Exception(string.Format("無法取得簡訊派送報表，目前使用者 {0} 沒有對應的簡訊識別碼 {1}",
                        CurrentUserName,
                        sendMessageRuleId));

                var sendMessageHistoryRepository = this.unitOfWork.Repository<SendMessageHistory>();

                var result = sendMessageHistoryRepository.GetMany(p => p.SendMessageRuleId == sendMessageRuleId).Select(p => new
                {
                    sendTime = p.SendTime,
                    sendTitle = p.SendTitle,
                    sendBody = p.SendBody,
                    messageStatus = p.MessageStatusString,
                    destinationAddress = p.DestinationAddress,

                    createdTime = p.SendMessageResultCreatedTime,
                    sentDate = p.SentDate,
                    doneDate = p.DoneDate,
                    deliveryStatus = p.DeliveryStatusString,
                    price = p.MessageCost,

                    delivered = p.Delivered,
                    name = p.DestinationName,
                    region = p.Region,
                });

                returnVal = this.Request.CreateResponse(HttpStatusCode.OK, result);

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
