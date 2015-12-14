using EFunTech.Sms.Schema;
using JUtilSharp.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EFunTech.Sms.Portal
{
    public class SendMessageStatisticService
    {
        private ILogService logService;
        private IUnitOfWork unitOfWork;

        public SendMessageStatisticService(ILogService logService, IUnitOfWork unitOfWork)
        {
            this.logService = logService;
            this.unitOfWork = unitOfWork;
        }

        public void AddOrUpdateSendMessageStatistic(int sendMessageQueueId)
        {
            // 重新寫入統計表
            this.unitOfWork.Repository<SendMessageStatistic>().Delete(p => p.SendMessageQueueId == sendMessageQueueId);

            DateTime utcNow = DateTime.UtcNow;

            var sendMessageHistoryRepository = this.unitOfWork.Repository<SendMessageHistory>();

            var histories = sendMessageHistoryRepository.GetMany(p => p.SendMessageQueueId == sendMessageQueueId).ToList();

            // http://stackoverflow.com/questions/6248926/linq-to-sql-sum-inside-a-select-new
            // http://stackoverflow.com/questions/847066/group-by-multiple-columns

            var entities = histories.GroupBy(p => new
            {
                p.DepartmentId,
                p.CreatedUserId,
                p.SendMessageRuleId,
                p.SendMessageQueueId,
                p.SendMessageType,

                p.SendTime,
                p.SendTitle,
                p.SendBody,
                p.SendCustType,
                p.RequestId,

                p.ProviderName
            })
            .Select(g => new 
            {
                DepartmentId = g.Key.DepartmentId,
                CreatedUserId = g.Key.CreatedUserId,
                SendMessageRuleId = g.Key.SendMessageRuleId,
                SendMessageQueueId = g.Key.SendMessageQueueId,
                SendMessageType = g.Key.SendMessageType,

                SendTime = g.Key.SendTime,
                SendTitle = g.Key.SendTitle,
                SendBody = g.Key.SendBody,
                SendCustType = g.Key.SendCustType,
                RequestId = g.Key.RequestId,

                ProviderName = g.Key.ProviderName,

                // 發送通數
                TotalReceiverCount = g.Count(),
                // 花費點數
                TotalMessageCost = g.Sum(p => p.MessageCost),
                // 成功接收
                TotalSuccess = g.Count(p => p.Delivered),
                // 傳送中通數
                TotalSending = g.Count(p =>
                    // 20151119 Norman, 尚未取得派送結果
                    p.DeliveryStatus == DeliveryReportStatus.MessageAccepted || 
                    // infobip
                    // 20151121 Norman, 將 Infobip 所有不是 DeliveredToTerminal 的狀態都判定為逾時
                    //p.DeliveryStatus == DeliveryReportStatus.MessageWaiting ||
                    //p.DeliveryStatus == DeliveryReportStatus.DeliveredToNetwork ||
                    // every8d
                    p.DeliveryStatus == DeliveryReportStatus.Sending),
                // 逾期收訊
                TotalTimeout = g.Count(p =>
                    // 20151119 Norman, 無法取得派送結果(超過一天)
                    p.DeliveryStatus == DeliveryReportStatus.DeliveryReportTimeout || 
                    // infobip
                    p.DeliveryStatus == DeliveryReportStatus.DeliveryUncertain ||
                    p.DeliveryStatus == DeliveryReportStatus.DeliveryImpossible ||
                    p.DeliveryStatus == DeliveryReportStatus.MessageWaiting ||
                    p.DeliveryStatus == DeliveryReportStatus.DeliveredToNetwork ||
                        // every8d
                    p.DeliveryStatus == DeliveryReportStatus.TerminalUncertain ||
                    p.DeliveryStatus == DeliveryReportStatus.NetworkUncertain102 ||
                    p.DeliveryStatus == DeliveryReportStatus.NetworkUncertain104 ||
                    p.DeliveryStatus == DeliveryReportStatus.NetworkUncertain105 ||
                    p.DeliveryStatus == DeliveryReportStatus.NetworkUncertain106 ||
                    p.DeliveryStatus == DeliveryReportStatus.Timeout107 ||
                    p.DeliveryStatus == DeliveryReportStatus.Timeout_1 ||
                    p.DeliveryStatus == DeliveryReportStatus.Timeout_2 ||
                    p.DeliveryStatus == DeliveryReportStatus.Timeout_4 ||
                    p.DeliveryStatus == DeliveryReportStatus.Timeout_5 ||
                    p.DeliveryStatus == DeliveryReportStatus.Timeout_6 ||
                    p.DeliveryStatus == DeliveryReportStatus.Timeout_8 ||
                    p.DeliveryStatus == DeliveryReportStatus.Timeout_9 ||
                    p.DeliveryStatus == DeliveryReportStatus.Timeout_32 ||
                    p.DeliveryStatus == DeliveryReportStatus.Timeout_100 ||
                    p.DeliveryStatus == DeliveryReportStatus.Timeout_101 ||
                    p.DeliveryStatus == DeliveryReportStatus.Timeout_201 ||
                    p.DeliveryStatus == DeliveryReportStatus.Timeout_202 ||
                    p.DeliveryStatus == DeliveryReportStatus.Timeout_203 ||
                        // every8d 門號問題
                    p.DeliveryStatus == DeliveryReportStatus.PhoneNumberNotAvailable ||
                    p.DeliveryStatus == DeliveryReportStatus.WrongPhoneNumber),
                // 資料建立時間
                CreatedTime = utcNow
            }).ToList()
            .Select(p => new SendMessageStatistic
            {
                DepartmentId = p.DepartmentId,
                CreatedUserId = p.CreatedUserId,
                SendMessageRuleId = p.SendMessageRuleId,
                SendMessageQueueId = p.SendMessageQueueId,
                SendMessageType = p.SendMessageType,

                SendTime = p.SendTime,
                SendTitle = p.SendTitle,
                SendBody = p.SendBody,
                SendCustType = p.SendCustType,
                RequestId = p.RequestId,

                ProviderName = p.ProviderName,

                // 發送通數
                TotalReceiverCount = p.TotalReceiverCount,
                // 花費點數
                TotalMessageCost = p.TotalMessageCost,
                // 成功接收
                TotalSuccess = p.TotalSuccess,
                // 傳送中通數
                TotalSending = p.TotalSending,
                // 逾期收訊
                TotalTimeout = p.TotalTimeout,
                // 資料建立時間
                CreatedTime = p.CreatedTime,
            });

            foreach (var entity in entities)
            {
                this.unitOfWork.Repository<SendMessageStatistic>().Insert(entity);
            }
        }
    }
}