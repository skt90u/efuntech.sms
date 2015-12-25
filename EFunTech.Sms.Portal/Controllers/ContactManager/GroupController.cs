using EFunTech.Sms.Portal.Models;
using EFunTech.Sms.Schema;
using System.Linq;
using EFunTech.Sms.Portal.Controllers.Common;

using System.Collections.Generic;
using LinqKit;
using EFunTech.Sms.Portal.Models.Criteria;
using System.Threading.Tasks;
using System.Data.Entity;

namespace EFunTech.Sms.Portal.Controllers
{
    public class GroupController : AsyncCrudApiController<GroupCriteriaModel, GroupModel, Group, int>
	{
        private ISystemParameters systemParameters;

        public GroupController(ISystemParameters systemParameters, DbContext context, ILogService logService)
			: base(context, logService)
		{
            this.systemParameters = systemParameters;
		}

		protected override IQueryable<Group> DoGetList(GroupCriteriaModel criteria)
		{
            var predicate = PredicateBuilder.True<Group>();

            predicate = predicate.And(p => p.CreatedUserId == CurrentUserId);

			var searchText = criteria.SearchText;
			if (!string.IsNullOrEmpty(searchText))
			{
                var innerPredicate = PredicateBuilder.False<Group>();

                innerPredicate = innerPredicate.Or(p => !string.IsNullOrEmpty(p.Name) && p.Name.Contains(searchText));
                innerPredicate = innerPredicate.Or(p => !string.IsNullOrEmpty(p.Description) && p.Description.Contains(searchText));

                predicate = predicate.And(innerPredicate);
			}

            var result = context.Set<Group>()
                            .AsExpandable()
                            .Where(predicate);

            if (criteria.IncludeAllGroup)
            {
                result = result.Union(new List<Group> { new Group
                {
                    Id = 0, // Id = 0 stand for All Group
                    Name = "AllGroup",
                    Description = "所有群組",
                    Deletable = false,
                }});
            }
            
            return result.OrderByDescending(p => p.Id);
		}

        protected override async Task<Group> DoCreate(GroupModel model, Group entity)
		{
			entity = new Group();
            entity.CreatedUserId = CurrentUserId;
			entity.Name = model.Name;
			entity.Description = model.Description;
			entity.Deletable = true;

            entity = await context.InsertAsync(entity);

			return entity;
		}

        protected override async Task DoUpdate(GroupModel model, int id, Group entity)
		{
            await context.UpdateAsync(entity);
		}

        protected override async Task DoRemove(int id)
		{
            var groupId = id;

            if (systemParameters.ContactAtMostOneGroup) // 聯絡人只能對應至一個群組
            {
                var contactIds = context.Set<GroupContact>().Where(p => p.GroupId == groupId).Select(p => p.ContactId).ToList();

                // 刪除聯絡人與這個群組對應關係
                await context.DeleteAsync<GroupContact>(p => p.GroupId == groupId);

                // 刪除這個群組內所有聯絡人
                if (contactIds.Count != 0)
                    await context.DeleteAsync<Contact>(p => contactIds.Contains(p.Id));

                // 分享聯絡人 - 刪除其他帳號與這個群組的分享關係
                await context.DeleteAsync<SharedGroupContact>(p => p.GroupId == groupId);

                // 刪除群組
                await context.DeleteAsync<Group>(p => p.Id == groupId);
            }
            else
            {
                // 刪除聯絡人與這個群組對應關係
                await context.DeleteAsync<GroupContact>(p => p.GroupId == groupId);

                // 分享聯絡人 - 刪除其他帳號與這個群組的分享關係
                await context.DeleteAsync<SharedGroupContact>(p => p.GroupId == groupId);

                // 刪除群組            
                await context.DeleteAsync<Group>(p => p.Id == groupId);
            }
        }

        protected override IEnumerable<GroupModel> ConvertModel(IEnumerable<GroupModel> models)
        {
            foreach (var model in models)
            {
                model.Editable = model.Name != Group.CommonContactGroupName;
                model.Deletable = model.Name != Group.CommonContactGroupName;
            }

            return models;
        }
	}
}
