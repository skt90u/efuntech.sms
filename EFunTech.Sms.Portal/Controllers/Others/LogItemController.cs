﻿using EFunTech.Sms.Portal.Models;
using EFunTech.Sms.Schema;
using System.Linq;
using EFunTech.Sms.Portal.Controllers.Common;
using EFunTech.Sms.Portal.Models.Common;
using JUtilSharp.Database;

using System.Collections.Generic;
using LinqKit;
using System;
using EFunTech.Sms.Core;
using System.ComponentModel;
using System.Data;
using EFunTech.Sms.Portal.Models.Criteria;

namespace EFunTech.Sms.Portal.Controllers
{
	public class LogItemController : CrudApiController<LogItemCriteriaModel, LogItemModel, LogItem, int>
	{
		public LogItemController(IUnitOfWork unitOfWork, ILogService logService)
			: base(unitOfWork, logService)
		{
		}

		protected override IOrderedQueryable<LogItem> DoGetList(LogItemCriteriaModel criteria)
		{
            IQueryable<LogItem> result = this.repository.GetAll().AsQueryable();

			var predicate = PredicateBuilder.True<LogItem>();

            predicate = predicate.And(p => p.CreatedTime >= criteria.StartDate);
            predicate = predicate.And(p => p.CreatedTime <= criteria.EndDate);

            if(criteria.LogLevel != LogLevel.All)
            {
                var innerPredicate = PredicateBuilder.False<LogItem>();

                //foreach (LogLevel logLevel in Enum.GetValues(typeof(LogLevel)))
                //{
                //    if (criteria.LogLevel.HasFlag(logLevel))
                //    {
                //        innerPredicate = innerPredicate.Or(p => p.LogLevel == logLevel);
                //    }
                //}

                // 20151101 Norman, 
                // 將 LogLevel 改成使用 Flag 之後，Hangfire 仍然使用舊的方式(Debug = 0, Info = 1, Warn = 2, Error = 3)
                // 找不出發現原因，目前沒有解決辦法，只好改回來
                innerPredicate = innerPredicate.Or(p => p.LogLevel == criteria.LogLevel);

                predicate = predicate.And(innerPredicate);
            }
            
			var searchText = criteria.SearchText;
			if (!string.IsNullOrEmpty(searchText))
			{
				var innerPredicate = PredicateBuilder.False<LogItem>();

				innerPredicate = innerPredicate.Or(p => !string.IsNullOrEmpty(p.EntryAssembly) && p.EntryAssembly.Contains(searchText));
				innerPredicate = innerPredicate.Or(p => !string.IsNullOrEmpty(p.Class) && p.Class.Contains(searchText));
				innerPredicate = innerPredicate.Or(p => !string.IsNullOrEmpty(p.Method) && p.Method.Contains(searchText));
				innerPredicate = innerPredicate.Or(p => !string.IsNullOrEmpty(p.Message) && p.Message.Contains(searchText));
				innerPredicate = innerPredicate.Or(p => !string.IsNullOrEmpty(p.StackTrace) && p.StackTrace.Contains(searchText));
				innerPredicate = innerPredicate.Or(p => !string.IsNullOrEmpty(p.UserName) && p.UserName.Contains(searchText));
				innerPredicate = innerPredicate.Or(p => !string.IsNullOrEmpty(p.Host) && p.Host.Contains(searchText));

				predicate = predicate.And(innerPredicate);
			}
			result = result.AsExpandable().Where(predicate);

			return result.OrderByDescending(p => p.Id);
		}

		protected override LogItem DoGet(int id)
		{
            throw new NotImplementedException();
		}

		protected override LogItem DoCreate(LogItemModel model, LogItem entity, out int id)
		{
            throw new NotImplementedException();
		}

		protected override void DoUpdate(LogItemModel model, int id, LogItem entity)
		{
            throw new NotImplementedException();
		}

		protected override void DoRemove(int id, LogItem entity)
		{
            this.repository.Delete(p => p.Id == id);
		}

		protected override void DoRemove(List<int> ids, List<LogItem> entities)
		{
            for (var i = 0; i < ids.Count; i++)
                DoRemove(ids[i], entities[i]);
		}

        private string GetString(string str, int maxLen = 1000)
        {
            return (str != null) 
                ? str.Substring(0, Math.Min(maxLen, str.Length)) 
                : null;
        }

        protected override ReportDownloadModel ProduceFile(LogItemCriteriaModel criteria, List<LogItemModel> resultList)
        {
            TimeSpan clientTimezoneOffset = ClientTimezoneOffset;
            string timeFormat = Converter.Every8d_SentTime;

            var result = resultList.Select(p => new
            {
                建立時間 = Converter.ToLocalTimeString(p.CreatedTime, clientTimezoneOffset, timeFormat),
                Host = p.Host,
                建立者 = p.UserName,
                函式 = p.Class + "." + p.Method,
                層級 = AttributeHelper.GetColumnDescription(p.LogLevel),
                訊息內容 = GetString(p.Message),
                呼叫堆疊 = GetString(p.StackTrace),
            });

            return ProduceExcelFile(
                fileName: "偵錯紀錄.xlsx",
                sheetName: "偵錯紀錄",
                resultList: result.ToList());
        }
        
	}
}
