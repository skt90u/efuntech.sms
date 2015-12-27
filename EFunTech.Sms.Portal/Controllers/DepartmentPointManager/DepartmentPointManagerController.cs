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
        //    // �M��ثe�ϥΪ̥H�Υثe�ϥΪ̪��l�b��
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
        /// �d�ߤl�b���H�Τl�b���ҫإߪ��b��
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

            // �M��ثe�ϥΪ̥H�Υثe�ϥΪ̪��l�b���ή]�b��
            IEnumerable<ApplicationUser> users = this.apiControllerHelper.GetDescendingUsersAndUser(CurrentUser);

            // �M��ثe�ϥΪ̥H�Υثe�ϥΪ̪��l�b��
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
                    model.AllotSettingDesc = "�L�]�w";
                }
                else
                {
                    if(model.AllotSetting.MonthlyAllot){
                        model.AllotSettingDesc = string.Format("�w�B����:�C�Ӥ�{0}����{1:0.00}�I�A�^�k�I��:�_", 
                            model.AllotSetting.MonthlyAllotDay,
                            model.AllotSetting.MonthlyAllotPoint);
                    }
                    else{
                        model.AllotSettingDesc = string.Format("�۰ʸ��I:�I�ƤU��{0:0.00}�I�A�ɺ���{1:0.00}�I",
                            model.AllotSetting.LimitMinPoint,
                            model.AllotSetting.LimitMaxPoint);
                    }
                }
            }
            return models;
        }

        /// <summary>
        /// ��ʼ��I
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
        /// �إ߼��I�]�w
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
        /// �R�����I�]�w
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
        /// �^���Ҧ��I��
        /// </summary>
        [System.Web.Http.HttpDelete]
        [Route("api/DepartmentPointManager/RecoveryPoint")]
        public async Task<HttpResponseMessage> RecoveryPoint([FromUri]string[] ids)
        {
            try
            {
                using (TransactionScope scope = context.CreateTransactionScope())
                {
                    // �ѳ̤U�h���ϥΪ̶}�l���^
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
