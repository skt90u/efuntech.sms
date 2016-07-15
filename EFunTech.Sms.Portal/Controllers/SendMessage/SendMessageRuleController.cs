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
        private ISystemParameters systemParameters;

        public SendMessageRuleController(DbContext context, ILogService logService, ISystemParameters systemParameters)
            : base(context, logService)
        {
            this.sendMessageRuleService = new SendMessageRuleService(new UnitOfWork(context), logService);
            this.systemParameters = systemParameters;
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
            try
            {
                // systemParameters.AllowSendMessage: 避免測試的時候，誤發大量簡訊；請在正式上線的時候才打開
                // 測試設定簡訊的時候，請將以下這一行註解掉，測試完畢，請恢復
                //if (!systemParameters.AllowSendMessage)throw new Exception("測試環境不允許發送簡訊。");

                model.UpdateClientTimezoneOffset(ClientTimezoneOffset);

                var rules = this.sendMessageRuleService.CreateSendMessageRuleFromWeb(CurrentUser, model);

                entity = rules.FirstOrDefault();

                return Task.FromResult(entity);
            }
            finally
            {
                GC.Collect();
                //GC.WaitForPendingFinalizers(); // WaitForPendingFinalizers doesn't necessarily give "better performance": it simply blocks until all objects in the finalisation queue have been finalised
            }
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
                var messageCostInfo = new MessageCostInfo(model.SendBody, _entity.Mobile);

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
