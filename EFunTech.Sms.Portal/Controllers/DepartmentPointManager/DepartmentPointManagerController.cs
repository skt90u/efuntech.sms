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

namespace EFunTech.Sms.Portal.Controllers
{
	public class DepartmentPointManagerController : CrudApiController<DepartmentPointManagerCriteriaModel, ApplicationUserModel, ApplicationUser, string>
	{
		public DepartmentPointManagerController(IUnitOfWork unitOfWork, ILogService logService)
			: base(unitOfWork, logService)
		{
		}

        //protected override IOrderedQueryable<ApplicationUser> DoGetList(DepartmentPointManagerCriteriaModel criteria)
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
        protected override IOrderedQueryable<ApplicationUser> DoGetList(DepartmentPointManagerCriteriaModel criteria)
        {
            // �M��ثe�ϥΪ̥H�Υثe�ϥΪ̪��l�b���ή]�b��
            List<ApplicationUser> users = this.apiControllerHelper.GetDescendingUsersAndUser(CurrentUser);

            // �M��ثe�ϥΪ̥H�Υثe�ϥΪ̪��l�b��
            var result = users.AsQueryable();

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
            
            result = result.AsExpandable().Where(predicate);

            // �Ѥj��p�ƦC
            return result.OrderByDescending(p => p.Level).ThenBy(p => p.Id);
        }

		protected override ApplicationUser DoGet(string id)
		{
            return this.unitOfWork.Repository<ApplicationUser>().GetById(id);
		}

		protected override ApplicationUser DoCreate(ApplicationUserModel model, ApplicationUser entity, out string id)
		{
            throw new NotImplementedException();
		}

		protected override void DoUpdate(ApplicationUserModel model, string id, ApplicationUser entity)
		{
            throw new NotImplementedException();
		}

		protected override void DoRemove(string id, ApplicationUser entity)
		{
            throw new NotImplementedException();
		}

		protected override void DoRemove(List<string> ids, List<ApplicationUser> entities)
		{
            throw new NotImplementedException();
		}

        protected override IEnumerable<ApplicationUserModel> ConvertModel(IEnumerable<ApplicationUserModel> models)
        {
            foreach (var model in models)
            {
                var isCurrentUser = model.Id == CurrentUser.Id;
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
        public HttpResponseMessage AllotPoint([FromBody] AllotPointModel model)
        {
            try
            {
                using (TransactionScope scope = this.unitOfWork.CreateTransactionScope())
                {
                    var src = this.repository.GetById(CurrentUser.Id);
                    var dsts = this.repository.GetMany(p => model.ids.Contains(p.Id)).ToList(); // �w�g�}�Ҥ@�ӻP�o�� Command ������ DataReader�A�������N������
                    this.tradeService.AllotPoint(src, dsts, model.point);
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
        /// �إ߼��I�]�w
        /// </summary>
        [System.Web.Http.HttpPost]
        [Route("api/DepartmentPointManager/CreateAllotSetting")]
        public HttpResponseMessage CreateAllotSetting([FromBody] CreateAllotSettingModel model)
        {
            try
            {
                using (TransactionScope scope = this.unitOfWork.CreateTransactionScope())
                {
                    this.unitOfWork.Repository<AllotSetting>().Delete(p => model.ids.Contains(p.Owner.Id));

                    var users = this.repository.GetMany(p => model.ids.Contains(p.Id)).ToList(); // �w�g�}�Ҥ@�ӻP�o�� Command ������ DataReader�A�������N������

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

                        
                        this.unitOfWork.Repository<AllotSetting>().Insert(allotSetting);
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
        public HttpResponseMessage DeleteAllotSetting([FromUri]string id)
        {
            try
            {
                using (TransactionScope scope = this.unitOfWork.CreateTransactionScope())
                {
                    this.unitOfWork.Repository<AllotSetting>().Delete(p => p.Owner.Id == id);

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
        public virtual HttpResponseMessage RecoveryPoint([FromUri]string[] ids)
        {
            try
            {
                List<ApplicationUser> entities = new List<ApplicationUser>();

                foreach (var id in ids)
                {
                    ApplicationUser entity = this.repository.GetById(id);
                    if (entity == null)
                    {
                        throw new HttpResponseException(HttpStatusCode.NotFound);
                    }
                    entities.Add(entity);
                }

                using (TransactionScope scope = this.unitOfWork.CreateTransactionScope())
                {
                    // �ѳ̤U�h���ϥΪ̶}�l���^
                    // select UserName from AspNetUsers Order By Level
                    var users = entities.OrderBy(p => p.Level);

                    foreach(var user in users)
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
