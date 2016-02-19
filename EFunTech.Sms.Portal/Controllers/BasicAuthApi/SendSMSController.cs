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
using Newtonsoft.Json;
using System.Data.Entity;
using System.Threading.Tasks;

namespace EFunTech.Sms.Portal.Controllers
{
    public class SendSMSModel
    {
        public DateTime? sendTime{get;set;}
        public string subject{get;set;}
        public string content{get;set;}
        public string[] mobiles{get;set;}
    }

    [IdentityBasicAuthentication]
    public class SendSMSController : ApiControllerBase
    {
        private SendMessageRuleService sendMessageRuleService;

        public SendSMSController(DbContext context, ILogService logService)
            : base(context, logService)
        {
            this.sendMessageRuleService = new SendMessageRuleService(new UnitOfWork(context), logService);
        }

        /// <summary>
        /// Sends the SMS.
        /// </summary>
        /// <param name="sendTime">
        /// 簡訊預定發送時間
        ///     -立即發送：if sendTime = null
        ///     -預約發送：請傳入計時間，若小於系統接單將不予，若傳遞時間已逾現在之，將立即發送。
        /// </param>
        /// <param name="subject">簡訊類別描述，發送紀錄查詢時參考用，可不填</param>
        /// <param name="content">簡訊發送 內容</param>
        /// <param name="mobile">格式為 : +886912345678，多筆接收人時，請以逗點隔開</param>
        /// <returns>
        /// remainingSmsBalance: 發送後剩餘點數
        /// totalReceiverCount: 發送通數
        /// totalMessageCost: 本次發送扣除點數
        /// sendMessageRuleId: 發送識別碼
        /// </returns>
        /// <example>
        /// Method:
        ///     Post
        /// Url:
        ///     http://localhost:20155/api/SendSMS
        /// Header:
        ///     Authorization = Basic YWRtaW46MTIzNDU2 (舉例說明)
        ///     Content-Type  = application/json
        ///     TimezoneOffset = "+08:00"
        /// Body:
        ///     {"sendTime":"2015-10-16T10:34:27.8680576+08:00","subject":"S","content":"C","mobiles":["0921859698","0921859698","0921859698","0921859698"]}
        /// </example>
        [HttpPost]
        public HttpResponseMessage Post([FromBody] SendSMSModel sendSMSModel)
        //public HttpResponseMessage Post([FromBody] SendSMSModel sendSMSModel)
        {
            try
            {
                this.logService.Debug("BasicAuthApi::SendSMSController，Post({0})", JsonConvert.SerializeObject(sendSMSModel));

                DateTime? sendTime = sendSMSModel.sendTime;
                string subject = sendSMSModel.subject;
                string content = sendSMSModel.content;
                string[] mobiles = sendSMSModel.mobiles;

                HttpResponseMessage returnVal = null;

                using (TransactionScope scope = context.CreateTransactionScope())
                {
                    bool isImmediately = !sendTime.HasValue;

                    var model = new SendMessageRuleModel();

                    model.SendTitle = subject;
                    model.SendBody = content;
                    model.RecipientFromType = RecipientFromType.ManualInput;
                    model.RecipientFromManualInput = new RecipientFromManualInputModel
                    {
                        PhoneNumbers = string.Join(",", mobiles)
                    };
                    model.SendTimeType = isImmediately ? SendTimeType.Immediately : SendTimeType.Deliver;
                    model.SendDeliver = isImmediately
                        ? null
                        : new SendDeliverModel
                        {
                            SendTime = sendTime.Value,
                            ClientTimezoneOffset = ClientTimezoneOffset,
                        };
                    model.SendCycleEveryDay = null;
                    model.SendCycleEveryWeek = null;
                    model.SendCycleEveryMonth = null;
                    model.SendCycleEveryYear = null;
                    model.SendCustType = SendCustType.OneWay;
                    model.UseParam = false;
                    model.SendMessageType = SendMessageType.SmsMessage;
                    model.TotalReceiverCount = 0;
                    model.TotalMessageCost = 0;
                    model.RemainingSmsBalance = 0;
                    model.CreatedTime = DateTime.UtcNow;

                    model.UpdateClientTimezoneOffset(ClientTimezoneOffset);

                    var rules = this.sendMessageRuleService.CreateSendMessageRuleFromWeb(CurrentUser, model);

                    var result = rules.Select(p => new
                    {
                        balance = p.RemainingSmsBalance, // 發送後剩餘點數
                        smsCount = p.TotalReceiverCount, // 發送通數
                        price = p.TotalMessageCost, // 本次發送扣除點數
                        bulkId = p.Id, // 發送識別碼
                    });

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
