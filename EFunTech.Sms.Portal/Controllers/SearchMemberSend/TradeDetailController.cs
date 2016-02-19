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
using System.ComponentModel;

using System.Data;
using EFunTech.Sms.Core;
using System.Data.Entity;
using System.Threading.Tasks;

namespace EFunTech.Sms.Portal.Controllers
{
	public class TradeDetailController : CrudApiController<TradeDetailCriteriaModel, TradeDetailModel, TradeDetail, int>
	{
        protected TradeService tradeService;

        public TradeDetailController(DbContext context, ILogService logService)
            : base(context, logService)
        {
            this.tradeService = new TradeService(new UnitOfWork(context), logService);
        }

        protected override IQueryable<TradeDetail> DoGetList(TradeDetailCriteriaModel criteria)
		{
			var predicate = PredicateBuilder.True<TradeDetail>();
            predicate = predicate.And(p => p.OwnerId == CurrentUserId);
            predicate = predicate.And(p => p.TradeTime >= criteria.StartDate);
            predicate = predicate.And(p => p.TradeTime <= criteria.EndDate);

            if (criteria.TradeType != TradeType.All) 
                predicate = predicate.And(p => p.TradeType == criteria.TradeType);

            var searchText = criteria.SearchText;
            if (!string.IsNullOrEmpty(searchText))
            {
                var innerPredicate = PredicateBuilder.False<TradeDetail>();

                innerPredicate = innerPredicate.Or(p => !string.IsNullOrEmpty(p.Remark) && p.Remark.Contains(searchText));

                predicate = predicate.And(innerPredicate);
            }

            var result = context.Set<TradeDetail>()
                            .AsExpandable()
                            .Where(predicate)
                            .OrderByDescending(p => p.Id);

			return result;
		}

        protected override async Task DoRemove(int id)
		{
            TradeDetail entity = await DoGet(id);

            this.tradeService.DismissAllot(entity);
		}

        //protected override IEnumerable<TradeDetailModel> ConvertModel(IEnumerable<TradeDetailModel> models)
        //{
        //    int rowNo = 0;
        //    foreach(var model in models)
        //    {
        //        model.RowNo = ++rowNo;
        //    }

        //    return models;
        //}

        protected override ReportDownloadModel ProduceFile(TradeDetailCriteriaModel criteria, IEnumerable<TradeDetailModel> models)
        {
            var clientTimezoneOffset = ClientTimezoneOffset;
            string timeFormat = Converter.Every8d_SentTime;
            
            var result = models.Select(p => new
            {
                // 交易時間	交易類別	交易點數	交易說明
                交易時間 = Converter.ToLocalTimeString(p.TradeTime, clientTimezoneOffset, timeFormat),
                交易類別 = p.TradeTypeString,
                交易點數 = p.Point,
                交易說明 = p.Remark,
            });
            return ProduceExcelFile(
                fileName: "點數購買與匯轉明細.xlsx", 
                sheetName: "點數購買與匯轉明細",
                models: result);
        }
	}
}
