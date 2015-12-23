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

namespace EFunTech.Sms.Portal.Controllers
{
    public class SendParamSMSModel
    {
        public DateTime? sendTime{get;set;}
        public string subject{get;set;}
        public string content { get; set; }
        public List<UploadedMessageReceiverModel> messageReceivers { get; set; }
    }

    [IdentityBasicAuthentication]
    public class SendParamSMSController : ApiControllerBase
    {
        private SendMessageRuleService sendMessageRuleService;

        public SendParamSMSController(IUnitOfWork unitOfWork, ILogService logService)
            : base(unitOfWork, logService) 
        {
            this.sendMessageRuleService = new SendMessageRuleService(unitOfWork, logService);
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
        ///     http://localhost:20155/api/SendParamSMS
        /// Header:
        ///     Authorization = Basic YWRtaW46MTIzNDU2 (舉例說明)
        ///     Content-Type  = application/json
        ///     TimezoneOffset = "+08:00"
        /// Body:
        ///     {  
        ///        "sendTime":null,
        ///        "subject":"SS",
        ///        "content":"CC",
        ///        "messageReceivers":[  
        ///           {  
        ///              "name":"Andy",
        ///              "mobile":"+86921123456",
        ///              "email":"andy@gmail.com",
        ///              "sendTime":"2045-10-16T11:58:13.0385009+08:00",
        ///              "param1":"P1",
        ///              "param2":"P2",
        ///              "param3":"P3",
        ///              "param4":"P4",
        ///              "param5":"P5",
        ///           },
        ///           {  
        ///              "name":"John",
        ///              "mobile":"+86921654321",
        ///              "email":"john@gmail.com",
        ///              "sendTime":null,
        ///              "param1":"P11",
        ///              "param2":"P22",
        ///              "param3":"P33",
        ///              "param4":"P44",
        ///              "param5":"P55",
        ///           }
        ///        ]
        ///     }
        /// </example>
        [HttpPost]
        public HttpResponseMessage Post([FromBody] SendParamSMSModel sendParamSMSModel)
        {
            try
            {
                this.logService.Debug("BasicAuthApi::SendParamSMSController，Post({0})", JsonConvert.SerializeObject(sendParamSMSModel));

                DateTime? sendTime = sendParamSMSModel.sendTime;
                string subject = sendParamSMSModel.subject;
                string content = sendParamSMSModel.content;
                List<UploadedMessageReceiverModel> messageReceivers = sendParamSMSModel.messageReceivers;

                HttpResponseMessage returnVal = null;

                List<string> ErrorMessages = new List<string>();

                using (TransactionScope scope = this.unitOfWork.CreateTransactionScope())
                {
                    bool isImmediately = !sendTime.HasValue;

                    // Simulate SaveUploadedFile

                    UploadedFile entityUploadedFile = new UploadedFile();
                    entityUploadedFile.FileName = "FakeFileName";
                    entityUploadedFile.FilePath = "FakeFilePath";
                    entityUploadedFile.UploadedFileType = UploadedFileType.SendParamMessage;
                    entityUploadedFile.CreatedUser = CurrentUser;
                    entityUploadedFile.CreatedTime = DateTime.UtcNow;
                    entityUploadedFile = this.unitOfWork.Repository<UploadedFile>().Insert(entityUploadedFile);

                    // Simulate Insert UploadedMessageReceivers

                    for (int i = 0; i < messageReceivers.Count; i++)
                    {
                        var _model = messageReceivers[i];

                        if (string.IsNullOrEmpty(_model.Mobile)) continue;

                        UploadedMessageReceiver entity = new UploadedMessageReceiver();
                        entity.RowNo = i + 1;
                        entity.Name = _model.Name;
                        entity.Mobile = _model.Mobile;
                        entity.Email = _model.Email;

                        if (_model.SendTime.HasValue)
                        {
                            entity.SendTime = Converter.ToUniversalTime(_model.SendTime.Value, ClientTimezoneOffset);
                        }
                        else
                        {
                            entity.SendTime = null;
                        }

                        entity.ClientTimezoneOffset = ClientTimezoneOffset;
                        entity.UseParam = true;
                        entity.Param1 = _model.Param1;
                        entity.Param2 = _model.Param2;
                        entity.Param3 = _model.Param3;
                        entity.Param4 = _model.Param4;
                        entity.Param5 = _model.Param5;
                        entity.CreatedUserId = CurrentUserId;
                        entity.CreatedTime = entityUploadedFile.CreatedTime;
                        entity.UploadedFile = entityUploadedFile;
                        entity.UploadedSessionId = entityUploadedFile.Id;

                        var error = string.Empty;
                        var isValid = this.validationService.Validate(entity, out error);

                        if (!isValid)
                            ErrorMessages.Add(string.Format("{0}: {1}", entity.Mobile, error));

                        // 目前就算驗證不過也沒關係，仍然可以存檔
                        entity = this.unitOfWork.Repository<UploadedMessageReceiver>().Insert(entity);
                    }

                    SendMessageRuleModel model = new SendMessageRuleModel();

                    model.SendTitle = subject;
                    model.SendBody = content;
                    model.RecipientFromType = RecipientFromType.FileUpload;
                    model.RecipientFromFileUpload = new RecipientFromFileUploadModel
                    {
                        UploadedFileId = entityUploadedFile.Id,
                        AddSelfToMessageReceiverList = false,
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
                    model.UseParam = true;
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

                    returnVal = this.Request.CreateResponse(HttpStatusCode.OK, new {
                        result = result,
                        errors = ErrorMessages,
                    });

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
