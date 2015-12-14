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
using System.Reflection;
using OfficeOpenXml;
using System.IO;
using AutoMapper;
using OfficeOpenXml.Style;
using System.Drawing;
using EFunTech.Sms.Core;

namespace EFunTech.Sms.Portal.Controllers
{
	public class MessageReceiverController : CrudApiController<MessageReceiverCriteriaModel, MessageReceiverModel, MessageReceiver, int>
	{
		public MessageReceiverController(IUnitOfWork unitOfWork, ILogService logService)
			: base(unitOfWork, logService)
		{
		}

		protected override IOrderedQueryable<MessageReceiver> DoGetList(MessageReceiverCriteriaModel criteria)
		{
            IQueryable<MessageReceiver> result = this.repository.GetAll();

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
			result = result.AsExpandable().Where(predicate);

			return result.OrderByDescending(p => p.Id);
		}

        protected override ReportDownloadModel ProduceFile(MessageReceiverCriteriaModel criteria, List<MessageReceiverModel> resultList)
        {
            var sendMessageRule = this.unitOfWork.Repository<SendMessageRule>().GetById(criteria.SendMessageRuleId);
            var sendMessageRuleModel = Mapper.Map<SendMessageRule, SendMessageRuleModel>(sendMessageRule);
            var sendTimeType = sendMessageRule.SendTimeType;

            switch (sendTimeType)
            {
                case SendTimeType.Deliver:
                    {
                        TimeSpan clientTimezoneOffset = ClientTimezoneOffset;
                        string timeFormat = Converter.Every8d_SentTime;

                        var result = resultList.Select(p => new
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
                            resultList: result.ToList());
                    }
                case SendTimeType.Cycle:
                    {
                        var result = resultList.Select(p => new
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
                            resultList: result.ToList());
                    }
                default:
                    throw new Exception(string.Format("Unknown SendTimeType({0}", sendTimeType));
            }
        }

		protected override MessageReceiver DoGet(int id)
		{
            return this.repository.GetById(id);
		}

		protected override MessageReceiver DoCreate(MessageReceiverModel model, MessageReceiver entity, out int id)
		{
            throw new NotImplementedException();
		}

		protected override void DoUpdate(MessageReceiverModel model, int id, MessageReceiver entity)
		{
            throw new NotImplementedException();
		}

        /// <summary>
        /// 刪除指定收訊者
        /// </summary>
		protected override void DoRemove(int id, MessageReceiver entity)
		{
            // (1) 只能刪除自己所建立的簡訊規則
            // (2) 檢查目前簡訊狀態為 Ready 才可以執行刪除動作
            // (3) 設定目前簡訊狀態為刪除中
            // (4) 回補目前使用者點數
            // (5) 刪除 簡訊收訊人 (MessageReceiver)
            // (6) 設定目前簡訊狀態為 Ready 
            // (7) 判斷是否全部收訊者都刪除，如果是就刪除簡訊規則 ?

            var sendMessageRuleRepository = this.unitOfWork.Repository<SendMessageRule>();

            var sendMessageRule = sendMessageRuleRepository.GetById(entity.SendMessageRuleId);
            if (sendMessageRule == null)
                throw new Exception("找不到對應簡訊規則");

            ////////////////////////////////////////
            // (1) 只能刪除自己所建立的簡訊規則
            ////////////////////////////////////////

            if (!CurrentUser.SendMessageRules.Any(p => p.Id == sendMessageRule.Id))
                return;

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
            sendMessageRuleRepository.Update(sendMessageRule);

            ////////////////////////////////////////
            // (4) 回補目前使用者點數 + 更新目前簡訊規則總花費點數
            ////////////////////////////////////////

            this.tradeService.DeleteMessageReceiver(sendMessageRule, entity);

            ////////////////////////////////////////
            // (5) 刪除 簡訊收訊人 (MessageReceiver)
            ////////////////////////////////////////

            this.repository.Delete(entity);

            ////////////////////////////////////////
            // (6) 設定目前簡訊狀態為 Ready 
            ////////////////////////////////////////

            sendMessageRule.SendMessageRuleStatus = SendMessageRuleStatus.Ready;
            sendMessageRuleRepository.Update(sendMessageRule);

            ////////////////////////////////////////
            // (7) 判斷是否全部收訊者都刪除，如果是就刪除簡訊規則 ?
            ////////////////////////////////////////
            //if (!this.repository.Any(p => p.SendMessageRuleId == sendMessageRule.Id))
            //{

            //}
		}

		protected override void DoRemove(List<int> ids, List<MessageReceiver> entities)
		{
            for (var i = 0; i < entities.Count; i++)
            {
                DoRemove(ids[i], entities[i]);
            }
		}

	}
}
