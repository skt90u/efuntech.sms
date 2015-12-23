using EFunTech.Sms.Portal.Models;
using EFunTech.Sms.Schema;
using System.Linq;
using EFunTech.Sms.Portal.Controllers.Common;
using JUtilSharp.Database;
using System.Collections.Generic;
using LinqKit;
using System;

using EFunTech.Sms.Portal.Models.Criteria;

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

            var result = this.repository.DbSet
                             .AsExpandable()
                             .Where(predicate)
                             .OrderByDescending(p => p.Id);

            return result;
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

        protected override void DoRemove(int id)
		{
            this.sendMessageRuleService.RemoveSendMessageRule(CurrentUser, id);
        }

        #endregion

        protected override void DoRemove(int[] ids)
		{
            for (int i = 0; i < ids.Length; i++)
            {
                DoRemove(ids[i]);
            }
		}

        

    }
}
