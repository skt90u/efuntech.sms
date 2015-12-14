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

namespace EFunTech.Sms.Portal.Controllers
{
	public class MemberSendMessageHistoryController : CrudApiController<MemberSendMessageHistoryCriteriaModel, SendMessageHistoryModel, SendMessageHistory, int>
	{
		public MemberSendMessageHistoryController(IUnitOfWork unitOfWork, ILogService logService)
			: base(unitOfWork, logService)
		{
		}

		protected override IOrderedQueryable<SendMessageHistory> DoGetList(MemberSendMessageHistoryCriteriaModel criteria)
		{
            IQueryable<SendMessageHistory> result = this.repository.GetAll();

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

            result = result.AsExpandable().Where(predicate);

			return result.OrderByDescending(p => p.Id);
		}

		protected override SendMessageHistory DoGet(int id)
		{
            throw new NotImplementedException();
		}

		protected override SendMessageHistory DoCreate(SendMessageHistoryModel model, SendMessageHistory entity, out int id)
		{
            throw new NotImplementedException();
		}

		protected override void DoUpdate(SendMessageHistoryModel model, int id, SendMessageHistory entity)
		{
            throw new NotImplementedException();
		}

		protected override void DoRemove(int id, SendMessageHistory entity)
		{
            throw new NotImplementedException();
		}

		protected override void DoRemove(List<int> ids, List<SendMessageHistory> entities)
		{
            throw new NotImplementedException();
		}

        protected override ReportDownloadModel ProduceFile(MemberSendMessageHistoryCriteriaModel criteria, List<SendMessageHistoryModel> resultList)
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
