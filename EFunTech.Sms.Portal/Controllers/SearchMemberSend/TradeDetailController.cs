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

namespace EFunTech.Sms.Portal.Controllers
{
	public class TradeDetailController : CrudApiController<TradeDetailCriteriaModel, TradeDetailModel, TradeDetail, int>
	{
        public TradeDetailController(IUnitOfWork unitOfWork, ILogService logService)
			: base(unitOfWork, logService)
		{
		}

		protected override IOrderedQueryable<TradeDetail> DoGetList(TradeDetailCriteriaModel criteria)
		{
            IQueryable<TradeDetail> result = this.unitOfWork.Repository<TradeDetail>().GetAll();

			var predicate = PredicateBuilder.True<TradeDetail>();
            predicate = predicate.And(p => p.OwnerId == CurrentUser.Id);
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

            result = result.AsExpandable().Where(predicate);

			return result.OrderByDescending(p => p.Id);
		}

		protected override TradeDetail DoGet(int id)
		{
            return this.unitOfWork.Repository<TradeDetail>().GetById(id);
		}

		protected override TradeDetail DoCreate(TradeDetailModel model, TradeDetail entity, out int id)
		{
            throw new NotImplementedException();
		}

		protected override void DoUpdate(TradeDetailModel model, int id, TradeDetail entity)
		{
            throw new NotImplementedException();
		}

		protected override void DoRemove(int id, TradeDetail entity)
		{
            this.tradeService.DismissAllot(entity);
		}

		protected override void DoRemove(List<int> ids, List<TradeDetail> entities)
		{
            throw new NotImplementedException();
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

        protected override ReportDownloadModel ProduceFile(TradeDetailCriteriaModel criteria, List<TradeDetailModel> resultList)
        {
            var clientTimezoneOffset = ClientTimezoneOffset;
            string timeFormat = Converter.Every8d_SentTime;
            
            var result = resultList.Select(p => new
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
                resultList: result.ToList());
        }
	}
}
