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
using EFunTech.Sms.Core;
using System.Data.Entity;

namespace EFunTech.Sms.Portal.Controllers
{
	public class DeliveryReportQueueController : AsyncCrudApiController<DeliveryReportQueueCriteriaModel, DeliveryReportQueueModel, DeliveryReportQueue, int>
	{
        public DeliveryReportQueueController(DbContext context, ILogService logService)
            : base(context, logService)
        {
        }

        protected override IQueryable<DeliveryReportQueue> DoGetList(DeliveryReportQueueCriteriaModel criteria)
		{
			var predicate = PredicateBuilder.True<DeliveryReportQueue>();

            predicate = predicate.And(p => p.CreatedTime >= criteria.StartDate);
            predicate = predicate.And(p => p.CreatedTime <= criteria.EndDate);

			var searchText = criteria.SearchText;
			if (!string.IsNullOrEmpty(searchText))
			{
				var innerPredicate = PredicateBuilder.False<DeliveryReportQueue>();

				innerPredicate = innerPredicate.Or(p => !string.IsNullOrEmpty(p.RequestId) && p.RequestId.Contains(searchText));
				innerPredicate = innerPredicate.Or(p => !string.IsNullOrEmpty(p.ProviderName) && p.ProviderName.Contains(searchText));

				predicate = predicate.And(innerPredicate);
			}

            var result = context.Set<DeliveryReportQueue>()
                            .AsExpandable()
                            .Where(predicate)
                            .OrderByDescending(p => p.Id);

            return result;
		}

        protected override ReportDownloadModel ProduceFile(DeliveryReportQueueCriteriaModel criteria, IEnumerable<DeliveryReportQueueModel> resultList)
        {
            TimeSpan clientTimezoneOffset = ClientTimezoneOffset;
            string timeFormat = Converter.Every8d_SentTime;

            var result = resultList.Select(p => new
            {
                簡訊序列編號 = p.SendMessageQueueId,
                簡訊識別碼 = p.RequestId,
                簡訊供應商 = p.ProviderName,
                建立時間 = Converter.ToLocalTimeString(p.CreatedTime, clientTimezoneOffset, timeFormat),
                已發送 = p.SendMessageResultItemCount,
                已接收 = p.DeliveryReportCount,
            });

            return ProduceExcelFile(
                fileName: "接收簡訊發送結果序列.xlsx",
                sheetName: "接收簡訊發送結果序列",
                models: result);
        }
	}
}
