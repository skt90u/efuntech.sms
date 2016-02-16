using EFunTech.Sms.Schema;
using JUtilSharp.Database;
using Microsoft.Practices.ServiceLocation;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Every8dApi;
using EFunTech.Sms.Core;
using Hangfire;

namespace EFunTech.Sms.Portal
{
    public class Every8dSmsProvider : ISmsProvider
    {
        private string userName;
        private string password;

        private ISystemParameters systemParameters;
        private ILogService logService;
        private IUnitOfWork unitOfWork;
        private SendMessageStatisticService sendMessageStatisticService;

        private TradeService tradeService;
        private SmsProviderType smsProviderType;

        public Every8dSmsProvider(ISystemParameters systemParameters, ILogService logService, IUnitOfWork unitOfWork, SmsProviderType smsProviderType)
        {
            this.userName = systemParameters.Every8dUserName;
            this.password = systemParameters.Every8dPassword;

            this.systemParameters = systemParameters;
            this.logService = logService;
            this.unitOfWork = unitOfWork;
            this.smsProviderType = smsProviderType;

            this.sendMessageStatisticService = new SendMessageStatisticService(logService, unitOfWork);

            this.tradeService = new TradeService(unitOfWork, logService);
        }

        public string Name
        {
            get { return smsProviderType.ToString(); }
        }

        public bool IsAvailable
        {
            get
            {
                try
                {
                    // we use get balance to determine the provider is available or not
                    var tmp = this.Balance;
                    return true;
                }
                catch (Exception ex)
                {
                    this.logService.Error(ex);
                    return false;
                }
            }
        }

        public decimal Balance
        {
            get
            {
                using (var smsClient = new SMSClient(userName, password))
                {
                    return (decimal)smsClient.GetCredit();
                }
            }
        }

        public decimal ToProviderBalance(decimal efuntechBalance)
        {
            // TODO: 針對不同簡訊提供商，將 totalMessageCost 轉換成他們對應的點數單位
            return efuntechBalance;
        }

        public void SendSMS(int sendMessageQueueId)
        {
            var sendMessageQueue = this.unitOfWork.Repository<SendMessageQueue>().GetById(sendMessageQueueId);

            using (var smsClient = new SMSClient(userName, password))
            {
                DateTime? sendTime = null; // NULL 表示立刻發送
                string subject = sendMessageQueue.SendTitle;
                string message = sendMessageQueue.SendBody;

                var messageReceiver = this.unitOfWork.Repository<MessageReceiver>().GetMany(p =>
                    p.SendMessageRuleId == sendMessageQueue.SendMessageRuleId &&
                    p.SendBody == message)
                    .ToList();

                var every8dMessageReceiver = messageReceiver
                    .Select(p => new Every8d_MessageReceiver
                {
                    NAME = p.Name,
                    MOBILE = p.E164Mobile,
                    EMAIL = string.Empty, // p.Email, // 不要依賴 Every8d 寄送 Email
                    SENDTIME = null,
                    CONTENT = message,
                }).ToList();

                this.logService.Debug("Every8dSmsProvider，發送簡訊(簡訊編號：{0}，簡訊序列編號：{1}，發送內容：{2}，發送名單：[{3}])",
                    sendMessageQueue.SendMessageRuleId,
                    sendMessageQueue.Id,
                    message,
                    string.Join("、", every8dMessageReceiver.Select(p => p.MOBILE)));

                SEND_SMS_RESULT sendMessageResult = smsClient.SendParamSMS(sendTime, subject, every8dMessageReceiver);

                this.logService.Debug("Every8dSmsProvider，發送簡訊(簡訊編號：{0}，簡訊序列編號：{1}，回傳簡訊發送識別碼：{2}，回傳結果：{3})",
                    sendMessageQueue.SendMessageRuleId,
                    sendMessageQueue.Id,
                    sendMessageResult.BATCH_ID,
                    sendMessageResult.ToString());

                // Send Email
                string body = message;
                string[] emails = messageReceiver.Where(p => !string.IsNullOrEmpty(p.Email)).Select(p => p.Email).ToArray();
                if (emails.Any())
                {
                    this.logService.Debug("Every8dSmsProvider，發送Email(簡訊編號：{0}，簡訊序列編號：{1}，主旨：{2}，內容：{3}，發送名單：[{4}])",
                        sendMessageQueue.SendMessageRuleId,
                        sendMessageQueue.Id,
                        subject,
                        body,
                        string.Join("、", emails));

                    BackgroundJob.Enqueue<CommonMailService>(x => x.Send(subject, body, emails));
                }
                else
                {
                    this.logService.Debug("Every8dSmsProvider，無須發送Email(簡訊編號：{0}，簡訊序列編號：{1})",
                                            sendMessageQueue.SendMessageRuleId,
                                            sendMessageQueue.Id);
                }

                UpdateDb(sendMessageQueue, sendMessageResult);
            }
        }

        private void UpdateDb(SendMessageQueue sendMessageQueue, SEND_SMS_RESULT sendMessageResult)
        {
            // 寫入對應的 SendMessageResult

            Every8d_SendMessageResult every8d_SendMessageResult = new Every8d_SendMessageResult();

            every8d_SendMessageResult.SendMessageQueueId = sendMessageQueue.Id;

            every8d_SendMessageResult.SendTime = sendMessageQueue.SendTime;
            every8d_SendMessageResult.Subject = sendMessageQueue.SendTitle;
            every8d_SendMessageResult.Content = sendMessageQueue.SendBody;
            every8d_SendMessageResult.CreatedTime = DateTime.UtcNow;

            every8d_SendMessageResult.CREDIT = sendMessageResult.CREDIT;
            every8d_SendMessageResult.SENDED = sendMessageResult.SENDED;
            every8d_SendMessageResult.COST = sendMessageResult.COST;
            every8d_SendMessageResult.UNSEND = sendMessageResult.UNSEND;
            every8d_SendMessageResult.BATCH_ID = sendMessageResult.BATCH_ID;

            every8d_SendMessageResult = this.unitOfWork.Repository<Every8d_SendMessageResult>().Insert(every8d_SendMessageResult);

            // Every8d 在此階段無法取得寫入 SendMessageHistory，無對應資料結構
            // CreateSendMessageHistory(sendMessageQueue.Id);

            // 在 Thread 中等待 30 秒，再寫入 DeliveryReportQueue
            var delayMilliseconds = (int)30 * 1000;
            FaFTaskFactory.StartNew(delayMilliseconds, () =>
            {
                using (var context = new ApplicationDbContext())
                {
                    var _unitOfWork = new UnitOfWork(context);
                    var _repository = _unitOfWork.Repository<DeliveryReportQueue>();

                    // 寫入簡訊派送結果等待取回序列
                    DeliveryReportQueue deliveryReportQueue = new DeliveryReportQueue();
                    deliveryReportQueue.SendMessageQueueId = sendMessageQueue.Id;
                    deliveryReportQueue.RequestId = every8d_SendMessageResult.BATCH_ID;
                    deliveryReportQueue.ProviderName = this.Name;
                    deliveryReportQueue.CreatedTime = DateTime.UtcNow;
                    deliveryReportQueue.SendMessageResultItemCount = every8d_SendMessageResult.SENDED.HasValue ? every8d_SendMessageResult.SENDED.Value : 0;
                    deliveryReportQueue.DeliveryReportCount = 0;
                    deliveryReportQueue = _repository.Insert(deliveryReportQueue);
                }
            });
        }


        public void GetDeliveryReport(string requestId)
        {
            using (var smsClient = new SMSClient(userName, password))
            {
                SMS_LOG SMS_LOG = smsClient.GetDeliveryStatus(requestId, string.Empty);

                this.logService.Debug("Every8dSmsProvider，接收簡訊發送結果(簡訊發送識別碼：{0}，發送結果：{1})",
                requestId,
                SMS_LOG.ToString());

                UpdateDb(requestId, SMS_LOG);
            }
        }

        private void UpdateDb(string requestId, SMS_LOG SMS_LOG)
        {
            //連線狀態代碼，各代碼說明請參閱下表。 
            //    0： 取得連線成功 
            // -100： 取得連線失敗，無此帳號 
            // -101： 取得連線失敗，密碼錯誤 
            // -999： 帳號已封鎖，請洽服務窗口。認證錯誤超過 10 次將封鎖此服務。 

            if (SMS_LOG.CODE != 0) return;
            if (SMS_LOG.GET_DELIVERY_STATUS.SMS_LIST.Count == 0) return;

            
            // (3) 將 DeliveryReportList.DeliveryReports 塞入對應資料表 Infobip_DeliveryReport

            Every8d_SendMessageResult SendMessageResult = this.unitOfWork.Repository<Every8d_SendMessageResult>().Get(p => p.BATCH_ID == requestId);
            // if (SendMessageResult == null) return; 不應該為 null

            int CODE = SMS_LOG.CODE;
            string DESCRIPTION = SMS_LOG.DESCRIPTION;

            foreach (var SMS_CONTENT in SMS_LOG.GET_DELIVERY_STATUS.SMS_LIST)
            {
                Every8d_DeliveryReport entity = new Every8d_DeliveryReport();
                entity.RequestId = requestId;
                entity.CODE = CODE;
                entity.DESCRIPTION = DESCRIPTION;
                entity.NAME = SMS_CONTENT.NAME;
                entity.MOBILE = SMS_CONTENT.MOBILE;
                entity.SENT_TIME = SMS_CONTENT.SENT_TIME;
                entity.COST = SMS_CONTENT.COST;
                entity.STATUS = SMS_CONTENT.STATUS;
                entity.CreatedTime = DateTime.UtcNow;
                entity.SendMessageResult = SendMessageResult;

                entity = this.unitOfWork.Repository<Every8d_DeliveryReport>().Insert(entity);
            }

            Every8d_SendMessageResult every8d_SendMessageResult = this.unitOfWork.Repository<Every8d_SendMessageResult>().Get(p => p.BATCH_ID == requestId);
            if (every8d_SendMessageResult == null)
                throw new Exception(string.Format("Every8dSmsProvider，無法取得 Infobip_SendMessageResult(BATCH_ID：{0})", requestId));

            // (4) 如果所有派送結果都取回了，就在 DeliveryReportQueue (待查詢簡訊發送結果序列) 刪除對應資料
            var deliveryReportQueueRepository = this.unitOfWork.Repository<DeliveryReportQueue>();
            var deliveryReportQueue = deliveryReportQueueRepository.Get(p => p.RequestId == requestId && p.ProviderName == this.Name);
            if (deliveryReportQueue != null)
            {
                int SendMessageResultItemCount = every8d_SendMessageResult.SENDED.HasValue ? every8d_SendMessageResult.SENDED.Value : 0;
                int DeliveryReportCount = this.unitOfWork.Repository<Every8d_DeliveryReport>().Count(p => p.RequestId == requestId);

                deliveryReportQueue.SendMessageResultItemCount = SendMessageResultItemCount;
                deliveryReportQueue.DeliveryReportCount = DeliveryReportCount;
                deliveryReportQueueRepository.Update(deliveryReportQueue);

                // 20151111 Norman, 暫時不刪除，用以除錯
                //if (DeliveryReportCount >= SendMessageResultItemCount)
                //{
                //    deliveryReportQueueRepository.Delete(p => p.RequestId == requestId);
                //}
            }

            int sendMessageQueueId = SendMessageResult.SendMessageQueueId;
            // (5) 出大表，SendMessageHistorys
            UpdateSendMessageHistory(sendMessageQueueId);
        }

        /// <summary>
        /// 在 GetDeliveryReport 之後，更新大表 SendMessageHistory
        /// </summary>
        public void UpdateSendMessageHistory(int sendMessageQueueId)
        {
            var sendMessageRuleRepository = this.unitOfWork.Repository<SendMessageRule>();
            var sendMessageQueueRepository = this.unitOfWork.Repository<SendMessageQueue>();
            var sendMessageHistoryRepository = this.unitOfWork.Repository<SendMessageHistory>();

            SendMessageQueue sendMessageQueue = sendMessageQueueRepository.GetById(sendMessageQueueId);

            SendMessageRule sendMessageRule = sendMessageRuleRepository.GetById(sendMessageQueue.SendMessageRuleId);

            var clientTimezoneOffset = sendMessageRule.ClientTimezoneOffset;

            Every8d_SendMessageResult sendMessageResult = this.unitOfWork.Repository<Every8d_SendMessageResult>().Get(p => p.SendMessageQueueId == sendMessageQueueId);
            // if (SendMessageResult == null) return; 不應該為 null

            int? DepartmentId = null;
            if (sendMessageRule.CreatedUser.Department != null)
                DepartmentId = sendMessageRule.CreatedUser.Department.Id;
            string CreatedUserId = sendMessageRule.CreatedUser.Id;
            int SendMessageRuleId = sendMessageQueue.SendMessageRuleId;
            SendMessageType SendMessageType = sendMessageQueue.SendMessageType;

            DateTime SendTime = sendMessageQueue.SendTime;
            string SendTitle = sendMessageQueue.SendTitle;
            string SendBody = sendMessageQueue.SendBody;
            SendCustType SendCustType = sendMessageQueue.SendCustType;
            string RequestId = sendMessageResult.BATCH_ID;

            string ProviderName = this.Name;

            DateTime SendMessageResultCreatedTime = sendMessageResult.CreatedTime;

            List<Every8d_DeliveryReport> DeliveryReports = sendMessageResult.DeliveryReports.ToList();

            foreach (var DeliveryReport in DeliveryReports)
            {
                string DestinationName = DeliveryReport.NAME;

                SendMessageHistory entity = new SendMessageHistory();

                ////////////////////////////////////////
                // 01 ~ 05

                entity.DepartmentId = DepartmentId;
                entity.CreatedUserId = CreatedUserId;
                entity.SendMessageRuleId = SendMessageRuleId;
                entity.SendMessageQueueId = sendMessageQueueId;
                entity.SendMessageType = SendMessageType;

                ////////////////////////////////////////
                // 06 ~ 10

                entity.SendTime = SendTime;
                entity.SendTitle = SendTitle;
                entity.SendBody = SendBody;
                entity.SendCustType = SendCustType;
                entity.RequestId = RequestId;

                ////////////////////////////////////////
                // 11 ~ 15

                entity.ProviderName = ProviderName;
                entity.MessageId = null;
                entity.MessageStatus = MessageStatus.Unknown;
                entity.MessageStatusString = entity.MessageStatus.ToString();
                entity.SenderAddress = sendMessageRule.SenderAddress;
                
                ////////////////////////////////////////
                // 16 ~ 20

                // 20151106 Norman, Infobip 給的手機門號是 E164格式，但是沒有加上 "+"，使用 【MobileUtil.GetE164PhoneNumber】會導致誤判
                var destinationAddress = DeliveryReport.MOBILE;
                if (!destinationAddress.StartsWith("+"))
                    destinationAddress = "+" + destinationAddress;

                entity.DestinationAddress = MobileUtil.GetE164PhoneNumber(destinationAddress);
                entity.SendMessageResultCreatedTime = SendMessageResultCreatedTime;

                // TODO: 驗證 Every8d 回傳發送時間轉成 UTC 時間是否正確
                //entity.SentDate = Converter.ToDateTime(DeliveryReport.SENT_TIME, Converter.Every8d_SentTime).Value; // 2010/03/23 12:05:29
                //entity.DoneDate = Converter.ToDateTime(DeliveryReport.SENT_TIME, Converter.Every8d_SentTime).Value; // 2010/03/23 12:05:29，簡訊供應商沒有提供此資訊，因此設定與SentDate一致
                entity.SentDate = Converter.ToDateTime(DeliveryReport.SENT_TIME, Converter.Every8d_SentTime).Value.ToUniversalTime(); // 2010/03/23 12:05:29
                entity.DoneDate = Converter.ToDateTime(DeliveryReport.SENT_TIME, Converter.Every8d_SentTime).Value.ToUniversalTime(); // 2010/03/23 12:05:29，簡訊供應商沒有提供此資訊，因此設定與SentDate一致

                entity.DeliveryStatus = (DeliveryReportStatus)Convert.ToInt32(DeliveryReport.STATUS);

                ////////////////////////////////////////
                // 21 ~ 25

                entity.DeliveryStatusString = entity.DeliveryStatus.ToString();
                entity.Price = Convert.ToDecimal(DeliveryReport.COST);
                entity.DeliveryReportCreatedTime = DeliveryReport.CreatedTime;
                entity.MessageCost = (new MessageCostInfo(entity.SendBody, entity.DestinationAddress).MessageCost);
                entity.Delivered = IsDelivered(entity.DeliveryStatus);

                ////////////////////////////////////////
                // 26
                entity.DestinationName = DestinationName;
                entity.Region = MobileUtil.GetRegionName(entity.DestinationAddress);
                entity.CreatedTime = DateTime.UtcNow;

                entity = sendMessageHistoryRepository.Insert(entity);

                // 如果發送失敗，就回補點數
                // 20151123 Norman, Eric 要求發送失敗不回補點數
                //this.tradeService.HandleSendMessageHistory(sendMessageRule, sendMessageQueue, entity);
            }

            // (6) 出統計表
            this.sendMessageStatisticService.AddOrUpdateSendMessageStatistic(sendMessageQueueId);
        }

        /// <summary>
        /// 判斷傳送簡訊是否成功
        /// </summary>
        private bool IsDelivered(DeliveryReportStatus Status)
        {
            return Status == DeliveryReportStatus.Sent;
        }

    }
}
