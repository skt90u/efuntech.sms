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
using EFunTech.Sms.Core;
using System.Data.Entity;

namespace EFunTech.Sms.Portal.Controllers
{
	public class MemberSendMessageHistoryController : CrudApiController<MemberSendMessageHistoryCriteriaModel, SendMessageHistoryModel, SendMessageHistory, int>
	{
        public MemberSendMessageHistoryController(DbContext context, ILogService logService)
            : base(context, logService)
        {
        }

        protected override IQueryable<SendMessageHistory> DoGetList(MemberSendMessageHistoryCriteriaModel criteria)
		{
			var predicate = PredicateBuilder.True<SendMessageHistory>();

            predicate = predicate.And(p => p.SendMessageQueueId == criteria.SendMessageQueueId);

            // 依接收狀態查詢
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

                    predicate = predicate.And(_predicate);
                }
            }

            var result = context.Set<SendMessageHistory>()
                            .AsExpandable()
                            .Where(predicate)
                            .OrderByDescending(p => p.Id);

			return result;
		}

        protected override ReportDownloadModel ProduceFile(MemberSendMessageHistoryCriteriaModel criteria, IEnumerable<SendMessageHistoryModel> resultList)
        {
            TimeSpan clientTimezoneOffset = ClientTimezoneOffset;
            string timeFormat = Converter.Every8d_SentTime;

            var result = resultList.Select(p => new
            {
                收訊者姓名 = p.DestinationName,
                收訊者門號 = p.DestinationAddress,
                發送地區 = p.Region,
                收訊內容 = p.SendBody,
                收訊狀態 = p.DeliveryStatusString,
                收訊時間 = Converter.ToLocalTimeString(p.SentDate, clientTimezoneOffset, timeFormat),
                發送扣點 = p.MessageCost,
            });

            return ProduceZipFile(
                fileName: "發送統計明細紀錄.zip",
                zipEntries: new Dictionary<string, DataTable> 
                { 
                      { "發送統計明細紀錄", Converter.ToDataTable(result.ToList()) },
                }
            );
        }
	}
}
