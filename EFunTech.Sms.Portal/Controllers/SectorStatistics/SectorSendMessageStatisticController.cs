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
using System.Linq.Expressions;
using AutoMapper;
using System.ComponentModel;
using EFunTech.Sms.Core;
using EFunTech.Sms.Portal.Models.Mapper;
using System.Data.Entity;

namespace EFunTech.Sms.Portal.Controllers
{
	public class SectorSendMessageStatisticController : CrudApiController<SectorSendMessageStatisticCriteriaModel, SendMessageStatisticModel, SendMessageStatistic, int>
	{
        protected ApiControllerHelper apiControllerHelper;

        public SectorSendMessageStatisticController(DbContext context, ILogService logService)
            : base(context, logService)
        {
            this.apiControllerHelper = new ApiControllerHelper(context, logService);
        }

        private IOrderedQueryable<SendMessageHistory> GetSendMessageHistory(SectorSendMessageStatisticCriteriaModel criteria)
        {
            var predicate = PredicateBuilder.True<SendMessageHistory>();

            //this.logService.Debug("SectorSendMessageStatistic.GetSendMessageHistory 查詢時間範圍 = {0} ~ {1}", Converter.DebugString(criteria.StartDate), Converter.DebugString(criteria.EndDate));

            predicate = predicate.And(p => p.SendTime >= criteria.StartDate);
            predicate = predicate.And(p => p.SendTime <= criteria.EndDate);

            switch (criteria.SearchType)
            {
                case SearchType.Department:
                    {
                        if (!string.IsNullOrEmpty(criteria.DepartmentIds))
                        {
                            var departmentIds = criteria.DepartmentIds.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).Select(p => Convert.ToInt32(p)).ToList();

                            predicate = predicate.And(p => p.DepartmentId.HasValue && departmentIds.Contains(p.DepartmentId.Value));
                        }
                        else
                        {
                            if (CurrentUser.Department != null)
                            {
                                predicate = predicate.And(p => p.DepartmentId.HasValue && CurrentUser.Department.Id == p.DepartmentId.Value);
                            }
                            else
                            {
                                // 取得目前帳號以及所有
                                var users = this.apiControllerHelper.GetDescendingUsersAndUser(CurrentUserId);

                                var userIds = users.Select(p => p.Id);

                                predicate = predicate.And(p => userIds.Contains(p.CreatedUserId));
                            }
                        }
                    } break;

                case SearchType.Member:
                    {
                        var userIds = criteria.UserIds.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);

                        predicate = predicate.And(p => userIds.Contains(p.CreatedUserId));

                    } break;
            }

            var result  = context.Set<SendMessageHistory>()
                            .AsExpandable()
                            .Where(predicate)
                            .OrderByDescending(p => p.Id);

            return result;
        }

		protected override IQueryable<SendMessageStatistic> DoGetList(SectorSendMessageStatisticCriteriaModel criteria)
		{
            DateTime utcNow = DateTime.UtcNow;

            var predicate = PredicateBuilder.True<SendMessageStatistic>();

            //this.logService.Debug("SectorSendMessageStatistic.DoGetList 查詢時間範圍 = {0} ~ {1}", Converter.DebugString(criteria.StartDate), Converter.DebugString(criteria.EndDate));

            predicate = predicate.And(p => p.SendTime >= criteria.StartDate);
            predicate = predicate.And(p => p.SendTime <= criteria.EndDate);

            switch (criteria.SearchType)
            {
                case SearchType.Department:
                    {
                        if(!string.IsNullOrEmpty(criteria.DepartmentIds))
                        {
                            var departmentIds = criteria.DepartmentIds.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).Select(p => Convert.ToInt32(p)).ToList();    

                            predicate = predicate.And(p => p.DepartmentId.HasValue && departmentIds.Contains(p.DepartmentId.Value));
                        }
                        else
                        {
                            if (CurrentUser.Department != null)
                            {
                                predicate = predicate.And(p => p.DepartmentId.HasValue && CurrentUser.Department.Id == p.DepartmentId.Value);
                            }
                            else
                            {
                                // 取得目前帳號以及所有
                                var users = this.apiControllerHelper.GetDescendingUsersAndUser(CurrentUserId);

                                var userIds = users.Select(p => p.Id);

                                predicate = predicate.And(p => userIds.Contains(p.CreatedUserId));
                            }
                        }
                    }break;

                case SearchType.Member:
                    {
                        if (!string.IsNullOrEmpty(criteria.UserIds))
                        {
                            var userIds = criteria.UserIds.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);

                            predicate = predicate.And(p => userIds.Contains(p.CreatedUserId));
                        }

                    } break;
            }

            var result = context.Set<SendMessageStatistic>()
                            .AsExpandable()
                            .Where(predicate)
                            .OrderByDescending(p => p.Id);

            result = result.OrderByDescending(p => p.Id);

            result = result.GroupBy(p => new
            {
                p.DepartmentId,
                p.CreatedUserId,
            })
            .Select(g => new
            {
                DepartmentId = g.Key.DepartmentId,
                CreatedUserId = g.Key.CreatedUserId,
                SendMessageRuleId = g.FirstOrDefault().SendMessageRuleId,
                SendMessageQueueId = g.FirstOrDefault().SendMessageQueueId,
                SendMessageType = g.FirstOrDefault().SendMessageType,

                SendTime = g.FirstOrDefault().SendTime,
                SendTitle = g.FirstOrDefault().SendTitle,
                SendBody = g.FirstOrDefault().SendBody,
                SendCustType = g.FirstOrDefault().SendCustType,
                RequestId = g.FirstOrDefault().RequestId,

                ProviderName = g.FirstOrDefault().ProviderName,

                // 發送通數
                TotalReceiverCount = g.Sum(p => p.TotalReceiverCount),
                // 花費點數
                TotalMessageCost = g.Sum(p => p.TotalMessageCost),
                // 成功接收
                TotalSuccess = g.Sum(p => p.TotalSuccess),
                // 傳送中通數
                TotalSending = g.Sum(p => p.TotalSending),
                // 逾期收訊
                TotalTimeout = g.Sum(p => p.TotalTimeout),

                CreatedTime = utcNow
            })
            .ToList()
            .Select(p => new SendMessageStatistic
            {
                DepartmentId = p.DepartmentId,
                CreatedUserId = p.CreatedUserId,
                SendMessageRuleId = p.SendMessageRuleId,
                SendMessageQueueId = p.SendMessageQueueId,
                SendMessageType = p.SendMessageType,

                SendTime = p.SendTime,
                SendTitle = p.SendTitle,
                SendBody = p.SendBody,
                SendCustType = p.SendCustType,
                RequestId = p.RequestId,

                ProviderName = p.ProviderName,

                // 發送通數
                TotalReceiverCount = p.TotalReceiverCount,
                // 花費點數
                TotalMessageCost = p.TotalMessageCost,
                // 成功接收
                TotalSuccess = p.TotalSuccess,
                // 傳送中通數
                TotalSending = p.TotalSending,
                // 逾期收訊
                TotalTimeout = p.TotalTimeout,
                // 資料建立時間
                CreatedTime = p.CreatedTime,
            })
            .AsQueryable()
            .OrderByDescending(p => p.Id);

            return result;
		}

        protected override IEnumerable<SendMessageStatisticModel> ConvertModel(IEnumerable<SendMessageStatisticModel> models)
        {
            var departments = context.Set<Department>().ToList();
            var users = context.Set<ApplicationUser>().ToList();

            foreach (var model in models)
            {
                if (model.DepartmentId.HasValue)
                {
                    model.DepartmentName = departments.Find(p => p.Id == model.DepartmentId).Name;
                }

                model.FullName = users.Find(p => p.Id == model.CreatedUserId).FullName;
            }

            return models;
        }

        protected override ReportDownloadModel ProduceFile(SectorSendMessageStatisticCriteriaModel criteria, IEnumerable<SendMessageStatisticModel> models)
        {
            switch (criteria.DownloadType)
            {
                case DownloadType.Statistic:
                    {
                        var result = models.Select(p => new
                        {
                            部門 = p.DepartmentName,
                            姓名 = p.FullName,
                            發送通數 = p.TotalReceiverCount,
                            發送扣點 = p.TotalMessageCost,
                        });

                        return ProduceZipFile(
                            fileName: "部門通數統計.zip",
                            zipEntries: new Dictionary<string, DataTable> 
                            { 
                                { "部門通數統計", Converter.ToDataTable(result.ToList()) },
                            }
                        );
                    }
                case DownloadType.All:
                    {
                        IEnumerable<SendMessageHistoryModel> historyModels = (Mapper.Map<IEnumerable<SendMessageHistory>, IEnumerable<SendMessageHistoryModel>>(GetSendMessageHistory(criteria))).ToList();

                        ////////////////////////////////////////
                        // protected override IEnumerable<SectorSendMessageHistoryModel> ConvertModel(IEnumerable<SectorSendMessageHistoryModel> models)

                        //var deaprtmentRepository = this.unitOfWork.Repository<Department>();
                        //var userRepository = this.unitOfWork.Repository<ApplicationUser>();

                        //int rowNo = 0;

                        //foreach (var model in models)
                        //{
                        //    model.RowNo = ++rowNo;

                        //    if (model.DepartmentId.HasValue)
                        //    {
                        //        model.DepartmentName = deaprtmentRepository.GetById(model.DepartmentId.Value).Name;
                        //    }

                        //    var user = userRepository.GetById(model.CreatedUserId);
                        //    model.UserName = user.UserName;
                        //    model.FullName = user.FullName;

                        //    model.DeliveryStatusChineseString = model.DeliveryStatusString; // TODO: DeliveryStatus 中文說明
                        //}

                        historyModels = SendMessageHistoryProfile.ConvertModel(historyModels, new UnitOfWork(context));

                        ////////////////////////////////////////

                        TimeSpan clientTimezoneOffset = ClientTimezoneOffset;
                        string timeFormat = Converter.Every8d_SentTime;

                        var result = historyModels.Select(p => new
                        {
                            簡訊類別 = AttributeHelper.GetColumnDescription(p.SendMessageType),
                            部門 = p.DepartmentName,
                            姓名 = p.FullName,
                            帳號 = p.UserName,
                            門號 = p.DestinationAddress,
                            發送時間 = Converter.ToLocalTimeString(p.SendTime, clientTimezoneOffset, timeFormat),
                            收訊時間 = Converter.ToLocalTimeString(p.SentDate, clientTimezoneOffset, timeFormat),
                            簡訊類別描述 = p.SendTitle,
                            發送內容 = p.SendBody,	
                            狀態 = p.DeliveryStatusChineseString,
                            發送扣點 = p.MessageCost,
                        });

                        return ProduceZipFile(
                            fileName: "部門發送紀錄.zip",
                            zipEntries: new Dictionary<string, DataTable> 
                            { 
                                { "部門發送紀錄", Converter.ToDataTable(result.ToList()) },
                            }
                        );
                    }
                default:
                    throw new Exception(string.Format("尚未實作下載類型為{0}的下載內容", criteria.DownloadType.ToString()));
            }
        }
    }
}
