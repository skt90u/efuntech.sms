using EFunTech.Sms.Portal.Models;
using EFunTech.Sms.Schema;
using System.Linq;
using EFunTech.Sms.Portal.Controllers.Common;
using EFunTech.Sms.Portal.Models.Common;
using JUtilSharp.Database;

using System.Collections.Generic;
using LinqKit;
using System;
using EFunTech.Sms.Portal.Models.Criteria;
using System.Data;
using System.ComponentModel;
using EFunTech.Sms.Core;

namespace EFunTech.Sms.Portal.Controllers
{
	public class MemberSendMessageStatisticController : CrudApiController<MemberSendMessageStatisticCriteriaModel, SendMessageStatisticModel, SendMessageStatistic, int>
	{
		public MemberSendMessageStatisticController(IUnitOfWork unitOfWork, ILogService logService)
			: base(unitOfWork, logService)
		{
		}

		protected override IOrderedQueryable<SendMessageStatistic> DoGetList(MemberSendMessageStatisticCriteriaModel criteria)
		{
            // 目前使用者的資料
            IQueryable<SendMessageStatistic> result = this.repository.GetMany(p => p.CreatedUserId == CurrentUser.Id).AsQueryable();

            var sendMessageHistoryRepository = this.unitOfWork.Repository<SendMessageHistory>();

            var predicate = PredicateBuilder.True<SendMessageStatistic>();

            //this.logService.Debug("MemberSendMessageStatistic.DoGetList 查詢時間範圍 = {0} ~ {1}", Converter.DebugString(criteria.StartDate), Converter.DebugString(criteria.EndDate));

            predicate = predicate.And(p => p.SendTime >= criteria.StartDate);
            predicate = predicate.And(p => p.SendTime <= criteria.EndDate);

            // 依門號
            if (!string.IsNullOrEmpty(criteria.Mobile))
            {
                string Mobile = criteria.Mobile;
                if (MobileUtil.IsPossibleNumber(Mobile))
                    Mobile = MobileUtil.GetE164PhoneNumber(Mobile);

                var sendMessageQueueIds = sendMessageHistoryRepository.GetMany(p => p.CreatedUserId == CurrentUser.Id &&
                                                                                    p.DestinationAddress.Contains(Mobile) &&
                                                                                    p.SendTime >= criteria.StartDate && // 20151126 Norman, 避免StackOverFlow, 縮小範圍, 只取實際需要的
                                                                                    p.SendTime <= criteria.EndDate) // 20151126 Norman, 避免StackOverFlow, 縮小範圍, 只取實際需要的
                                                                      .Select(p => p.SendMessageQueueId)
                                                                      .Distinct()
                                                                      .ToList();

                //var innerPredicate = PredicateBuilder.False<SendMessageStatistic>();

                //foreach (var sendMessageQueueId in sendMessageQueueIds)
                //{
                //    innerPredicate = innerPredicate.Or(p => p.SendMessageQueueId == sendMessageQueueId);
                //}

                //predicate = predicate.And(innerPredicate);

                predicate.And(p => sendMessageQueueIds.Contains(p.SendMessageQueueId)); // 20151126 Norman, 避免StackOverFlow
            }

            // 依接收狀態查詢
            if (!string.IsNullOrEmpty(criteria.ReceiptStatus))
            {
                var arrReceiptStatus = criteria.ReceiptStatus.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);

                if (arrReceiptStatus.Length != 0)
                {
                    var _predicate = PredicateBuilder.False<SendMessageHistory>();

                    foreach (string receiptStatus in arrReceiptStatus)
                    {
                        var status = (DeliveryReportStatus)Convert.ToInt32(receiptStatus);
                        _predicate = _predicate.Or(p => p.DeliveryStatus == status);
                    }

                    var sendMessageQueueIds = sendMessageHistoryRepository.GetMany(p => p.CreatedUserId == CurrentUser.Id &&
                                                                                        p.SendTime >= criteria.StartDate && // 20151126 Norman, 避免StackOverFlow, 縮小範圍, 只取實際需要的
                                                                                        p.SendTime <= criteria.EndDate) // 20151126 Norman, 避免StackOverFlow, 縮小範圍, 只取實際需要的
                                                                          .AsExpandable()
                                                                          .Where(_predicate)
                                                                          .Select(p => p.SendMessageQueueId)
                                                                          .Distinct()
                                                                          .ToList();

                    //var innerPredicate = PredicateBuilder.False<SendMessageStatistic>();

                    //foreach (var sendMessageQueueId in sendMessageQueueIds)
                    //{
                    //    innerPredicate = innerPredicate.Or(p => p.SendMessageQueueId == sendMessageQueueId);
                    //}

                    //predicate = predicate.And(innerPredicate);

                    predicate.And(p => sendMessageQueueIds.Contains(p.SendMessageQueueId)); // 20151126 Norman, 避免StackOverFlow
                }

                // 傳送中   
                // MessageWaiting = 1004,
                // DeliveredToNetwork = 1005,
                // Sending = 0,
                // 成功  
                // DeliveredToTerminal = 1001,
                // Sent = 100,
                // 空號   
                // PhoneNumberNotAvailable = 103,
                // 電話號碼格式錯誤  
                // WrongPhoneNumber = -3,
                // 逾時
                // DeliveryUncertain = 1002,
                // DeliveryImpossible = 1003,
                // TerminalUncertain = 101,
                // NetworkUncertain102 = 102,
                // NetworkUncertain104 = 104,
                // NetworkUncertain105 = 105,
                // NetworkUncertain106 = 106,
                // Timeout107 = 107,
                // Timeout_1 = -1,
                // Timeout_2 = -2,
                // Timeout_4 = -4,
                // Timeout_5 = -5,
                // Timeout_6 = -6,
                // Timeout_8 = -8,
                // Timeout_9 = -9,
                // Timeout_32 = -32,
                // Timeout_100 = -100,
                // Timeout_101 = -101,
                // Timeout_201 = -201,
                // Timeout_202 = -202,
                // Timeout_203 = -203,
            }

            result = result.AsExpandable().Where(predicate);

            return result.OrderByDescending(p => p.Id);
		}

		protected override SendMessageStatistic DoGet(int id)
		{
            return this.repository.GetById(id);
		}

		protected override SendMessageStatistic DoCreate(SendMessageStatisticModel model, SendMessageStatistic entity, out int id)
		{
            throw new NotImplementedException();
		}

		protected override void DoUpdate(SendMessageStatisticModel model, int id, SendMessageStatistic entity)
		{
            throw new NotImplementedException();
		}

		protected override void DoRemove(int id, SendMessageStatistic entity)
		{
            throw new NotImplementedException();
		}

		protected override void DoRemove(List<int> ids, List<SendMessageStatistic> entities)
		{
            throw new NotImplementedException();
		}

        protected override IEnumerable<SendMessageStatisticModel> ConvertModel(IEnumerable<SendMessageStatisticModel> models)
        {
            var deaprtmentRepository = this.unitOfWork.Repository<Department>();
            var userRepository = this.unitOfWork.Repository<ApplicationUser>();

            int rowNo = 0;

            foreach (var model in models)
            {
                model.RowNo = ++rowNo;
            }

            return models;
        }

        protected override ReportDownloadModel ProduceFile(MemberSendMessageStatisticCriteriaModel criteria, List<SendMessageStatisticModel> resultList)
        {
            TimeSpan clientTimezoneOffset = ClientTimezoneOffset;
            string timeFormat = Converter.Every8d_SentTime;

            var result = resultList.Select(p => new
            {
                訊息類型 = AttributeHelper.GetColumnDescription(p.SendMessageType),
                發送時間 = Converter.ToLocalTimeString(p.SendTime, clientTimezoneOffset, timeFormat),
                簡訊類別描述 = p.SendTitle,
                發送內容 = p.SendBody,
                發送通數 = p.TotalReceiverCount,
                成功接收 = p.TotalSuccess,
                傳送中 = p.TotalSending,
                //逾期收訊 = p.TotalTimeout,
                傳送失敗 = p.TotalTimeout,
                發送扣點 = p.TotalMessageCost,
            });

            return ProduceZipFile(
                fileName: "發送統計紀錄.zip",
                zipEntries: new Dictionary<string, DataTable> 
                { 
                      { "發送統計紀錄", Converter.ToDataTable(result.ToList()) },
                }
            );
        }
        

	}
}
