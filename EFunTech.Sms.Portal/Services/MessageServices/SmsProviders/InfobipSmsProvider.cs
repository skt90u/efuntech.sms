/*
http://www.gsma.com/oneapi/faq-messaging-api-questions/

What are the correct formats when specifying recipients 
when using OneAPI messaging functions?OneAPI 
supports two schemes for specifying message recipients, 
the first is to specify a mobile phone number directly, 
the second uses a scheme known as ACR – 
Anonymous Customer ReferenceAnonymous Customer Reference is a proposal by 
GSMA where a unique identification is provided for a mobile user but which does not use their mobile phone number in any identifiable way. Where ACR is provided by a mobile network this should be used.Example formats for messaging recipients:
tel:0789123456 – phone number using numbering scheme of the home network
tel:+19876543210 – phone number using international format including country code
acr:0123456890123456789 – anonymous customer reference
 */

using EFunTech.Sms.Core;
using EFunTech.Sms.Portal;
using EFunTech.Sms.Schema;
using Hangfire;
using JUtilSharp.Database;
using Microsoft.Practices.ServiceLocation;
using Newtonsoft.Json;
using OneApi.Client.Impl;
using OneApi.Config;
using OneApi.Listeners;
using OneApi.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFunTech.Sms.Portal
{
    /// <summary>
    /// http://dev.infobip.com/
    /// </summary>
    public class InfobipSmsProvider : ISmsProvider
    {
        private string userName;
        private string password;
        private Configuration configuration;
        private SMSClient smsClient;

        private ISystemParameters systemParameters;
        private ILogService logService;
        private IUnitOfWork unitOfWork;
        private SendMessageStatisticService sendMessageStatisticService;

        private TradeService tradeService;
        private SmsProviderType smsProviderType;

        public InfobipSmsProvider(ISystemParameters systemParameters, ILogService logService, IUnitOfWork unitOfWork, SmsProviderType smsProviderType)
        {
            if(smsProviderType == SmsProviderType.InfobipHighQuality)
            {
                this.userName = systemParameters.InfobipHighQualityUserName; 
                this.password = systemParameters.InfobipHighQualityPassword;
            }
            else
            {
                 this.userName = systemParameters.InfobipNormalQualityUserName; 
                 this.password = systemParameters.InfobipNormalQualityPassword;
            }
            
            this.configuration = new Configuration(userName, password);
            this.smsClient = new SMSClient(configuration);

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
                catch(Exception ex)
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
                AccountBalance accountBalance = this.smsClient.CustomerProfileClient.GetAccountBalance();
                return accountBalance.Balance;
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
            
            string message = sendMessageQueue.SendBody;
            
            var messageReceivers = this.unitOfWork.Repository<MessageReceiver>().GetMany(p =>
                p.SendMessageRuleId == sendMessageQueue.SendMessageRuleId &&
                p.SendBody == message).ToList();

            var senderAddress = string.Empty;
            var sendMessageRule = this.unitOfWork.Repository<SendMessageRule>().GetById(sendMessageQueue.SendMessageRuleId);
            if (sendMessageRule != null)
            {
                senderAddress = sendMessageRule.SenderAddress;
            }

            string[] recipientAddress = messageReceivers.Select(p => p.E164Mobile).ToArray();

            this.logService.Debug("InfobipSmsProvider(smsProviderType = {0})，發送簡訊(簡訊編號：{1}，簡訊序列編號：{2}，發送內容：{3}，發送名單：[{4}])", 
                smsProviderType.ToString(),
                sendMessageQueue.SendMessageRuleId,
                sendMessageQueue.Id,
                message,
                string.Join("、", recipientAddress));

            var smsRequest = new SMSRequest(senderAddress, message, recipientAddress);
            // 還是不知道怎麼使用
            // http://dev.infobip.com/v1/docs/logs-vs-delivery-reports
            // http://dev.infobip.com/docs/notify-url
            // 還是不知道怎麼使用 smsRequest.NotifyURL = "";
            //smsRequest.NotifyURL = @"http://zutech-sms.azurewebsites.net/api/InfobipNotification";

            SendMessageResult sendMessageResult = this.smsClient.SmsMessagingClient.SendSMS(smsRequest);

            this.logService.Debug("InfobipSmsProvider(smsProviderType = {0})，發送簡訊(簡訊編號：{1}，簡訊序列編號：{2}，回傳簡訊發送識別碼：{3}，回傳結果：{4})",
                smsProviderType.ToString(),
                sendMessageQueue.SendMessageRuleId,
                sendMessageQueue.Id,
                sendMessageResult.ClientCorrelator,
                sendMessageResult.ToString());

            // Send Email
            string subject = sendMessageQueue.SendTitle;
            string body = message;
            string[] emails = messageReceivers.Where(p => !string.IsNullOrEmpty(p.Email)).Select(p => p.Email).ToArray();
            if (emails.Any())
            {
                this.logService.Debug("InfobipSmsProvider(smsProviderType = {0})，發送Email(簡訊編號：{1}，簡訊序列編號：{2}，主旨：{3}，內容：{4}，發送名單：[{5}])",
                    smsProviderType.ToString(),
                    sendMessageQueue.SendMessageRuleId,
                    sendMessageQueue.Id,
                    subject,
                    body,
                    string.Join("、", emails));

                BackgroundJob.Enqueue<CommonMailService>(x => x.Send(subject, body, emails));
            }
            else
            {
                this.logService.Debug("InfobipSmsProvider(smsProviderType = {0})，無須發送Email(簡訊編號：{1}，簡訊序列編號：{2})",
                                        smsProviderType.ToString(),
                                        sendMessageQueue.SendMessageRuleId,
                                        sendMessageQueue.Id);
            }
            
            string requestId = sendMessageResult.ClientCorrelator; // you can use this to get deliveryReportList later.

            UpdateDb(sendMessageQueue, messageReceivers, sendMessageResult);
        }

        private void UpdateDb(SendMessageQueue sendMessageQueue, List<MessageReceiver> messageReceivers, SendMessageResult sendMessageResult)
        {
            // 寫入對應的 SendMessageResult

            var infobip_SendMessageResult = new Infobip_SendMessageResult();
            infobip_SendMessageResult.SourceTable = SourceTable.SendMessageQueue;
            infobip_SendMessageResult.SourceTableId = sendMessageQueue.Id;
            infobip_SendMessageResult.ClientCorrelator = sendMessageResult.ClientCorrelator;
            infobip_SendMessageResult.CreatedTime = DateTime.UtcNow; // 接收發送命令回傳值的時間
            infobip_SendMessageResult.Balance = this.Balance;
            infobip_SendMessageResult = this.unitOfWork.Repository<Infobip_SendMessageResult>().Insert(infobip_SendMessageResult);

            for (var i = 0; i < sendMessageResult.SendMessageResults.Length; i++)
            {
                // 一個 messageReceiver 對應一個 sendMessageResult
                //  尚未驗證，是否我傳送的 destinations 順序與 SendMessageResults 順序一致
                //  【目前假設是一致的】
                var messageReceiver = messageReceivers[i];
                var sendMessageResultItem = sendMessageResult.SendMessageResults[i];

                var infobip_SendMessageResultItem = new Infobip_SendMessageResultItem();

                infobip_SendMessageResultItem.MessageId = sendMessageResultItem.MessageId;
                infobip_SendMessageResultItem.MessageStatusString = sendMessageResultItem.MessageStatus;

                EFunTech.Sms.Schema.MessageStatus MessageStatus = EFunTech.Sms.Schema.MessageStatus.Unknown;
                Enum.TryParse<EFunTech.Sms.Schema.MessageStatus>(sendMessageResultItem.MessageStatus, out MessageStatus);
                infobip_SendMessageResultItem.MessageStatus = MessageStatus;

                infobip_SendMessageResultItem.SenderAddress = sendMessageResultItem.SenderAddress;
                infobip_SendMessageResultItem.DestinationAddress = sendMessageResultItem.DestinationAddress;
                infobip_SendMessageResultItem.SendMessageResult = infobip_SendMessageResult;
                infobip_SendMessageResultItem.DestinationName = messageReceiver.Name;


                infobip_SendMessageResultItem = this.unitOfWork.Repository<Infobip_SendMessageResultItem>().Insert(infobip_SendMessageResultItem);
            }

            var infobip_ResourceReference = new Infobip_ResourceReference();
            infobip_ResourceReference.SendMessageResultId = infobip_SendMessageResult.Id;
            infobip_ResourceReference.ResourceURL = sendMessageResult.ResourceRef.ResourceURL;
            infobip_ResourceReference = this.unitOfWork.Repository<Infobip_ResourceReference>().Insert(infobip_ResourceReference);

            CreateSendMessageHistory(sendMessageQueue);

            // 在 Thread 中等待 30 秒，再寫入 DeliveryReportQueue
            var delayMilliseconds = (int)30 * 1000;
            FaFTaskFactory.StartNew(delayMilliseconds, () =>
            {
                using(var context = new ApplicationDbContext())
                {
                    var _unitOfWork = new UnitOfWork(context);
                    var _repository = _unitOfWork.Repository<DeliveryReportQueue>();

                    // 寫入簡訊派送結果等待取回序列
                    var deliveryReportQueue = new DeliveryReportQueue();
                    deliveryReportQueue.SourceTableId = sendMessageQueue.Id;
                    deliveryReportQueue.SourceTable = SourceTable.SendMessageQueue;
                    deliveryReportQueue.RequestId = infobip_SendMessageResult.ClientCorrelator;
                    deliveryReportQueue.ProviderName = this.Name;
                    deliveryReportQueue.CreatedTime = DateTime.UtcNow;
                    deliveryReportQueue.SendMessageResultItemCount = infobip_SendMessageResult.SendMessageResults.Count;
                    deliveryReportQueue.DeliveryReportCount = 0;
                    deliveryReportQueue = _repository.Insert(deliveryReportQueue);
                }
            });
        }

        public void GetDeliveryReport(string requestId)
        {
            // (1) 根據 requestId (DeliveryReportQueue.ClientCorrelator) 查詢簡訊發送結果
            // (2) 如果查得到結果(DeliveryReportList.DeliveryReports.Length != 0)，則表示 SendMessageQueue 經由 Infobip 發送所有訊息的結果已經取得，可以進行以下步驟
            // (3) 在 DeliveryReportQueue (待查詢簡訊發送結果序列) 刪除對應資料
            // (4) 將 DeliveryReportList.DeliveryReports 塞入對應資料表 Infobip_DeliveryReport
            // (5) 出大表，SendMessageHistorys

            // (1) 根據 requestId (DeliveryReportQueue.ClientCorrelator) 查詢簡訊發送結果

            DeliveryReportList deliveryReportList = this.smsClient.SmsMessagingClient.GetDeliveryReportsByRequestId(requestId);

            // (2) 如果查得到結果(DeliveryReportList.DeliveryReports.Length != 0)，則表示 SendMessageQueue 經由 Infobip 發送所有訊息的結果已經取得，可以進行以下步驟

            UpdateDb(requestId, deliveryReportList);

            
        }

        private void UpdateDb(string requestId, DeliveryReportList deliveryReportList)
        {
            if (deliveryReportList.DeliveryReports.Length == 0) return;

            this.logService.Debug("InfobipSmsProvider(smsProviderType = {0})，接收簡訊發送結果(簡訊發送識別碼：{1}，發送結果：{2})",
                smsProviderType.ToString(),
                requestId,
                deliveryReportList.ToString());

            // (3) 將 DeliveryReportList.DeliveryReports 塞入對應資料表 Infobip_DeliveryReport

            foreach (var deliveryReport in deliveryReportList.DeliveryReports)
            {
                var entity = new Infobip_DeliveryReport();
                entity.RequestId = requestId;
                entity.MessageId = deliveryReport.MessageId;
                entity.SentDate = deliveryReport.SentDate.ToUniversalTime(); // 已換轉換成UTC時間
                entity.DoneDate = deliveryReport.DoneDate.ToUniversalTime(); // 已換轉換成UTC時間
                entity.StatusString = deliveryReport.Status;
                
                DeliveryReportStatus Status = DeliveryReportStatus.Unknown;
                Enum.TryParse<DeliveryReportStatus>(deliveryReport.Status, out Status);
                entity.Status = Status;

                entity.Price = deliveryReport.Price;
                entity.CreatedTime = DateTime.UtcNow;
                entity = this.unitOfWork.Repository<Infobip_DeliveryReport>().Insert(entity);
            }

            Infobip_SendMessageResult infobip_SendMessageResult = this.unitOfWork.Repository<Infobip_SendMessageResult>().Get(p => p.ClientCorrelator == requestId);
            if (infobip_SendMessageResult == null)
                throw new Exception(string.Format("InfobipSmsProvider(smsProviderType = {0})，無法取得 Infobip_SendMessageResult(ClientCorrelator：{1})", smsProviderType.ToString(), requestId));

            // (4) 如果所有派送結果都取回了，就在 DeliveryReportQueue (待查詢簡訊發送結果序列) 刪除對應資料
            var deliveryReportQueueRepository = this.unitOfWork.Repository<DeliveryReportQueue>();
            var deliveryReportQueue = deliveryReportQueueRepository.Get(p => p.RequestId == requestId && p.ProviderName == this.Name);
            if (deliveryReportQueue != null)
            {
                int SendMessageResultItemCount = infobip_SendMessageResult.SendMessageResults.Count;
                int DeliveryReportCount = this.unitOfWork.Repository<Infobip_DeliveryReport>().Count(p => p.RequestId == requestId);
                
                deliveryReportQueue.SendMessageResultItemCount = SendMessageResultItemCount;
                deliveryReportQueue.DeliveryReportCount = DeliveryReportCount;
                deliveryReportQueueRepository.Update(deliveryReportQueue);

                // 20151111 Norman, 暫時不刪除，用以除錯
                //if (DeliveryReportCount >= SendMessageResultItemCount)
                //{
                //    deliveryReportQueueRepository.Delete(p => p.RequestId == requestId);
                //}
            }

            // (5) 出大表，SendMessageHistorys
            SourceTable sourceTable = infobip_SendMessageResult.SourceTable;
            int sourceTableId = infobip_SendMessageResult.SourceTableId;
            switch (sourceTable)
            {
                case SourceTable.SendMessageQueue:
                    int sendMessageQueueId = infobip_SendMessageResult.SourceTableId;
                    UpdateSendMessageHistory(sourceTableId);
                    break;
                case SourceTable.SendMessageHistory:
                    int sendMessageHistory = infobip_SendMessageResult.SourceTableId;
                    UpdateSendMessageRetryHistory(sendMessageHistory);
                    break;
            }
        }

        private void UpdateSendMessageRetryHistory(int sendMessageHistory)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 在 SendSMS 之後，建立大表 SendMessageHistory
        /// </summary>
        public void CreateSendMessageHistory(SendMessageQueue sendMessageQueue)
        {
            int sendMessageQueueId = sendMessageQueue.Id;

            var sendMessageRuleRepository = this.unitOfWork.Repository<SendMessageRule>();
            var sendMessageQueueRepository = this.unitOfWork.Repository<SendMessageQueue>();
            var sendMessageHistoryRepository = this.unitOfWork.Repository<SendMessageHistory>();

            SendMessageRule sendMessageRule = sendMessageRuleRepository.GetById(sendMessageQueue.SendMessageRuleId);

            Infobip_SendMessageResult sendMessageResult = this.unitOfWork.Repository<Infobip_SendMessageResult>().Get(p => p.SourceTable == SourceTable.SendMessageQueue && p.SourceTableId == sendMessageQueueId);
            if (sendMessageResult == null)
                throw new Exception(string.Format("InfobipSmsProvider(smsProviderType = {0})，無法取得 Infobip_SendMessageResult(SendMessageQueueId：{1})", smsProviderType.ToString(), sendMessageQueueId));

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
            string RequestId = sendMessageResult.ClientCorrelator;

            string ProviderName = this.Name;

            DateTime SendMessageResultCreatedTime = sendMessageResult.CreatedTime;

            List<Infobip_SendMessageResultItem> SendMessageResults = sendMessageResult.SendMessageResults.ToList();

            foreach (var SendMessageResult in SendMessageResults)
            {
                string DestinationName = SendMessageResult.DestinationName;

                var entity = new SendMessageHistory();

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
                entity.MessageId = SendMessageResult.MessageId;
                entity.MessageStatus = (EFunTech.Sms.Schema.MessageStatus)((int)SendMessageResult.MessageStatus);
                entity.MessageStatusString = entity.MessageStatus.ToString();
                entity.SenderAddress = SendMessageResult.SenderAddress;

                ////////////////////////////////////////
                // 16 ~ 20

                // 20151106 Norman, Infobip 給的手機門號是 E164格式，但是沒有加上 "+"，使用 【MobileUtil.GetE164PhoneNumber】會導致誤判
                var destinationAddress = SendMessageResult.DestinationAddress;
                if (!destinationAddress.StartsWith("+", StringComparison.OrdinalIgnoreCase))
                    destinationAddress = "+" + destinationAddress;
                
                entity.DestinationAddress = MobileUtil.GetE164PhoneNumber(destinationAddress);
                entity.SendMessageResultCreatedTime = SendMessageResultCreatedTime;
                entity.SentDate = null;
                entity.DoneDate = null;
                entity.DeliveryStatus = DeliveryReportStatus.MessageAccepted; 

                ////////////////////////////////////////
                // 21 ~ 25

                entity.DeliveryStatusString = entity.DeliveryStatus.ToString();
                entity.Price = (decimal)0.0; // 尚未傳送完成
                entity.DeliveryReportCreatedTime = null;
                entity.MessageCost = (new MessageCostInfo(entity.SendBody, entity.DestinationAddress).MessageCost);
                entity.Delivered = IsDelivered(entity.DeliveryStatus);

                ////////////////////////////////////////
                // 26
                entity.DestinationName = DestinationName;
                entity.Region = MobileUtil.GetRegionName(entity.DestinationAddress);
                entity.CreatedTime = DateTime.UtcNow;
                entity.RetryMaxTimes = systemParameters.RetryMaxTimes;
                entity.RetryTotalTimes = 0;
                entity.SendMessageRetryHistoryId = null;

                entity = sendMessageHistoryRepository.Insert(entity);
            }

            // (6) 出統計表
            this.sendMessageStatisticService.AddOrUpdateSendMessageStatistic(sendMessageQueueId);
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

            Infobip_SendMessageResult sendMessageResult = this.unitOfWork.Repository<Infobip_SendMessageResult>().Get(p => p.SourceTable == SourceTable.SendMessageQueue && p.SourceTableId == sendMessageQueueId);
            if (sendMessageResult == null)
                throw new Exception(string.Format("InfobipSmsProvider(smsProviderType = {0})，無法取得 Infobip_SendMessageResult(SendMessageQueueId：{1})", smsProviderType.ToString(), sendMessageQueueId));

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
            string RequestId = sendMessageResult.ClientCorrelator;

            string ProviderName = this.Name;

            DateTime SendMessageResultCreatedTime = sendMessageResult.CreatedTime;

            List<Infobip_SendMessageResultItem> SendMessageResults = sendMessageResult.SendMessageResults.ToList();

            foreach (var SendMessageResult in SendMessageResults)
            {
                Infobip_DeliveryReport DeliveryReport = SendMessageResult.DeliveryReport;
                if (DeliveryReport == null) continue; // 如果尚未取得派送報表，就忽略

                SendMessageHistory entity = this.unitOfWork.Repository<SendMessageHistory>().Get(p => p.MessageId == SendMessageResult.MessageId);
                if (entity == null) continue; // 如果找不到對應 MessageId，就忽略

                string DestinationName = SendMessageResult.DestinationName;

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
                entity.MessageId = SendMessageResult.MessageId;
                entity.MessageStatus = (EFunTech.Sms.Schema.MessageStatus)((int)SendMessageResult.MessageStatus);
                entity.MessageStatusString = entity.MessageStatus.ToString();
                entity.SenderAddress = SendMessageResult.SenderAddress;

                ////////////////////////////////////////
                // 16 ~ 20

                // 20151106 Norman, Infobip 給的手機門號是 E164格式，但是沒有加上 "+"，使用 【MobileUtil.GetE164PhoneNumber】會導致誤判
                var destinationAddress = SendMessageResult.DestinationAddress;
                if (!destinationAddress.StartsWith("+", StringComparison.OrdinalIgnoreCase))
                    destinationAddress = "+" + destinationAddress;

                entity.DestinationAddress = MobileUtil.GetE164PhoneNumber(destinationAddress);
                entity.SendMessageResultCreatedTime = SendMessageResultCreatedTime;
                entity.SentDate = DeliveryReport.SentDate;
                entity.DoneDate = DeliveryReport.DoneDate;
                entity.DeliveryStatus = DeliveryReport.Status;

                ////////////////////////////////////////
                // 21 ~ 25

                entity.DeliveryStatusString = entity.DeliveryStatus.ToString();
                entity.Price = DeliveryReport.Price ?? (decimal) 0.0;
                entity.DeliveryReportCreatedTime = DeliveryReport.CreatedTime;
                entity.MessageCost = (new MessageCostInfo(entity.SendBody, entity.DestinationAddress).MessageCost);
                entity.Delivered = IsDelivered(entity.DeliveryStatus);

                ////////////////////////////////////////
                // 26
                entity.DestinationName = DestinationName;

                entity.Region = MobileUtil.GetRegionName(entity.DestinationAddress);

                sendMessageHistoryRepository.Update(entity);

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
            return (Status == DeliveryReportStatus.DeliveredToTerminal);
        }

        public void RetrySMS(int sendMessageHistoryId)
        {
            var sendMessageHistory = this.unitOfWork.Repository<SendMessageHistory>().GetById(sendMessageHistoryId);

            string message = sendMessageHistory.SendBody;

            var senderAddress = sendMessageHistory.SenderAddress;

            //string[] recipientAddress = { sendMessageHistory.DestinationAddress };
            string[] recipientAddress = { "+886921859698" };

            this.logService.Debug("InfobipSmsProvider(smsProviderType = {0})，重試簡訊(簡訊發送結果歷史紀錄編號：{1}，發送內容：{2}，發送名單：[{3}])",
                smsProviderType.ToString(),
                sendMessageHistory.Id,
                message,
                string.Join("、", recipientAddress));

            var smsRequest = new SMSRequest(senderAddress, message, recipientAddress);
            // 還是不知道怎麼使用
            // http://dev.infobip.com/v1/docs/logs-vs-delivery-reports
            // http://dev.infobip.com/docs/notify-url
            // 還是不知道怎麼使用 smsRequest.NotifyURL = "";
            //smsRequest.NotifyURL = @"http://zutech-sms.azurewebsites.net/api/InfobipNotification";

            SendMessageResult sendMessageResult = this.smsClient.SmsMessagingClient.SendSMS(smsRequest);

            this.logService.Debug("InfobipSmsProvider(smsProviderType = {0})，重試簡訊(簡訊發送結果歷史紀錄編號：{1}，回傳簡訊發送識別碼：{2}，回傳結果：{3})",
                smsProviderType.ToString(),
                sendMessageHistory.Id,
                sendMessageResult.ClientCorrelator,
                sendMessageResult.ToString());

            string requestId = sendMessageResult.ClientCorrelator; // you can use this to get deliveryReportList later.

            UpdateDb(sendMessageHistory, sendMessageResult);
        }

        private void UpdateDb(SendMessageHistory sendMessageHistory, SendMessageResult sendMessageResult)
        {
            // 寫入對應的 SendMessageResult
            /*
            var infobip_SendMessageResult = new Infobip_SendMessageResult();
            infobip_SendMessageResult.SendMessageQueueId = sendMessageQueue.Id;
            infobip_SendMessageResult.ClientCorrelator = sendMessageResult.ClientCorrelator;
            infobip_SendMessageResult.CreatedTime = DateTime.UtcNow; // 接收發送命令回傳值的時間
            infobip_SendMessageResult.Balance = this.Balance;
            infobip_SendMessageResult = this.unitOfWork.Repository<Infobip_SendMessageResult>().Insert(infobip_SendMessageResult);

            for (var i = 0; i < sendMessageResult.SendMessageResults.Length; i++)
            {
                // 一個 messageReceiver 對應一個 sendMessageResult
                //  尚未驗證，是否我傳送的 destinations 順序與 SendMessageResults 順序一致
                //  【目前假設是一致的】
                var messageReceiver = messageReceivers[i];
                var sendMessageResultItem = sendMessageResult.SendMessageResults[i];

                var infobip_SendMessageResultItem = new Infobip_SendMessageResultItem();

                infobip_SendMessageResultItem.MessageId = sendMessageResultItem.MessageId;
                infobip_SendMessageResultItem.MessageStatusString = sendMessageResultItem.MessageStatus;

                EFunTech.Sms.Schema.MessageStatus MessageStatus = EFunTech.Sms.Schema.MessageStatus.Unknown;
                Enum.TryParse<EFunTech.Sms.Schema.MessageStatus>(sendMessageResultItem.MessageStatus, out MessageStatus);
                infobip_SendMessageResultItem.MessageStatus = MessageStatus;

                infobip_SendMessageResultItem.SenderAddress = sendMessageResultItem.SenderAddress;
                infobip_SendMessageResultItem.DestinationAddress = sendMessageResultItem.DestinationAddress;
                infobip_SendMessageResultItem.SendMessageResult = infobip_SendMessageResult;
                infobip_SendMessageResultItem.DestinationName = messageReceiver.Name;


                infobip_SendMessageResultItem = this.unitOfWork.Repository<Infobip_SendMessageResultItem>().Insert(infobip_SendMessageResultItem);
            }

            var infobip_ResourceReference = new Infobip_ResourceReference();
            infobip_ResourceReference.SendMessageResultId = infobip_SendMessageResult.SendMessageQueueId;
            infobip_ResourceReference.ResourceURL = sendMessageResult.ResourceRef.ResourceURL;
            infobip_ResourceReference = this.unitOfWork.Repository<Infobip_ResourceReference>().Insert(infobip_ResourceReference);

            CreateSendMessageHistory(sendMessageQueue);

            // 在 Thread 中等待 30 秒，再寫入 DeliveryReportQueue
            var delayMilliseconds = (int)30 * 1000;
            FaFTaskFactory.StartNew(delayMilliseconds, () =>
            {
                using (var context = new ApplicationDbContext())
                {
                    var _unitOfWork = new UnitOfWork(context);
                    var _repository = _unitOfWork.Repository<DeliveryReportQueue>();

                    // 寫入簡訊派送結果等待取回序列
                    var deliveryReportQueue = new DeliveryReportQueue();
                    deliveryReportQueue.SendMessageQueueId = sendMessageQueue.Id;
                    deliveryReportQueue.RequestId = infobip_SendMessageResult.ClientCorrelator;
                    deliveryReportQueue.ProviderName = this.Name;
                    deliveryReportQueue.CreatedTime = DateTime.UtcNow;
                    deliveryReportQueue.SendMessageResultItemCount = infobip_SendMessageResult.SendMessageResults.Count;
                    deliveryReportQueue.DeliveryReportCount = 0;
                    deliveryReportQueue = _repository.Insert(deliveryReportQueue);
                }
            });
            */
        }
    }
}

