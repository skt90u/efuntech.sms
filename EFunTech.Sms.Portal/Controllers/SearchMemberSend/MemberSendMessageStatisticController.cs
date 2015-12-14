﻿using EFunTech.Sms.Portal.Models;
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
using EFunTech.Sms.Core;

namespace EFunTech.Sms.Portal.Controllers
{
	public class MemberSendMessageStatisticController : CrudApiController<MemberSendMessageStatisticCriteriaModel, SendMessageStatisticModel, SendMessageStatistic, int>
	{
		public MemberSendMessageStatisticController(IUnitOfWork unitOfWork, ILogService logService)
			: base(unitOfWork, logService)
		{
		}

		protected override IOrderedQueryable<SendMessageStatistic> DoGetList(MemberSendMessageStatisticCriteriaModel criteria)
		{
            // 目前使用者的資料
            IQueryable<SendMessageStatistic> result = this.repository.GetAll();

            var sendMessageHistoryRepository = this.unitOfWork.Repository<SendMessageHistory>();

            var predicate = PredicateBuilder.True<SendMessageStatistic>();
            predicate = predicate.And(p => p.CreatedUserId == CurrentUser.Id);
            predicate = predicate.And(p => p.SendTime >= criteria.StartDate);
            predicate = predicate.And(p => p.SendTime <= criteria.EndDate);

            if (!string.IsNullOrEmpty(criteria.Mobile) ||
                !string.IsNullOrEmpty(criteria.ReceiptStatus))
            {
                var innerPredicate = PredicateBuilder.True<SendMessageHistory>();
                innerPredicate = innerPredicate.And(p => p.CreatedUserId == CurrentUser.Id);
                innerPredicate = innerPredicate.And(p => p.SendTime >= criteria.StartDate);
                innerPredicate = innerPredicate.And(p => p.SendTime <= criteria.EndDate);

                if (!string.IsNullOrEmpty(criteria.Mobile))
                {
                    var _predicate = PredicateBuilder.False<SendMessageHistory>();

                    var arrMobile = criteria.Mobile.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);

                    if (arrMobile.Length != 0)
                    {
                        foreach (string mobile in arrMobile)
                        {
                            string mobileKeyword = mobile;

                            if (MobileUtil.IsPossibleNumber(mobileKeyword))
                                mobileKeyword = MobileUtil.GetE164PhoneNumber(mobileKeyword);

                            _predicate = _predicate.Or(p => p.DestinationAddress.Contains(mobileKeyword));
                        }

                        innerPredicate = innerPredicate.And(_predicate);
                    }
                }

                if (!string.IsNullOrEmpty(criteria.ReceiptStatus))
                {
                    var _predicate = PredicateBuilder.False<SendMessageHistory>();

                    var arrReceiptStatus = criteria.ReceiptStatus.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);

                    if (arrReceiptStatus.Length != 0)
                    {
                        foreach (string receiptStatus in arrReceiptStatus)
                        {
                            var status = (DeliveryReportStatus)Convert.ToInt32(receiptStatus);
                            _predicate = _predicate.Or(p => p.DeliveryStatus == status);
                        }

                        innerPredicate = innerPredicate.And(_predicate);
                    }
                }

                var _result = sendMessageHistoryRepository.GetAll()
                                                          .AsExpandable()
                                                          .Where(innerPredicate)
                                                          .Select(p => p.SendMessageQueueId)
                                                          .Distinct();

                var sendMessageQueueIds = _result.ToList();

                predicate = predicate.And(p => sendMessageQueueIds.Contains(p.SendMessageQueueId)); // 20151126 Norman, 避免StackOverFlow
            }

            result = result.AsExpandable().Where(predicate);

            return result.OrderByDescending(p => p.Id);
		}

		protected override SendMessageStatistic DoGet(int id)
		{
            return this.repository.GetById(id);
		}

		protected override SendMessageStatistic DoCreate(SendMessageStatisticModel model, SendMessageStatistic entity, out int id)
		{
            throw new NotImplementedException();
		}

		protected override void DoUpdate(SendMessageStatisticModel model, int id, SendMessageStatistic entity)
		{
            throw new NotImplementedException();
		}

		protected override void DoRemove(int id, SendMessageStatistic entity)
		{
            throw new NotImplementedException();
		}

		protected override void DoRemove(List<int> ids, List<SendMessageStatistic> entities)
		{
            throw new NotImplementedException();
		}

        //protected override IEnumerable<SendMessageStatisticModel> ConvertModel(IEnumerable<SendMessageStatisticModel> models)
        //{
        //    var deaprtmentRepository = this.unitOfWork.Repository<Department>();
        //    var userRepository = this.unitOfWork.Repository<ApplicationUser>();

        //    int rowNo = 0;

        //    foreach (var model in models)
        //    {
        //        model.RowNo = ++rowNo;
        //    }

        //    return models;
        //}

        protected override ReportDownloadModel ProduceFile(MemberSendMessageStatisticCriteriaModel criteria, List<SendMessageStatisticModel> resultList)
        {
            TimeSpan clientTimezoneOffset = ClientTimezoneOffset;
            string timeFormat = Converter.Every8d_SentTime;

            var result = resultList.Select(p => new
            {
                訊息類型 = AttributeHelper.GetColumnDescription(p.SendMessageType),
                發送時間 = Converter.ToLocalTimeString(p.SendTime, clientTimezoneOffset, timeFormat),
                簡訊類別描述 = p.SendTitle,
                發送內容 = p.SendBody,
                發送通數 = p.TotalReceiverCount,
                成功接收 = p.TotalSuccess,
                傳送中 = p.TotalSending,
                //逾期收訊 = p.TotalTimeout,
                傳送失敗 = p.TotalTimeout,
                發送扣點 = p.TotalMessageCost,
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
