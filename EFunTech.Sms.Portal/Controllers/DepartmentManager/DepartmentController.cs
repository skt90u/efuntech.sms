using EFunTech.Sms.Portal.Models;
using EFunTech.Sms.Schema;
using System.Linq;
using EFunTech.Sms.Portal.Controllers.Common;
using EFunTech.Sms.Portal.Models.Common;
using JUtilSharp.Database;

using System.Collections.Generic;
using LinqKit;
using System;
using AutoMapper;

namespace EFunTech.Sms.Portal.Controllers
{
	public class DepartmentController : CrudApiController<SearchTextCriteriaModel, DepartmentModel, Department, int>
	{
		public DepartmentController(IUnitOfWork unitOfWork, ILogService logService)
			: base(unitOfWork, logService)
		{
		}

		protected override IOrderedQueryable<Department> DoGetList(SearchTextCriteriaModel criteria)
		{
            IQueryable<Department> result = this.repository.GetAll().AsQueryable();

			var predicate = PredicateBuilder.True<Department>();

            switch (CurrentUserRole)
            {
                case Role.Administrator:
                    {
                        // 列出所有部門
                        predicate = predicate.And(p => true);
                    }break;
                case Role.Supervisor:
                    {
                        // 列出建立的所有部門
                        predicate = predicate.And(p => p.CreatedUser.Id == CurrentUser.Id);
                    }break;
                case Role.DepartmentHead:
                    {
                        // 列出所屬部門
                        if (CurrentUser.Department != null)
                        {
                            predicate = predicate.And(p => p.Id == CurrentUser.Department.Id);
                        }
                        else
                        {
                            predicate = predicate.And(p => false);
                        }
                    }break;
                case Role.Employee:
                    {
                        // 列出所屬部門
                        if (CurrentUser.Department != null)
                        {
                            predicate = predicate.And(p => p.Id == CurrentUser.Department.Id);
                        }
                        else
                        {
                            predicate = predicate.And(p => false);
                        }
                    }break;
            }

			result = result.AsExpandable().Where(predicate);

			return result.OrderByDescending(p => p.Id);
		}

		protected override Department DoGet(int id)
		{
            return this.repository.GetById(id);
		}

		protected override Department DoCreate(DepartmentModel model, Department entity, out int id)
		{
			entity = new Department();
			entity.Name = model.Name;
			entity.Description = model.Description;
			entity.CreatedUser = CurrentUser;

			entity = this.repository.Insert(entity);
			id = entity.Id;

			return entity;
		}

		protected override void DoUpdate(DepartmentModel model, int id, Department entity)
		{
            entity.CreatedUser = CurrentUser;
			this.repository.Update(entity);
		}

		protected override void DoRemove(int id, Department entity)
		{
            // 必須先刪除此部門下，所有使用者
            var users = this.unitOfWork.Repository<ApplicationUser>().GetMany(p => p.Department != null && p.Department.Id == id).ToList();
            if (users.Count != 0)
            {
                string error = string.Format("部門【{0}】下還有 {1} 位使用者，分別是 {2}，請先刪除該部門下所有使用者，之後再刪除此部門",
                    entity.Name,
                    users.Count,
                    string.Join("、", users.Select(p => p.UserName)));
                throw new Exception(error);
            }

            //foreach (var user in users)
            //{
            //    user.Department = null;
            //    this.unitOfWork.Repository<ApplicationUser>().Update(user);
            //}

			this.repository.Delete(entity);
		}

		protected override void DoRemove(List<int> ids, List<Department> entities)
		{
            for (var i = 0; i < entities.Count; i++)
            {
                DoRemove(ids[i], entities[i]);
            }
		}

        protected override IEnumerable<DepartmentModel> ConvertModel(IEnumerable<DepartmentModel> models)
        {
            foreach (var model in models)
            {
                // 取出此部門對應使用者
                var Users = Mapper.Map<List<ApplicationUser>, List<ApplicationUserModel>>(this.repository.GetById(model.Id).Users.ToList());
                //model.Users = Users;

                // 一帆(3) = Department.Name + Department.Users.Count
                model.Summary = string.Format("{0}({1})", model.Name, Users.Count);
            }

            return models;            
        }
	}
}
