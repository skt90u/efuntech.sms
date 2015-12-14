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

namespace EFunTech.Sms.Portal.Controllers
{
	public class DeliveryReportQueueController : CrudApiController<DeliveryReportQueueCriteriaModel, DeliveryReportQueueModel, DeliveryReportQueue, int>
	{
		public DeliveryReportQueueController(IUnitOfWork unitOfWork, ILogService logService)
			: base(unitOfWork, logService)
		{
		}

		protected override IOrderedQueryable<DeliveryReportQueue> DoGetList(DeliveryReportQueueCriteriaModel criteria)
		{
            IQueryable<DeliveryReportQueue> result = this.repository.GetAll().AsQueryable();

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
			result = result.AsExpandable().Where(predicate);

			return result.OrderByDescending(p => p.Id);
		}

		protected override DeliveryReportQueue DoGet(int id)
		{
            throw new NotImplementedException();
		}

		protected override DeliveryReportQueue DoCreate(DeliveryReportQueueModel model, DeliveryReportQueue entity, out int id)
		{
            throw new NotImplementedException();
		}

		protected override void DoUpdate(DeliveryReportQueueModel model, int id, DeliveryReportQueue entity)
		{
            throw new NotImplementedException();
		}

		protected override void DoRemove(int id, DeliveryReportQueue entity)
		{
            throw new NotImplementedException();
		}

		protected override void DoRemove(List<int> ids, List<DeliveryReportQueue> entities)
		{
            throw new NotImplementedException();
		}

        protected override ReportDownloadModel ProduceFile(DeliveryReportQueueCriteriaModel criteria, List<DeliveryReportQueueModel> resultList)
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
                resultList: result.ToList());
        }
	}
}
