using EFunTech.Sms.Core;
using EFunTech.Sms.Portal.Services;
using EFunTech.Sms.Schema;
using Hangfire;
using JUtilSharp.Database;
using LinqKit;
using OneApi.Client.Impl;
using OneApi.Config;
using OneApi.Model;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace EFunTech.Sms.Portal
{
    /// <summary>
    /// EFuntech Sms Background Job
    /// </summary>
    public class EfSmsBackgroundJob
    {
        private ISystemParameters systemParameters;
        private IUnitOfWork unitOfWork;
        private ILogService logService;
        private TradeService tradeService;
        private UniqueJobList uniqueJobList;
        private SendMessageStatisticService sendMessageStatisticService;

        public EfSmsBackgroundJob(ISystemParameters systemParameters, IUnitOfWork unitOfWork, ILogService logService)
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");
            System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("en-US");

            this.systemParameters = systemParameters;
            this.unitOfWork = unitOfWork;
            this.logService = logService;
            this.tradeService = new TradeService(unitOfWork, logService);

            this.uniqueJobList = new UniqueJobList(systemParameters, logService, unitOfWork);
            this.sendMessageStatisticService = new SendMessageStatisticService(logService, unitOfWork);
        }

        #region 每月定期撥入點數給子帳號
        
        /// <summary>
        /// 每月定期撥入點數給子帳號
        /// </summary>
        public void CheckMonthlyAllotPoint()
        {
            try
            {
                DateTime utcNow = DateTime.UtcNow;

                var users = this.unitOfWork.Repository<ApplicationUser>().GetMany(p =>
                    p.AllotSetting != null &&
                    p.AllotSetting.MonthlyAllot == true &&
                    p.AllotSetting.MonthlyAllotDay == utcNow.Day).ToList();

                if (!users.Any())
                {
                    // this.logService.Debug("無任何使用者須滿足每月定期撥點觸發條件");
                    return;
                }

                this.logService.Debug("處理每月定期撥點(使用者：[{0}])", string.Join("、", users.Select(p => p.UserName)));

                foreach (var user in users)
                {
                    tradeService.MonthlyAllotPoint(user);
                }
            }
            catch (Exception ex)
            {
                this.logService.Error(ex);
                throw;
            }
        }

        #endregion

        #region 派送簡訊工作(預約或者週期簡訊)


        /// <summary>
        /// 簡訊準備時間
        ///     設定需要提早多少時間，準備進行簡訊發送動作
        /// </summary>
        private static readonly TimeSpan SmsPreparationTimeSpan = new TimeSpan(0, 0, 0, 0, milliseconds: 60 * 1000);

        /// <summary>
        /// 簡訊預計發送時間差
        ///     設定經由我們的簡訊平台發送簡訊請求 -> Infobip | Every8d -> 客戶端手機需要經過多久時間
        /// </summary>
        private static readonly TimeSpan SmsLatencyTimeSpan = new TimeSpan(0, 0, 0, 0, milliseconds: 0 * 1000);

        //       目前時間      實際發送時間             預定發送時間                  客戶端收訊時間
        // ---------|------------------|-------------------------|------------------------------|-------
        //          |             SmsPreparationTimeSpan         |
        //                             |   SmsLatencyTimeSpan    |
        //          |   DelayTimeSpan  |

        /// <summary>
        /// 派送簡訊工作(預約或者週期簡訊)
        /// </summary>
        public void SendSMS()
        {
            try
            {
                // 對於預約發送簡訊，須滿足以下條件才會發送簡訊
                //  (1) SendMessageRuleStatus == SendMessageRuleStatus.Ready
                //  (2) 預計發送時間必須小於等於目前時間
                //  (3) 在 SendMessageQueue 沒有對應資料 (根據 SendMessageRuleId)

                // 對於週期發送簡訊，須滿足以下條件才會發送簡訊
                //  (1) SendMessageRuleStatus == SendMessageRuleStatus.Ready
                //  (2) 預計發送時間必須小於等於目前時間
                //  (3) 在 SendMessageQueue 最大一筆(ORDER BY SendTime DESC) 對應資料的發送時間，比預計發送時間小

                var rules = this.unitOfWork.Repository<SendMessageRule>().GetMany(p =>
                    p.SendMessageRuleStatus == SendMessageRuleStatus.Ready &&
                    (p.SendTimeType == SendTimeType.Deliver || p.SendTimeType == SendTimeType.Cycle)).ToList();

                var sendMessageQueueRepository = this.unitOfWork.Repository<SendMessageQueue>();

                DateTime utcNow = DateTime.UtcNow;

                if (!rules.Any())
                {
                    //this.logService.Debug("無任何待發送預約簡訊或週期簡訊");
                    return;
                }

                this.logService.Debug("待發送簡訊(預約簡訊：{0} 個，週期簡訊：{1} 個)"
                    , rules.Where(p => p.SendTimeType == SendTimeType.Deliver).Count()
                    , rules.Where(p => p.SendTimeType == SendTimeType.Cycle).Count()
                    );

                foreach (var rule in rules)
                {
                    // 預約發送的發送時間，不會是 null
                    // 週期發送的發送時間，如果為 null，表示找不到比目前時間小的發送時間
                    DateTime? nullableTime = rule.GetSendTime();
                    if (nullableTime.HasValue == false) continue;

                    DateTime sendTime = nullableTime.Value;

                    //  (2) 預計發送時間必須小於等於目前時間，因此不滿足此條件就忽略
                    //if (sendTime > utcNow) continue;

                    //  (2) 預計發送時間必須小於等於(目前時間 + 簡訊準備時間)，因此不滿足此條件就忽略
                    if (sendTime > utcNow + SmsPreparationTimeSpan) continue;

                    //BackgroundJob.Enqueue<CommonSmsService>(x => x.SendSMS(rule.Id, sendTime));

                    var delayTimeSpan = (sendTime - utcNow) - SmsLatencyTimeSpan;
                    var delayMilliseconds = (int)delayTimeSpan.TotalMilliseconds;

                    FaFTaskFactory.StartNew(delayMilliseconds, () =>
                    {
                        BackgroundJob.Enqueue<CommonSmsService>(x => x.SendSMS(rule.Id, sendTime));
                    });
                }
            }
            catch (Exception ex)
            {
                this.logService.Error(ex);
                throw;
            }
        }

        #endregion

        #region 定時接收簡訊回傳結果

        /// <summary>
        /// 定時接收簡訊回傳結果
        /// </summary>
        public void GetDeliveryReport()
        {
            try
            {
                var predicate = PredicateBuilder.True<DeliveryReportQueue>();

                // 避免 Migration 錯誤
                //{
                //    var innerPredicate = PredicateBuilder.False<DeliveryReportQueue>();

                //    var sendMessageQueueIds = this.unitOfWork.Repository<SendMessageQueue>().GetAll().Select(p => p.Id).ToList();
                //    foreach (var sendMessageQueueId in sendMessageQueueIds)
                //    {
                //        innerPredicate = innerPredicate.Or(p => p.SendMessageQueueId == sendMessageQueueId);
                //    }

                //    predicate = predicate.And(innerPredicate); // 會造成無窮迴圈，WHY???
                //} 

                // 尚未接收完成
                {
                    predicate = predicate.And(p => p.DeliveryReportCount < p.SendMessageResultItemCount);
                }

                // 尚未過期
                {
                    var expiryDate = DateTime.UtcNow.AddTicks(-1 * DeliveryReportQueue.QueryInterval.Ticks);

                    predicate = predicate.And(p => p.CreatedTime >= expiryDate); // 尚未過期
                }

                var queues = this.unitOfWork.Repository<DeliveryReportQueue>()
                    .GetAll()
                    .AsExpandable()
                    .Where(predicate)
                    .OrderByDescending(p => p.CreatedTime)
                    .ToList();

                if (!queues.Any()) return;

                var queuesInfos = queues
                    .ToList()
                    .Select(p => string.Format("{{ 簡訊發送識別碼:{0}, 已發送: {1}, 已接收:{2}}}", p.RequestId, p.SendMessageResultItemCount, p.DeliveryReportCount))
                    .ToList();

                this.logService.Debug("等待接收簡訊發送結果序列：{0})", ExcelBugFix.GetInformation(queuesInfos));

                foreach (var queue in queues)
                {
                    BackgroundJob.Enqueue<CommonSmsService>(x => x.GetDeliveryReport(queue.RequestId));
                }
            }
            catch (Exception ex)
            {
                this.logService.Error(ex);
                throw;
            }
        }

        #endregion

        #region 處理一天之後，尚未取得派送結果的情況

        /// <summary>
        /// 若一天之後，SendMessageHistory 狀態仍然為 MessageAccepted (仍未取得派送結果)，
        /// 則將此筆 SendMessageHistory 狀態改成 DeliveryReportTimeout
        /// </summary>
        public void HandleDeliveryReportTimeout()
        {
            UniqueJob uniqueJob = this.uniqueJobList.AddOrUpdate("HandleDeliveryReportTimeout");
            if (uniqueJob == null) return; // Job 已經存在

            try
            {
                using (var scope = this.unitOfWork.CreateTransactionScope())
                {
                    // 若一天之後，SendMessageHistory 狀態仍然為 MessageAccepted (仍未取得派送結果)，
                    // 則將此筆 SendMessageHistory 狀態改成 DeliveryReportTimeout

                    var sendMessageRuleRepository = this.unitOfWork.Repository<SendMessageRule>();
                    var sendMessageQueueRepository = this.unitOfWork.Repository<SendMessageQueue>();
                    var sendMessageHistoryRepository = this.unitOfWork.Repository<SendMessageHistory>();

                    var expiryDate = DateTime.UtcNow.AddTicks(-1 * DeliveryReportQueue.QueryInterval.Ticks);

                    var entities = sendMessageHistoryRepository
                        .GetMany(p =>
                            (p.DeliveryStatus == DeliveryReportStatus.MessageAccepted || // Infobip
                             p.DeliveryStatus == DeliveryReportStatus.Sending) && // Every8d
                                // 已經過期
                            p.CreatedTime < expiryDate)
                        .ToList();

                    var sendMessageQueueIds = entities.Select(p => p.SendMessageQueueId).Distinct().ToList();
                    var sendMessageRuleIds = entities.Select(p => p.SendMessageRuleId).Distinct().ToList();
                    var sendMessageQueues = sendMessageQueueRepository.GetMany(p => sendMessageQueueIds.Contains(p.Id)).ToList();
                    var sendMessageRules = sendMessageRuleRepository.GetMany(p => sendMessageRuleIds.Contains(p.Id)).ToList();

                    foreach (var entity in entities)
                    {
                        var oldDeliveryStatus = entity.DeliveryStatus;
                        var newDeliveryStatus = DeliveryReportStatus.DeliveryReportTimeout;
                        var sendMessageQueue = sendMessageQueues.Find(p => p.Id == entity.SendMessageQueueId);
                        var sendMessageRule = sendMessageRules.Find(p => p.Id == entity.SendMessageRuleId);

                        this.logService.Debug("{0}(簡訊編號：{1}，序列編號:{2})，收訊門號{3}已超過{4}未經收到派送結果，接收狀態將由{5}改成{6}",
                            AttributeHelper.GetColumnDescription(sendMessageRule.SendTimeType),
                            sendMessageRule.Id,
                            sendMessageQueue.Id,
                            entity.DestinationAddress,
                            DeliveryReportQueue.QueryInterval.ToString(),
                            oldDeliveryStatus.ToString(),
                            newDeliveryStatus.ToString());

                        entity.DeliveryStatus = newDeliveryStatus;
                        entity.DeliveryStatusString = entity.DeliveryStatus.ToString();
                        entity.Delivered = false;
                        sendMessageHistoryRepository.Update(entity);

                        // 如果發送失敗，就回補點數
                        // 20151123 Norman, Eric 要求發送失敗不回補點數
                        //tradeService.HandleSendMessageHistory(sendMessageRule, sendMessageQueue, entity);
                    }

                    foreach (var sendMessageQueueId in sendMessageQueueIds)
                    {
                        this.logService.Debug("簡訊序列編號:{0})，重新計算 SendMessageStatistic", sendMessageQueueId);

                        this.sendMessageStatisticService.AddOrUpdateSendMessageStatistic(sendMessageQueueId);
                    }

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
                this.uniqueJobList.Remove(uniqueJob);
            }
        }

        #endregion



        #region 重試發送失敗的簡訊

        public void RetrySMS()
        {
            try
            {
                var sendMessageHistoryRepository = this.unitOfWork.Repository<SendMessageHistory>();

                var predicate = PredicateBuilder.True<SendMessageHistory>();
                // 發送失敗
                predicate = predicate.And(p => p.Delivered == false && p.DeliveryStatus != DeliveryReportStatus.MessageAccepted && p.DeliveryStatus != DeliveryReportStatus.DeliveredToTerminal);
                // 未超過最大重送次數
                predicate = predicate.And(p => p.RetryMaxTimes > p.RetryTotalTimes);

                var innerPredicate = PredicateBuilder.False<SendMessageHistory>();
                // 尚未重送過
                innerPredicate = innerPredicate.Or(p => p.SendMessageRetryHistory == null);
                // 最後一次重送，仍然發送失敗
                innerPredicate = innerPredicate.Or(p => p.SendMessageRetryHistory != null &&
                                                        p.SendMessageRetryHistory.Delivered == false && p.DeliveryStatus != DeliveryReportStatus.MessageAccepted && p.DeliveryStatus != DeliveryReportStatus.DeliveredToTerminal);

                predicate = predicate.And(innerPredicate);

                var sendMessageHistorys = sendMessageHistoryRepository.GetAll().AsExpandable().Where(predicate);

                foreach (var sendMessageHistory in sendMessageHistorys)
                {
                    FaFTaskFactory.StartNew(() =>
                    {
                        BackgroundJob.Enqueue<CommonSmsService>(x => x.RetrySMS(sendMessageHistory.Id));
                    });
                }
            }
            catch (Exception ex)
            {
                this.logService.Error(ex);
                throw;
            }
        }

        #endregion
    
        
        #region HouseKeeping

        public void HouseKeeping()
        {
            var expiryDate = DateTime.UtcNow.AddMonths(-1);

            // 保留一個月的資料
            this.unitOfWork.Repository<LogItem>().Delete(p => p.CreatedTime < expiryDate);
        }

        #endregion
    
    }
}