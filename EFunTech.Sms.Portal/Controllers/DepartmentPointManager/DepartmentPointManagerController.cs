using EFunTech.Sms.Portal.Models;
using EFunTech.Sms.Schema;
using System.Linq;
using EFunTech.Sms.Portal.Controllers.Common;
using EFunTech.Sms.Portal.Models.Common;
using JUtilSharp.Database;

using System.Collections.Generic;
using System;
using EFunTech.Sms.Portal.Models.Criteria;
using System.Web.Http;
using System.Net.Http;
using System.Net;
using System.Transactions;
using LinqKit;
using System.Data.SqlTypes;
using System.Data.Entity;
using System.Threading.Tasks;

namespace EFunTech.Sms.Portal.Controllers
{
	public class DepartmentPointManagerController : CrudApiController<DepartmentPointManagerCriteriaModel, ApplicationUserModel, ApplicationUser, string>
	{
        protected ApiControllerHelper apiControllerHelper;
        protected TradeService tradeService;

        public DepartmentPointManagerController(DbContext context, ILogService logService)
			: base(context, logService)
		{
            this.apiControllerHelper = new ApiControllerHelper(context, logService);
            this.tradeService = new TradeService(new UnitOfWork(context), logService);
        }

        //protected override IQueryable<ApplicationUser> DoGetList(DepartmentPointManagerCriteriaModel criteria)
        //{
        //    // 尋找目前使用者以及目前使用者的子帳號
        //    var result = this.repository.GetMany(p => p.ParentId == CurrentUser.Id || p.Id == CurrentUser.Id).AsQueryable();

        //    var predicate = PredicateBuilder.True<ApplicationUser>();
        //    var fullName = criteria.FullName;
        //    var userName = criteria.UserName;
        //    if (!string.IsNullOrEmpty(fullName))
        //    {
        //        predicate = predicate.And(p => p.FullName.Contains(fullName));
        //    }
        //    if (!string.IsNullOrEmpty(userName))
        //    {
        //        predicate = predicate.And(p => p.UserName.Contains(userName));
        //    }
        //    result = result.AsExpandable().Where(predicate);

        //    return result.OrderByDescending(p => p.Id);
        //}

        /// <summary>
        /// 查詢子帳號以及子帳號所建立的帳號
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns></returns>
        protected override IQueryable<ApplicationUser> DoGetList(DepartmentPointManagerCriteriaModel criteria)
        {
            var predicate = PredicateBuilder.True<ApplicationUser>();
            var fullName = criteria.FullName;
            var userName = criteria.UserName;
            
            if (!string.IsNullOrEmpty(fullName))
            {
                predicate = predicate.And(p => p.FullName.Contains(fullName));
            }
            
            if (!string.IsNullOrEmpty(userName))
            {
                predicate = predicate.And(p => p.UserName.Contains(userName));
            }

            // 尋找目前使用者以及目前使用者的子帳號或孫帳號
            IEnumerable<ApplicationUser> users = this.apiControllerHelper.GetDescendingUsersAndUser(CurrentUserId);

            // 尋找目前使用者以及目前使用者的子帳號
            var result = users.AsQueryable()
                              .AsExpandable()
                              .Where(predicate)
                              .OrderByDescending(p => p.Level)
                              .ThenBy(p => p.Id);

            return result;
        }

        protected override IEnumerable<ApplicationUserModel> ConvertModel(IEnumerable<ApplicationUserModel> models)
        {
            foreach (var model in models)
            {
                var isCurrentUser = model.Id == CurrentUserId;
                model.CanAllotPoint = !isCurrentUser;
                model.Checked = false;
                if(model.AllotSetting == null){
                    model.AllotSettingDesc = "無設定";
                }
                else
                {
                    if(model.AllotSetting.MonthlyAllot){
                        model.AllotSettingDesc = string.Format("定額撥給:每個月{0}號撥{1:0.00}點，回歸點數:否", 
                            model.AllotSetting.MonthlyAllotDay,
                            model.AllotSetting.MonthlyAllotPoint);
                    }
                    else{
                        model.AllotSettingDesc = string.Format("自動補點:點數下限{0:0.00}點，補滿至{1:0.00}點",
                            model.AllotSetting.LimitMinPoint,
                            model.AllotSetting.LimitMaxPoint);
                    }
                }
            }
            return models;
        }

        /// <summary>
        /// 手動撥點
        /// </summary>
        [System.Web.Http.HttpPost]
        [Route("api/DepartmentPointManager/AllotPoint")]
        public async Task<HttpResponseMessage> AllotPoint([FromBody] AllotPointModel model)
        {
            try
            {
                using (TransactionScope scope = context.CreateTransactionScope())
                {
                    var src = await context.Set<ApplicationUser>().FindAsync(CurrentUserId);
                    var dsts = await context.Set<ApplicationUser>().Where(p => model.ids.Contains(p.Id)).ToListAsync();

                    this.tradeService.AllotPoint(src, dsts, model.point);

                    scope.Complete();
                }

                var response = this.Request.CreateResponse(HttpStatusCode.OK);

                return response;
            }
            catch (Exception ex)
            {
                this.logService.Error(ex);

                throw;
            }
        }

        /// <summary>
        /// 建立播點設定
        /// </summary>
        [System.Web.Http.HttpPost]
        [Route("api/DepartmentPointManager/CreateAllotSetting")]
        public async Task<HttpResponseMessage> CreateAllotSetting([FromBody] CreateAllotSettingModel model)
        {
            try
            {
                using (TransactionScope scope = context.CreateTransactionScope())
                {
                    await context.DeleteAsync<AllotSetting>(p => model.ids.Contains(p.Owner.Id));

                    var users = await context.Set<ApplicationUser>().Where(p => model.ids.Contains(p.Id)).ToListAsync(); 

                    foreach (var user in users)
                    {
                        var allotSetting = new AllotSetting{
                            MonthlyAllot = model.MonthlyAllot,
                            MonthlyAllotDay = model.MonthlyAllotDay,
                            MonthlyAllotPoint = model.MonthlyAllotPoint,
                            LastAllotTime = null,
                            LimitMinPoint = model.LimitMinPoint,
                            LimitMaxPoint = model.LimitMaxPoint,
                            Owner = user,
                        };

                        await context.InsertAsync<AllotSetting>(allotSetting);
                    }

                    scope.Complete();
                }

                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                this.logService.Error(ex);

                throw;
            }
        }

        /// <summary>
        /// 刪除播點設定
        /// </summary>
        [System.Web.Http.HttpDelete]
        [Route("api/DepartmentPointManager/DeleteAllotSetting")]
        public async Task<HttpResponseMessage> DeleteAllotSetting([FromUri]string id)
        {
            try
            {
                using (TransactionScope scope = context.CreateTransactionScope())
                {
                    await context.DeleteAsync<AllotSetting>(p => p.Owner.Id == id);

                    scope.Complete();
                }

                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                this.logService.Error(ex);

                throw;
            }
        }

        /// <summary>
        /// 回收所有點數
        /// </summary>
        [System.Web.Http.HttpDelete]
        [Route("api/DepartmentPointManager/RecoveryPoint")]
        public async Task<HttpResponseMessage> RecoveryPoint([FromUri]string[] ids)
        {
            try
            {
                using (TransactionScope scope = context.CreateTransactionScope())
                {
                    // 由最下層的使用者開始取回
                    // select UserName from AspNetUsers Order By Level
                    var users = await context.Set<ApplicationUser>().Where(p => ids.Contains(p.Id)).OrderBy(p => p.Level).ToListAsync();

                    foreach (var user in users)
                    {
                        this.tradeService.RecoveryPoint(CurrentUser, user);
                    }
                    
                    scope.Complete();
                }

                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                this.logService.Error(ex);

                throw;
            }
        }
	}
}
