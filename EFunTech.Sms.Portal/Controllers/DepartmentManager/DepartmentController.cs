using EFunTech.Sms.Portal.Models;
using EFunTech.Sms.Schema;
using System.Linq;
using EFunTech.Sms.Portal.Controllers.Common;
using EFunTech.Sms.Portal.Models.Common;
using System.Collections.Generic;
using LinqKit;
using System;
using System.Data.Entity;
using System.Threading.Tasks;

namespace EFunTech.Sms.Portal.Controllers
{
    public class DepartmentController : AsyncCrudApiController<SearchTextCriteriaModel, DepartmentModel, Department, int>
	{
        public DepartmentController(DbContext context, ILogService logService)
            : base(context, logService)
        {
        }

        protected override IQueryable<Department> DoGetList(SearchTextCriteriaModel criteria)
		{
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
                        predicate = predicate.And(p => p.CreatedUser.Id == CurrentUserId);
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

			var result = context.Set<Department>()
                            .AsExpandable()
                            .Where(predicate)
                            .OrderByDescending(p => p.Id);

            return result;
		}

        protected override async Task<Department> DoCreate(DepartmentModel model, Department entity)
		{
			entity = new Department();
			entity.Name = model.Name;
			entity.Description = model.Description;
			entity.CreatedUserId = CurrentUserId;

            entity = await context.InsertAsync(entity);

			return entity;
		}

        protected override async Task DoUpdate(DepartmentModel model, int id, Department entity)
		{
            await context.UpdateAsync(entity);
		}

        protected override async Task DoRemove(int id) 
		{
            // 必須先刪除此部門下，所有使用者
            var users = await context.Set<ApplicationUser>().Where(p => p.Department != null && p.Department.Id == id).ToListAsync();
            if (users.Count != 0)
            {
                var entity = await DoGet(id);

                string error = string.Format("部門【{0}】下還有 {1} 位使用者，分別是 {2}，請先刪除該部門下所有使用者，之後再刪除此部門",
                    entity.Name,
                    users.Count,
                    string.Join("、", users.Select(p => p.UserName)));
                throw new Exception(error);
            }

			await context.DeleteAsync< Department>(p => p.Id == id);
		}

        protected override async Task DoRemove(int[] ids) 
		{
            for (var i = 0; i < ids.Length; i++)
            {
                await DoRemove(ids[i]);
            }
        }

        protected override IEnumerable<DepartmentModel> ConvertModel(IEnumerable<DepartmentModel> models)
        {
            var departmentInfos = context.Set<Department>()
                                         .Include(p => p.Users)
                                         .Select(p => new {
                                            Id = p.Id,
                                            Name = p.Name,
                                            UserCount = p.Users.Count
                                         });

            foreach (var model in models)
            {
                // 取出此部門對應使用者
                var departmentInfo = departmentInfos.FirstOrDefault(p => p.Id == model.Id);
                if (departmentInfo != null)
                {
                // 一帆(3) = Department.Name + Department.Users.Count
                    model.Summary = string.Format("{0}({1})", departmentInfo.Name, departmentInfo.UserCount);
                }
            }

            return models;            
        }
	}
}
