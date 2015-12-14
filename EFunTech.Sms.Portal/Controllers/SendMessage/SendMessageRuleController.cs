using EFunTech.Sms.Portal.Models;
using EFunTech.Sms.Schema;
using System.Linq;
using EFunTech.Sms.Portal.Controllers.Common;
using EFunTech.Sms.Portal.Models.Common;
using JUtilSharp.Database;

using System.Collections.Generic;
using LinqKit;
using System;
using AutoMapper;

using System.ComponentModel;
using EFunTech.Sms.Portal.Models.Criteria;
using Microsoft.Practices.ServiceLocation;
using Hangfire;
using EFunTech.Sms.Core;
using Newtonsoft.Json;

namespace EFunTech.Sms.Portal.Controllers
{
	public class SendMessageRuleController : CrudApiController<SendMessageRuleCriteriaModel, SendMessageRuleModel, SendMessageRule, int>
	{
        private SendMessageRuleService sendMessageRuleService;
        
        public SendMessageRuleController(IUnitOfWork unitOfWork, ILogService logService)
			: base(unitOfWork, logService)
		{
            this.sendMessageRuleService = new SendMessageRuleService(unitOfWork, logService);
		}

		protected override IOrderedQueryable<SendMessageRule> DoGetList(SendMessageRuleCriteriaModel criteria)
		{
            IQueryable<SendMessageRule> result = CurrentUser.SendMessageRules.AsQueryable();

            var predicate = PredicateBuilder.True<SendMessageRule>();
            predicate = predicate.And(p => p.SendTimeType == criteria.SendTimeType);
            predicate = predicate.And(p => p.SendMessageRuleStatus == SendMessageRuleStatus.Ready);

            var searchText = criteria.SearchText;
            if (!string.IsNullOrEmpty(searchText))
            {
                var innerPredicate = PredicateBuilder.False<SendMessageRule>();

                innerPredicate = innerPredicate.Or(p => !string.IsNullOrEmpty(p.SendTitle) && p.SendTitle.Contains(searchText));
                innerPredicate = innerPredicate.Or(p => !string.IsNullOrEmpty(p.SendBody) && p.SendBody.Contains(searchText));

                predicate = predicate.And(innerPredicate);
            }
            result = result.AsExpandable().Where(predicate);

            return result.OrderByDescending(p => p.Id);
		}

		protected override SendMessageRule DoGet(int id)
		{
            return CurrentUser.SendMessageRules.Where(p => p.Id == id).FirstOrDefault();
		}

        #region DoCreate


        protected override SendMessageRule DoCreate(SendMessageRuleModel model, SendMessageRule entity, out int id)
		{
            model.UpdateClientTimezoneOffset(ClientTimezoneOffset);

            var rules = this.sendMessageRuleService.CreateSendMessageRuleFromWeb(CurrentUser, model);

            entity = rules.FirstOrDefault();
            id = entity.Id;
            return entity;
		}

        #endregion

        #region DoUpdate
        
        protected override void DoUpdate(SendMessageRuleModel model, int id, SendMessageRule entity)
		{
            model.UpdateClientTimezoneOffset(ClientTimezoneOffset);

            this.sendMessageRuleService.UpdateSendMessageRule(CurrentUser, model);
		}

        private void UpdateMessageReceivers(SendMessageRuleModel model)
        {
            DateTime utcNow = DateTime.UtcNow;

            var _repository = this.unitOfWork.Repository<MessageReceiver>();

            var _entities = _repository.GetMany(p => p.SendMessageRuleId == model.Id).ToList();

            foreach (var _entity in _entities)
            {
                MessageCostInfo messageCostInfo = new MessageCostInfo(model.SendBody, _entity.Mobile);

                _entity.SendTitle = model.SendTitle;
                _entity.UpdatedTime = utcNow;

                _entity.SendBody = messageCostInfo.SendBody;
                _entity.MessageLength = messageCostInfo.MessageLength;
                _entity.MessageNum = messageCostInfo.MessageNum;
                _entity.MessageCost = messageCostInfo.MessageCost;
                _entity.MessageFormatError = messageCostInfo.MessageFormatError;

                _repository.Update(_entity);
            }
        }

        #endregion

        #region DoRemove

        protected override void DoRemove(int id, SendMessageRule entity)
		{
            this.sendMessageRuleService.RemoveSendMessageRule(CurrentUser, id);
        }

        #endregion
        
        protected override void DoRemove(List<int> ids, List<SendMessageRule> entities)
		{
            for (int i = 0; i < ids.Count; i++)
            {
                DoRemove(ids[i], entities[i]);
            }
		}

        //protected override IEnumerable<SendMessageRuleModel> ConvertModel(IEnumerable<SendMessageRuleModel> models)
        //{
        //    DateTime utcNow = DateTime.UtcNow;

        //    foreach (var model in models)
        //    {
        //        if (model.SendTimeType == SendTimeType.Deliver)
        //        {
        //            DateTime dt = new DateTime(
        //                model.SendDeliver.Date.Year, 
        //                model.SendDeliver.Date.Month, 
        //                model.SendDeliver.Date.Day, 
        //                model.SendDeliver.Hour, 
        //                model.SendDeliver.Minute, 
        //                00);
        //            model.SendTimeString = dt.ToString("yyyy/MM/dd HH:mm:ss");
        //        }

        //        if (model.SendTimeType == SendTimeType.Cycle)
        //        {
        //            if (model.SendCycleEveryDay != null)
        //            {
        //                DateTime cycleDate = new DateTime(utcNow.Year, utcNow.Month, utcNow.Day,
        //                    model.SendCycleEveryDay.Hour,
        //                    model.SendCycleEveryDay.Minute,
        //                    0);

        //                model.StartDateString = model.SendCycleEveryDay.StartDate.ToString("yyyy/MM/dd HH:mm:ss");
        //                model.EndDateString = model.SendCycleEveryDay.EndDate.ToString("yyyy/MM/dd HH:mm:ss");
        //                model.CycleString = string.Format("每天的{0}",
        //                    cycleDate.ToString("HH:mm"));
        //            }
        //            else if (model.SendCycleEveryWeek != null)
        //            {
        //                DateTime cycleDate = new DateTime(utcNow.Year, utcNow.Month, utcNow.Day,
        //                    model.SendCycleEveryWeek.Hour,
        //                    model.SendCycleEveryWeek.Minute,
        //                    0);

        //                model.StartDateString = model.SendCycleEveryWeek.StartDate.ToString("yyyy/MM/dd HH:mm:ss");
        //                model.EndDateString = model.SendCycleEveryWeek.EndDate.ToString("yyyy/MM/dd HH:mm:ss");
        //                model.CycleString = string.Format("{0}的{1}",
        //                    // 取得 DayOfWeek 的中文名稱(C#)
        //                    // http://zip.nvp.com.tw/forum.php?mod=viewthread&tid=1150
        //                    string.Join("、", model.SendCycleEveryWeek.GetDayOfWeeks().Select(p => System.Globalization.DateTimeFormatInfo.CurrentInfo.DayNames[(int)p])),
        //                    cycleDate.ToString("HH:mm"));
        //            }
        //            else if (model.SendCycleEveryMonth != null)
        //            {
        //                DateTime cycleDate = new DateTime(utcNow.Year, utcNow.Month,
        //                    model.SendCycleEveryMonth.Day,
        //                    model.SendCycleEveryMonth.Hour,
        //                    model.SendCycleEveryMonth.Minute,
        //                    0);

        //                model.StartDateString = model.SendCycleEveryMonth.StartDate.ToString("yyyy/MM/dd HH:mm:ss");
        //                model.EndDateString = model.SendCycleEveryMonth.EndDate.ToString("yyyy/MM/dd HH:mm:ss");
        //                model.CycleString = string.Format("每月的{0}號{1}",
        //                    cycleDate.ToString("dd"),
        //                    cycleDate.ToString("HH:mm"));
        //            }
        //            else if (model.SendCycleEveryYear != null)
        //            {
        //                DateTime cycleDate = new DateTime(utcNow.Year,
        //                    model.SendCycleEveryYear.Month,
        //                    model.SendCycleEveryYear.Day,
        //                    model.SendCycleEveryYear.Hour,
        //                    model.SendCycleEveryYear.Minute,
        //                    0);

        //                model.StartDateString = model.SendCycleEveryYear.StartDate.ToString("yyyy/MM/dd HH:mm:ss");
        //                model.EndDateString = model.SendCycleEveryYear.EndDate.ToString("yyyy/MM/dd HH:mm:ss");
        //                model.CycleString = string.Format("每年的{0}月{1}號{2}",
        //                    cycleDate.ToString("MM"),
        //                    cycleDate.ToString("dd"),
        //                    cycleDate.ToString("HH:mm"));
        //            }
        //            else
        //            {
        //                model.StartDateString = string.Empty;
        //                model.EndDateString = string.Empty;
        //                model.CycleString = string.Empty;
        //            }
        //        }
        //    }

        //    return models;
        //}

    }
}
