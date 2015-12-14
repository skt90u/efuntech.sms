using EFunTech.Sms.Core;
using EFunTech.Sms.Schema;
using Hangfire;
using JUtilSharp.Database;
using OneApi.Client.Impl;
using OneApi.Config;
using OneApi.Model;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Web;

namespace EFunTech.Sms.Portal
{
    /// <summary>
    /// 避免
    /// (1) 多個 BackgroundJob 發送相同的簡訊規則(sendMessageRuleId, sendTime)
    /// (2) 多個 BackgroundJob 接收相同的派送報表(requestId)
    /// </summary>
    public class UniqueJobList
    {
        private ISystemParameters systemParameters;
        private ILogService logService;
        private IUnitOfWork unitOfWork;

        public static readonly TimeSpan QueryInterval = new TimeSpan(0, 0, 10, 0, 0); // 一天

        private IRepository<UniqueJob> repository;

        public UniqueJobList(ISystemParameters systemParameters, ILogService logService, IUnitOfWork unitOfWork)
        {
            this.systemParameters = systemParameters;
            this.logService = logService;
            this.unitOfWork = unitOfWork;

            this.repository = this.unitOfWork.Repository<UniqueJob>();
        }

        /// <summary>
        /// 只有當 signature 不存在於 UniqueJobQueues
        /// 才會建立新的 UniqueJobQueue 物件，並回傳此物件
        /// 否則，將回傳 NULL
        /// </summary>
        public UniqueJob AddOrUpdate(string methodName, params object[] arguments)
        {
            try
            {
                string signature = string.Format("{0}({1})", methodName, string.Join(",", arguments));

                var job = this.repository.Get(p => p.Signature == signature);
                if (job != null)
                {
                    var expiryDate = DateTime.UtcNow.AddTicks(-1 * UniqueJobList.QueryInterval.Ticks);

                    if (job.CreatedTime >= expiryDate) // 超過指定時間，重設Job
                    {
                        return null; // Job仍在執行中，不要加入新的Job，回傳 NULL
                    }
                    else
                    {
                        job.Signature = signature;
                        job.Status = EfJobQueueStatus.Enqueued;
                        job.CreatedTime = DateTime.UtcNow;

                        this.repository.Update(job);
                    }
                }
                else
                {
                    job = new UniqueJob();

                    job.Signature = signature;
                    job.Status = EfJobQueueStatus.Enqueued;
                    job.CreatedTime = DateTime.UtcNow;

                    job = this.repository.Insert(job);
                }

                return job;
            }
            catch (Exception ex)
            {
                this.logService.Error(ex);
                return null;
            }
        }

        //public UniqueJob AddOrUpdate(string methodName, params object[] arguments)
        //{
        //    try
        //    {
        //        string signature = string.Format("{0}({1})", methodName, string.Join(",", arguments));

        //        if (this.repository.Any(p => p.Signature == signature))
        //            return null; // 已經存在，不要加入新的Job，回傳 NULL

        //        var job = new UniqueJob();
        //        job.Signature = signature;
        //        job.Status = EfJobQueueStatus.Enqueued;
        //        job.CreatedTime = DateTime.UtcNow;
                
        //        job = this.repository.Insert(job);
        //        return job;
        //    }
        //    catch (Exception ex)
        //    {
        //        this.logService.Error(ex);
        //        return null;
        //    }
        //}

        public void Remove(UniqueJob job)
        {
            try
            {
                this.repository.Delete(job);
            }
            catch (Exception ex)
            {
                this.logService.Error(ex);
            }
        }
    }
}