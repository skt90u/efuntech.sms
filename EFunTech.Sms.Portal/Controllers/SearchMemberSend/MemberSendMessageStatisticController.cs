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
using System.Data.Entity;

namespace EFunTech.Sms.Portal.Controllers
{
	public class MemberSendMessageStatisticController : AsyncCrudApiController<MemberSendMessageStatisticCriteriaModel, SendMessageStatisticModel, SendMessageStatistic, int>
	{
        public MemberSendMessageStatisticController(DbContext context, ILogService logService)
            : base(context, logService)
        {
        }

        protected override IQueryable<SendMessageStatistic> DoGetList(MemberSendMessageStatisticCriteriaModel criteria)
		{
            var predicate = PredicateBuilder.True<SendMessageStatistic>();
            predicate = predicate.And(p => p.CreatedUserId == CurrentUserId);
            predicate = predicate.And(p => p.SendTime >= criteria.StartDate);
            predicate = predicate.And(p => p.SendTime <= criteria.EndDate);

            if (!string.IsNullOrEmpty(criteria.Mobile) ||
                !string.IsNullOrEmpty(criteria.ReceiptStatus))
            {
                var innerPredicate = PredicateBuilder.True<SendMessageHistory>();
                innerPredicate = innerPredicate.And(p => p.CreatedUserId == CurrentUserId);
                innerPredicate = innerPredicate.And(p => p.SendTime >= criteria.StartDate);
                innerPredicate = innerPredicate.And(p => p.SendTime <= criteria.EndDate);

                if (!string.IsNullOrEmpty(criteria.Mobile))
                {
                    var _predicate = PredicateBuilder.False<SendMessageHistory>();

                    var arrMobile = criteria.Mobile.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);

                    if (arrMobile.Length != 0)
                    {
                        foreach (string mobile in arrMobile)
                        {
                            string mobileKeyword = mobile;

                            if (MobileUtil.IsPossibleNumber(mobileKeyword))
                                mobileKeyword = MobileUtil.GetE164PhoneNumber(mobileKeyword);

                            _predicate = _predicate.Or(p => p.DestinationAddress.Contains(mobileKeyword));
                        }

                        innerPredicate = innerPredicate.And(_predicate);
                    }
                }

                if (!string.IsNullOrEmpty(criteria.ReceiptStatus))
                {
                    var _predicate = PredicateBuilder.False<SendMessageHistory>();

                    var arrReceiptStatus = criteria.ReceiptStatus.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);

                    if (arrReceiptStatus.Length != 0)
                    {
                        foreach (string receiptStatus in arrReceiptStatus)
                        {
                            var status = (DeliveryReportStatus)Convert.ToInt32(receiptStatus);
                            _predicate = _predicate.Or(p => p.DeliveryStatus == status);
                        }

                        innerPredicate = innerPredicate.And(_predicate);
                    }
                }

                var sendMessageQueueIds = context.Set<SendMessageHistory>()
                                            .AsExpandable()
                                            .Where(innerPredicate)
                                            .Select(p => p.SendMessageQueueId)
                                            .Distinct()
                                            .ToList();

                predicate = predicate.And(p => sendMessageQueueIds.Contains(p.SendMessageQueueId)); // 20151126 Norman, 避免StackOverFlow
            }

            // 目前使用者的資料
            var result = context.Set<SendMessageStatistic>()
                            .AsExpandable()
                            .Where(predicate)
                            .OrderByDescending(p => p.Id);

            return result;
		}

        //protected override IEnumerable<SendMessageStatisticModel> ConvertModel(IEnumerable<SendMessageStatisticModel> models)
        //{
        //    var deaprtmentRepository = this.unitOfWork.Repository<Department>();
        //    var userRepository = this.unitOfWork.Repository<ApplicationUser>();

        //    int rowNo = 0;

        //    foreach (var model in models)
        //    {
        //        model.RowNo = ++rowNo;
        //    }

        //    return models;
        //}

        protected override ReportDownloadModel ProduceFile(MemberSendMessageStatisticCriteriaModel criteria, IEnumerable<SendMessageStatisticModel> resultList)
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
