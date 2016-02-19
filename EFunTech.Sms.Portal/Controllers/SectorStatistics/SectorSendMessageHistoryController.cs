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
using EFunTech.Sms.Portal.Models.Mapper;
using EFunTech.Sms.Core;
using System.Data.Entity;

namespace EFunTech.Sms.Portal.Controllers
{
	public class SectorSendMessageHistoryController : CrudApiController<SectorSendMessageHistoryCriteriaModel, SendMessageHistoryModel, SendMessageHistory, int>
	{
        public SectorSendMessageHistoryController(DbContext context, ILogService logService)
            : base(context, logService)
        {
        }

        /// <summary>
        /// 搜尋指定使用者在特定發送時間範圍的資料
        /// </summary>
		protected override IQueryable<SendMessageHistory> DoGetList(SectorSendMessageHistoryCriteriaModel criteria)
		{
            var predicate = PredicateBuilder.True<SendMessageHistory>();

            // 指定使用者
            predicate = predicate.And(p => p.CreatedUserId == criteria.UserId);

            //this.logService.Debug("SectorSendMessageHistory.DoGetList 查詢時間範圍 = {0} ~ {1}", Converter.DebugString(criteria.StartDate), Converter.DebugString(criteria.EndDate));

            predicate = predicate.And(p => p.SendTime >= criteria.StartDate);
            predicate = predicate.And(p => p.SendTime <= criteria.EndDate);

            var result = context.Set<SendMessageHistory>()
                .AsExpandable()
                .Where(predicate)
                .OrderByDescending(p => p.Id);

            return result;
		}

        protected override IEnumerable<SendMessageHistoryModel> ConvertModel(IEnumerable<SendMessageHistoryModel> models)
        {
            return SendMessageHistoryProfile.ConvertModel(models, new UnitOfWork(context));
        }

        protected override ReportDownloadModel ProduceFile(SectorSendMessageHistoryCriteriaModel criteria, IEnumerable<SendMessageHistoryModel> models)
        {
            TimeSpan clientTimezoneOffset = ClientTimezoneOffset;
            string timeFormat = Converter.Every8d_SentTime;

            var result = models.Select(p => new
            {
                簡訊類別 = AttributeHelper.GetColumnDescription(p.SendMessageType),
                部門 = p.DepartmentName,
                姓名 = p.FullName,
                帳號 = p.UserName,
                門號 = p.DestinationAddress,
                發送地區 = p.Region,
                發送時間 = Converter.ToLocalTimeString(p.SendTime, clientTimezoneOffset, timeFormat),
                收訊時間 = Converter.ToLocalTimeString(p.SentDate, clientTimezoneOffset, timeFormat),
                簡訊類別描述 = p.SendTitle,
                發送內容 = p.SendBody,
                狀態 = p.DeliveryStatusChineseString,
                發送扣點 = p.MessageCost,
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
