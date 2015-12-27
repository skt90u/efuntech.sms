using EFunTech.Sms.Portal.Models;
using EFunTech.Sms.Schema;
using System.Linq;
using EFunTech.Sms.Portal.Controllers.Common;
using JUtilSharp.Database;
using System.Collections.Generic;
using LinqKit;
using System;

using EFunTech.Sms.Portal.Models.Criteria;
using System.Data.Entity;
using System.Threading.Tasks;
using EFunTech.Sms.Portal.Models.Mapper;

namespace EFunTech.Sms.Portal.Controllers
{
	public class SendMessageRuleController : CrudApiController<SendMessageRuleCriteriaModel, SendMessageRuleModel, SendMessageRule, int>
	{
        private SendMessageRuleService sendMessageRuleService;

        public SendMessageRuleController(DbContext context, ILogService logService)
            : base(context, logService)
        {
            this.sendMessageRuleService = new SendMessageRuleService(new UnitOfWork(context), logService);
        }

		protected override IQueryable<SendMessageRule> DoGetList(SendMessageRuleCriteriaModel criteria)
		{
            var predicate = PredicateBuilder.True<SendMessageRule>();

            predicate = predicate.And(p => p.CreatedUserId == CurrentUserId);
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

            var result = context.Set<SendMessageRule>()
                             .AsExpandable()
                             .Where(predicate)
                             .OrderByDescending(p => p.Id);

            return result;
		}

        #region DoCreate


        protected override Task<SendMessageRule> DoCreate(SendMessageRuleModel model, SendMessageRule entity)
		{
            model.UpdateClientTimezoneOffset(ClientTimezoneOffset);

            var rules = this.sendMessageRuleService.CreateSendMessageRuleFromWeb(CurrentUser, model);

            entity = rules.FirstOrDefault();
            
            return Task.FromResult(entity);
		}

        #endregion

        #region DoUpdate
        
        protected override async Task DoUpdate(SendMessageRuleModel model, int id, SendMessageRule entity)
		{
            await Task.Run(() => {
                model.UpdateClientTimezoneOffset(ClientTimezoneOffset);

                this.sendMessageRuleService.UpdateSendMessageRule(CurrentUser, model);
            });
		}

        private async Task UpdateMessageReceivers(SendMessageRuleModel model)
        {
            DateTime utcNow = DateTime.UtcNow;

            var _entities = context.Set<MessageReceiver>().Where(p => p.SendMessageRuleId == model.Id).ToList();

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

                await context.UpdateAsync(_entity);
            }
        }

        #endregion

        #region DoRemove

        protected override async Task DoRemove(int id)
		{
            await Task.Run(() => {
                this.sendMessageRuleService.RemoveSendMessageRule(CurrentUser, id);
            });
        }

        #endregion

        protected override async Task DoRemove(int[] ids)
		{
            for (int i = 0; i < ids.Length; i++)
            {
                await DoRemove(ids[i]);
            }
		}

        protected override IEnumerable<SendMessageRuleModel> ConvertModel(IEnumerable<SendMessageRuleModel> models)
        {
            foreach(var model in models)
            {
                SendMessageRuleProfile.ConvertModel(model);
            }

            return models;
        }

    }
}
