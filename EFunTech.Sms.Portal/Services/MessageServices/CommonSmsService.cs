using EFunTech.Sms.Core;
using EFunTech.Sms.Portal.Services;
using EFunTech.Sms.Schema;
using Hangfire;
using JUtilSharp.Database;
using OneApi.Model;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace EFunTech.Sms.Portal
{
    public class CommonSmsService
    {
        private ISystemParameters systemParameters;
        private ILogService logService;
        private IUnitOfWork unitOfWork;
        private TradeService tradeService;

        private UniqueJobList uniqueJobList;

        public CommonSmsService(ISystemParameters systemParameters, ILogService logService, IUnitOfWork unitOfWork)
        {
            this.systemParameters = systemParameters;
            this.logService = logService;
            this.unitOfWork = unitOfWork;
            this.tradeService = new TradeService(unitOfWork, logService);

            this.uniqueJobList = new UniqueJobList(systemParameters, logService, unitOfWork);
        }

        /// <summary>
        /// 列出所有提供的簡訊供應商，請依照提供優先順序排列。
        /// </summary>
        private SmsProviderType[] GetSmsProviderTypes(SmsProviderType smsProviderType)
        {
            var providerTypes = Enum.GetValues(typeof(SmsProviderType)).Cast<SmsProviderType>().ToList();
            var found = providerTypes.IndexOf(smsProviderType);

            if(found == -1)
                throw new Exception(string.Format("Can not find {0} in SmsProviderType", smsProviderType));

            var sub1 = providerTypes.Take(found);
            var sub2 = providerTypes.Skip(found);

            var result = new List<SmsProviderType>();
            result.AddRange(sub2);
            result.AddRange(sub1);

            return result.ToArray();
        }

        private SmsProviderType[] GetSmsProviderTypes(string providerName)
        {
            SmsProviderType smsProviderType = SmsProviderType.InfobipNormalQuality;
            if (!Enum.TryParse<SmsProviderType>(providerName, out smsProviderType))
            {
                throw new Exception(string.Format("Can not parse {0} to SmsProviderType", providerName));
            }

            return GetSmsProviderTypes(smsProviderType);
        }

        private Type ToInstanceType(SmsProviderType smsProviderType)
        {
            switch (smsProviderType)
            {
                case SmsProviderType.InfobipNormalQuality:
                case SmsProviderType.InfobipHighQuality:
                    return typeof(InfobipSmsProvider);
                case SmsProviderType.Every8d:
                    return typeof(Every8dSmsProvider);
                default:
                    throw new Exception(string.Format("Unknown SmsProviderType({0})", smsProviderType));
            }
        }

        private ISmsProvider GetProvider(SmsProviderType currentProviderType, decimal requiredBalance)
        {
            ISmsProvider provider = null;

            var providerTypesInOrder = GetSmsProviderTypes(currentProviderType);

            List<string> errors = new List<string>();

            foreach (var providerType in providerTypesInOrder)
            {
                Type instanceType = ToInstanceType(providerType);

                var _provider = Activator.CreateInstance(instanceType, systemParameters, logService, unitOfWork, providerType) as ISmsProvider;
                if (_provider != null &&
                    _provider.IsAvailable)
                {
                    decimal balance = _provider.Balance;
                    decimal toProviderBalance = _provider.ToProviderBalance(requiredBalance);

                    if (balance >= toProviderBalance)
                    {
                        provider = _provider;
                        break;
                    }
                    else
                    {
                        errors.Add(string.Format("簡訊供應商({0})：目前點數 {1}，發送所需點數 {2}。", providerType, balance, toProviderBalance));
                    }

                    // 20160520 Norman, 測試發送大量簡訊，測試完後請移除
                    //provider = _provider;
                }
            }

            if (provider == null)
            {
                string subject = string.Format("點數不足，無法發送簡訊(所需點數 {0})", requiredBalance);
                string body = string.Join("\n", errors);
                string[] destinations = new string[] { systemParameters.InsufficientBalanceNotifiee };
                //string[] destinations = this.unitOfWork.Repository<ApplicationUser>().GetMany(p => p.Roles.Contains(Role.Administrator)).Select(p => p.Email).ToArray();
                BackgroundJob.Enqueue<CommonMailService>(x => x.Send(subject, body, destinations));
                this.logService.Error(subject);
                throw new Exception(subject);
            }

            return provider;
        }

        private ISmsProvider GetProvider(string providerTypeName, bool mustAvailable = true)
        {
            ISmsProvider provider = null;

            var providerTypesInOrder = GetSmsProviderTypes(providerTypeName);
            var providerType = providerTypesInOrder.FirstOrDefault();
            Type instanceType = ToInstanceType(providerType);
            if (instanceType != null)
            {
                var _provider = Activator.CreateInstance(instanceType, systemParameters, logService, unitOfWork, providerType) as ISmsProvider;

                if (mustAvailable)
                {
                    if (_provider != null && _provider.IsAvailable)
                    {
                        provider = _provider;
                    }
                }
                else
                {
                    if (_provider != null)
                    {
                        provider = _provider;
                    }
                }
            }

            if (provider == null)
            {
                string message = string.Format("目前無法使用指定簡訊供應商 {0} ", providerTypeName);

                this.logService.Error(message);

                // 取得DeliveryReport並沒有急迫性，等到簡訊供應商可供連線再取即可。
                // throw new Exception(message);
            }

            return provider;
        }

        [AutomaticRetry(Attempts = 0)]
        public void SendSMS(int sendMessageRuleId, DateTime sendTime)
        {
            UniqueJob uniqueJob = null;

            try
            {
                uniqueJob = this.uniqueJobList.AddOrUpdate("SendSMS", sendMessageRuleId, sendTime);
                if (uniqueJob == null) return; // Job 已經存在

                // 如果在 SendMessageQueue 中已經有對應資料，就忽略
                if (this.unitOfWork.Repository<SendMessageQueue>().Any(p =>
                    p.SendMessageRuleId == sendMessageRuleId &&
                    p.SendTime == sendTime)) return;

                using (var scope = this.unitOfWork.CreateTransactionScope())
                {
                    SendMessageRule sendMessageRule = this.unitOfWork.Repository<SendMessageRule>().GetById(sendMessageRuleId);
                    if (sendMessageRule == null)
                    {
                        this.logService.Error("sendMessageRule is null (sendMessageRuleId: {0})", sendMessageRuleId);
                        return;
                    }

                    this.logService.Debug("CommonSmsService，發送簡訊(簡訊編號：{0}，預定發送時間：{1}，實際發送時間：{2})",
                            sendMessageRule.Id,
                            Converter.ToLocalTime(sendTime, sendMessageRule.ClientTimezoneOffset).ToString(Converter.Every8d_SentTime),
                            Converter.ToLocalTime(DateTime.UtcNow, sendMessageRule.ClientTimezoneOffset).ToString(Converter.Every8d_SentTime));

                    ApplicationUser user = sendMessageRule.CreatedUser;

                    if (sendMessageRule.SendMessageRuleStatus != SendMessageRuleStatus.Ready) return;

                    // 此簡訊規則下，所有收訊者資訊
                    var ruleReceivers = this.unitOfWork.Repository<MessageReceiver>().GetMany(p => p.SendMessageRuleId == sendMessageRule.Id).ToList();

                    // 目前有效的黑名單手機號碼 (E164)
                    var blackListMobiles = this.unitOfWork.Repository<Blacklist>()
                        .GetMany(p => p.CreatedUser.Id == sendMessageRule.CreatedUser.Id && p.Enabled == true)
                        .Select(p => p.Mobile)
                        .Distinct()
                        .ToList()
                        .Select(p => MobileUtil.GetE164PhoneNumber(p))
                        .ToList();

                    // 在黑名單的收訊者
                    var ruleReceiversInBlackList = ruleReceivers
                        .Where(p => blackListMobiles.Contains(MobileUtil.GetE164PhoneNumber(p.Mobile)))
                        .ToList();

                    // 不在黑名單的收訊者
                    var ruleReceiversNotInBlackList = ruleReceivers
                        .Where(p => !blackListMobiles.Contains(MobileUtil.GetE164PhoneNumber(p.Mobile)))
                        .ToList();

                    this.logService.Debug("CommonSmsService，發送簡訊(簡訊編號：{0}，預定發送名單：{1})", sendMessageRuleId, ExcelBugFix.GetInformation(ruleReceivers.Select(p => p.Mobile).ToList()));
                    this.logService.Debug("CommonSmsService，發送簡訊(簡訊編號：{0}，黑名單：{1})", sendMessageRuleId, ExcelBugFix.GetInformation(ruleReceiversInBlackList.Select(p => p.Mobile).ToList()));
                    this.logService.Debug("CommonSmsService，發送簡訊(簡訊編號：{0}，實際發送名單：{1})", sendMessageRuleId, ExcelBugFix.GetInformation(ruleReceiversNotInBlackList.Select(p => p.Mobile).ToList()));

                    //List<string> detailInformations = new List<string>();
                    //detailInformations.AddRange(ExcelBugFix.GetDetailInformation(ruleReceivers.Select(p => p.Mobile).ToList())
                    //    .Select(p => string.Format("CommonSmsService，發送簡訊(簡訊編號：{0}，預定發送名單(全)：{1})", sendMessageRuleId, p))
                    //    .ToList());
                    //detailInformations.AddRange(ExcelBugFix.GetDetailInformation(ruleReceiversInBlackList.Select(p => p.Mobile).ToList())
                    //    .Select(p => string.Format("CommonSmsService，發送簡訊(簡訊編號：{0}，黑名單(全)：{1})", sendMessageRuleId, p))
                    //    .ToList());
                    //detailInformations.AddRange(ExcelBugFix.GetDetailInformation(ruleReceiversNotInBlackList.Select(p => p.Mobile).ToList())
                    //    .Select(p => string.Format("CommonSmsService，發送簡訊(簡訊編號：{0}，實際發送名單(全)：{1})", sendMessageRuleId, p))
                    //    .ToList());
                    //foreach (var detailInformation in detailInformations)
                    //{
                    //    this.logService.Debug(detailInformation);
                    //}

                    // 找出所有不同的發送內容
                    var sendBodies = ruleReceiversNotInBlackList.Select(p => p.SendBody).Distinct().ToList();

                    // 針對沒有執行預扣的簡訊規則，判斷是否有足夠點數進行簡訊發送作業

                    decimal totalMessageCost = ruleReceiversNotInBlackList.Sum(p => p.MessageCost);

                    if (!this.tradeService.ShouldWithhold(sendMessageRule.SendTimeType))
                    {
                        if (sendMessageRule.CreatedUser.SmsBalance < totalMessageCost)
                        {
                            string message = string.Format("執行{0}失敗(編號:{1})，使用者 {2} 點數不足，目前點數 {3} 點，發送所需點數 {4} 點",
                                AttributeHelper.GetColumnDescription(sendMessageRule.SendTimeType),
                                sendMessageRule.Id,
                                sendMessageRule.CreatedUser.UserName,
                                sendMessageRule.CreatedUser.SmsBalance,
                                totalMessageCost);

                            this.logService.Error(message);

                            throw new Exception(message);
                        }
                    }

                    // 找出可以使用的簡訊供應商(可連線且餘額足夠)

                    SmsProviderType userSmsProviderType = user.SmsProviderType; 
                    
                    ISmsProvider provider = GetProvider(userSmsProviderType, totalMessageCost); // 如果沒有找到，將會拋出例外

                    // 開始發送，將 SendMessageRule 狀態改變為 Sending (正在發送簡訊規則)
                    sendMessageRule.SendMessageRuleStatus = SendMessageRuleStatus.Sending;
                    sendMessageRule.CreatedUser = user;
                    this.unitOfWork.Repository<SendMessageRule>().Update(sendMessageRule);

                    foreach (var sendBody in sendBodies)
                    {
                        // 依據傳送內容分群，每個群組使用一個SendMessageQueue，
                        // 此SendMessageQueue下，所有收訊者資訊
                        var queueReceivers = ruleReceiversNotInBlackList.Where(p => p.SendBody == sendBody).ToList();

                        var sendMessageQueue = new SendMessageQueue();
                        sendMessageQueue.SendMessageType = sendMessageRule.SendMessageType;
                        sendMessageQueue.SendTime = sendTime.ToUniversalTime(); // 預定發送訊息的時間(SendSMS經由Hangfire傳遞過來的sendTime，會將DateTimeKind轉為 Local，必須再轉成 Utc)
                        sendMessageQueue.SendTitle = sendMessageRule.SendTitle;
                        sendMessageQueue.SendBody = sendBody;
                        sendMessageQueue.SendCustType = sendMessageRule.SendCustType;
                        sendMessageQueue.TotalReceiverCount = queueReceivers.Count();
                        sendMessageQueue.TotalMessageCost = queueReceivers.Sum(p => p.MessageCost);
                        sendMessageQueue.SendMessageRuleId = sendMessageRule.Id;

                        // 只針對非預先扣除的簡訊類型，對於打算要發送的內容進行扣點
                        this.tradeService.CreateSendMessageQueue(sendMessageRule, sendMessageQueue);

                        sendMessageQueue = this.unitOfWork.Repository<SendMessageQueue>().Insert(sendMessageQueue);

                        this.logService.Debug("CommonSmsService，發送簡訊(簡訊編號：{0}，序列號碼：{1}，發送名單：{2}，發送內容：{3})", 
                            sendMessageRuleId,
                            sendMessageQueue.Id,
                            ExcelBugFix.GetInformation(queueReceivers.Select(p => p.Mobile).ToList()),
                            sendBody);

                        provider.SendSMS(sendMessageQueue.Id); // TODO: 在 EVA 測試, 要先註解這一行，避免發送簡訊

                    } // foreach (var sendBody in sendBodies)

                    // 全部簡訊發送完畢，將 SendMessageRule 狀態改變為 Sending (簡訊規則已發送完畢)
                    sendMessageRule.SendMessageRuleStatus = SendMessageRuleStatus.Sent;
                    this.unitOfWork.Repository<SendMessageRule>().Update(sendMessageRule);

                    // http://www.dotblogs.com.tw/rainmaker/archive/2015/08/19/153169.aspx

                    // 檢查此簡訊規則是否任務完成，
                    //  如果是的話，將狀態設為 Finish
                    //  如果否的話，將狀態設為 Ready (等待下次週期簡訊發送)

                    bool isFinish = !sendMessageRule.GetNextSendTime().HasValue;
                    sendMessageRule.SendMessageRuleStatus = isFinish ? SendMessageRuleStatus.Finish : SendMessageRuleStatus.Ready;
                    this.unitOfWork.Repository<SendMessageRule>().Update(sendMessageRule);

                    // 針對預先扣點的發送時間類型，進行黑名單的收訊者 - 退還點數
                    this.tradeService.HandleReceiversInBlackList(sendMessageRule, ruleReceiversInBlackList);

                    scope.Complete();
                }

                ////////////////////////////////////////
            }
            catch (Exception ex)
            {
                this.logService.Error(ex);
                throw;
            }
            finally
            {
                if (uniqueJob != null)
                    this.uniqueJobList.Remove(uniqueJob);
            }
        }

        [AutomaticRetry(Attempts = 0)]
        public void GetDeliveryReport(string requestId)
        {
            UniqueJob uniqueJob = null;

            try
            {
                uniqueJob = this.uniqueJobList.AddOrUpdate("GetDeliveryReport", requestId);
                if (uniqueJob == null) return; // Job 已經存在

                using (var scope = this.unitOfWork.CreateTransactionScope())
                {
                    var deliveryReportQueueRepository = this.unitOfWork.Repository<DeliveryReportQueue>();

                    var deliveryReportQueue = deliveryReportQueueRepository.Get(p => p.RequestId == requestId);
                    if (deliveryReportQueue == null) return;

                    ISmsProvider provider = GetProvider(deliveryReportQueue.ProviderName);

                    if (provider != null) // 取得DeliveryReport並沒有急迫性，等到簡訊供應商可供連線再取即可。
                    {
                        provider.GetDeliveryReport(requestId);
                    }

                    scope.Complete();
                }

                ////////////////////////////////////////
            }
            catch (Exception ex)
            {
                this.logService.Error("GetDeliveryReport({0})", requestId);
                this.logService.Error(ex);
                throw;
            }
            finally
            {
                if (uniqueJob != null)
                    this.uniqueJobList.Remove(uniqueJob);
            }
        }

        [AutomaticRetry(Attempts = 0)]
        public void RetrySMS(int sendMessageHistoryId)
        {
            UniqueJob uniqueJob = null;

            try
            {
                uniqueJob = this.uniqueJobList.AddOrUpdate("RetrySMS", sendMessageHistoryId);
                if (uniqueJob == null) return; // Job 已經存在

                using (var scope = this.unitOfWork.CreateTransactionScope())
                {
                    var sendMessageHistory = this.unitOfWork.Repository<SendMessageHistory>().GetById(sendMessageHistoryId);

                    decimal totalMessageCost = sendMessageHistory.MessageCost;

                    string currentProviderType = sendMessageHistory.ProviderName;

                    var providerTypesInOrder = GetSmsProviderTypes(currentProviderType);

                    // 由於使用預設的 Provider 無法發送成功，因此使用下一個 SmsProvider 發送
                    SmsProviderType userSmsProviderType = providerTypesInOrder[1];

                    ISmsProvider provider = GetProvider(userSmsProviderType, totalMessageCost); // 如果沒有找到，將會拋出例外

                    this.tradeService.RetrySMS(sendMessageHistory);

                    // 開始發送簡訊
                    provider.RetrySMS(sendMessageHistory.Id);

                    scope.Complete();
                }
            }
            catch (Exception ex)
            {
                this.logService.Error(ex);
                throw;
            }
            finally
            {
                if (uniqueJob != null)
                    this.uniqueJobList.Remove(uniqueJob);
            }
        }
    }
}