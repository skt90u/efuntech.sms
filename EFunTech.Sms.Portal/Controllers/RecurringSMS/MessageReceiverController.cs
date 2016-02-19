using EFunTech.Sms.Portal.Models;
using EFunTech.Sms.Schema;
using System.Linq;
using EFunTech.Sms.Portal.Controllers.Common;
using EFunTech.Sms.Portal.Models.Common;

using System.Collections.Generic;
using LinqKit;
using System;
using EFunTech.Sms.Portal.Models.Criteria;

using System.ComponentModel;
using System.Data;
using AutoMapper;
using EFunTech.Sms.Core;
using System.Data.Entity;
using JUtilSharp.Database;
using System.Threading.Tasks;
using EFunTech.Sms.Portal.Models.Mapper;

namespace EFunTech.Sms.Portal.Controllers
{
    public class MessageReceiverController : CrudApiController<MessageReceiverCriteriaModel, MessageReceiverModel, MessageReceiver, int>
	{
        protected TradeService tradeService;

        public MessageReceiverController(DbContext context, ILogService logService)
            : base(context, logService)
        {
            this.tradeService = new TradeService(new UnitOfWork(context), logService);
        }

        protected override IQueryable<MessageReceiver> DoGetList(MessageReceiverCriteriaModel criteria)
		{
			var predicate = PredicateBuilder.True<MessageReceiver>();
            predicate = predicate.And(p => p.SendMessageRuleId == criteria.SendMessageRuleId);

			var searchText = criteria.SearchText;
			if (!string.IsNullOrEmpty(searchText))
			{
				var innerPredicate = PredicateBuilder.False<MessageReceiver>();

				innerPredicate = innerPredicate.Or(p => !string.IsNullOrEmpty(p.Name) && p.Name.Contains(searchText));
				innerPredicate = innerPredicate.Or(p => !string.IsNullOrEmpty(p.Mobile) && p.Mobile.Contains(searchText));
				innerPredicate = innerPredicate.Or(p => !string.IsNullOrEmpty(p.Email) && p.Email.Contains(searchText));
				innerPredicate = innerPredicate.Or(p => !string.IsNullOrEmpty(p.SendTitle) && p.SendTitle.Contains(searchText));
				innerPredicate = innerPredicate.Or(p => !string.IsNullOrEmpty(p.SendBody) && p.SendBody.Contains(searchText));
				innerPredicate = innerPredicate.Or(p => !string.IsNullOrEmpty(p.MessageFormatError) && p.MessageFormatError.Contains(searchText));

				predicate = predicate.And(innerPredicate);
			}

            var result = context.Set<MessageReceiver>()
                             .AsExpandable()
                             .Where(predicate)
                             .OrderByDescending(p => p.Id);

			return result;
		}

        protected override ReportDownloadModel ProduceFile(MessageReceiverCriteriaModel criteria, IEnumerable<MessageReceiverModel> models)
        {
            var sendMessageRule = context.Set<SendMessageRule>().Find(criteria.SendMessageRuleId);
            var sendMessageRuleModel = Mapper.Map<SendMessageRule, SendMessageRuleModel>(sendMessageRule);
            sendMessageRuleModel = SendMessageRuleProfile.ConvertModel(sendMessageRuleModel);

            var sendTimeType = sendMessageRule.SendTimeType;

            switch (sendTimeType)
            {
                case SendTimeType.Deliver:
                    {
                        TimeSpan clientTimezoneOffset = ClientTimezoneOffset;
                        string timeFormat = Converter.Every8d_SentTime;

                        var result = models.Select(p => new
                        {
                            訊息類型 = p.SendMessageType == SendMessageType.SmsMessage ? "SMS" : "APP",
                            收訊者姓名 = p.Name,
                            收訊者門號 = p.Mobile,
                            預約時間 = Converter.ToLocalTimeString(sendMessageRuleModel.SendTime.Value, clientTimezoneOffset, timeFormat),
                            發送扣點 = p.MessageCost,
                        });
                        return ProduceExcelFile(
                            fileName: "預約簡訊收訊人名單.xlsx",
                            sheetName: "預約簡訊收訊人名單",
                            models: result);
                    }
                case SendTimeType.Cycle:
                    {
                        var result = models.Select(p => new
                        {
                            訊息類型 = p.SendMessageType == SendMessageType.SmsMessage ? "SMS" : "APP",
                            收訊者姓名 = p.Name,
                            收訊者門號 = p.Mobile,
                            收訊者信箱 = p.Email,
                            //預約時間 = sendMessageRuleModel.SendTimeString,
                            發送扣點 = p.MessageCost,
                        });
                        return ProduceExcelFile(
                            fileName: "週期簡訊收訊人名單.xlsx",
                            sheetName: "週期簡訊收訊人名單",
                            models: result);
                    }
                default:
                    throw new Exception(string.Format("Unknown SendTimeType({0}", sendTimeType));
            }
        }

        /// <summary>
        /// 刪除指定收訊者
        /// </summary>
        protected override async Task DoRemove(int id) 
		{
            MessageReceiver entity = await DoGet(id);

            // (1) 只能刪除自己所建立的簡訊規則
            // (2) 檢查目前簡訊狀態為 Ready 才可以執行刪除動作
            // (3) 設定目前簡訊狀態為刪除中
            // (4) 回補目前使用者點數
            // (5) 刪除 簡訊收訊人 (MessageReceiver)
            // (6) 設定目前簡訊狀態為 Ready 
            // (7) 判斷是否全部收訊者都刪除，如果是就刪除簡訊規則 ?

            var sendMessageRule = await context.Set<SendMessageRule>().FindAsync(entity.SendMessageRuleId);
            if (sendMessageRule == null)
                throw new Exception("找不到對應簡訊規則");

            ////////////////////////////////////////
            // (1) 只能刪除自己所建立的簡訊規則
            ////////////////////////////////////////

            if (!context.Set<SendMessageRule>().Any(p => p.Id == sendMessageRule.Id && p.CreatedUserId == CurrentUserId))
                return ;

            ////////////////////////////////////////
            // (2) 檢查目前簡訊狀態為 Ready 才可以執行刪除動作
            ////////////////////////////////////////

            if (sendMessageRule.SendMessageRuleStatus != SendMessageRuleStatus.Ready)
            {
                var description = AttributeHelper.GetColumnDescription(sendMessageRule.SendMessageRuleStatus);
                throw new Exception(description);
            }
            
            ////////////////////////////////////////
            // (3) 設定目前簡訊狀態為刪除中
            ////////////////////////////////////////

            sendMessageRule.SendMessageRuleStatus = SendMessageRuleStatus.Deleting;
            await context.UpdateAsync(sendMessageRule);

            ////////////////////////////////////////
            // (4) 回補目前使用者點數 + 更新目前簡訊規則總花費點數
            ////////////////////////////////////////

            this.tradeService.DeleteMessageReceiver(sendMessageRule, entity);

            ////////////////////////////////////////
            // (5) 刪除 簡訊收訊人 (MessageReceiver)
            ////////////////////////////////////////

            await context.DeleteAsync(entity);

            ////////////////////////////////////////
            // (6) 設定目前簡訊狀態為 Ready 
            ////////////////////////////////////////

            sendMessageRule.SendMessageRuleStatus = SendMessageRuleStatus.Ready;
            await context.UpdateAsync(sendMessageRule);

            ////////////////////////////////////////
            // (7) 判斷是否全部收訊者都刪除，如果是就刪除簡訊規則 ?
            ////////////////////////////////////////
            //if (!this.repository.Any(p => p.SendMessageRuleId == sendMessageRule.Id))
            //{

            //}
        }

        protected override async Task DoRemove(int[] ids) 
		{
            for (var i = 0; i < ids.Length; i++)
            {
                await DoRemove(ids[i]);
            }
        }

	}
}
