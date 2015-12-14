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

namespace EFunTech.Sms.Portal.Controllers
{
	public class GroupController : CrudApiController<GroupCriteriaModel, GroupModel, Group, int>
	{
        private ISystemParameters systemParameters;

        public GroupController(ISystemParameters systemParameters, IUnitOfWork unitOfWork, ILogService logService)
			: base(unitOfWork, logService)
		{
            this.systemParameters = systemParameters;
		}

		protected override IOrderedQueryable<Group> DoGetList(GroupCriteriaModel criteria)
		{
            var result = CurrentUser.Groups.AsQueryable();

            var predicate = PredicateBuilder.True<Group>();
			var searchText = criteria.SearchText;
			if (!string.IsNullOrEmpty(searchText))
			{
                var innerPredicate = PredicateBuilder.False<Group>();

                innerPredicate = innerPredicate.Or(p => !string.IsNullOrEmpty(p.Name) && p.Name.Contains(searchText));
                innerPredicate = innerPredicate.Or(p => !string.IsNullOrEmpty(p.Description) && p.Description.Contains(searchText));

                predicate = predicate.And(innerPredicate);
			}
			result = result.AsExpandable().Where(predicate);

            //if (criteria.IncludeAllGroup)
            //{
            //    var list = result.OrderByDescending(p => p.Id).ToList();

            //    list.Insert(0, new Group
            //    {
            //        Id = 0, // Id = 0 stand for All Group
            //        Name = "AllGroup",
            //        Description = "所有群組",
            //        Deletable = false,
            //    });

            //    return list.AsQueryable().OrderBy(p => p.Id);
            //}
            //else
            //{
            //    return result.OrderBy(p => p.Id);
            //}

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

		protected override Group DoGet(int id)
		{
			return CurrentUser.Groups.Where(p => p.Id == id).FirstOrDefault();
		}

		protected override Group DoCreate(GroupModel model, Group entity, out int id)
		{
			entity = new Group();
            entity.CreatedUserId = CurrentUser.Id;
			entity.CreatedUser = CurrentUser;
			entity.Name = model.Name;
			entity.Description = model.Description;
			entity.Deletable = true;

			entity = this.repository.Insert(entity);
			id = entity.Id;

			return entity;
		}

		protected override void DoUpdate(GroupModel model, int id, Group entity)
		{
            if (!CurrentUser.Groups.Any(p => p.Id == id))
				return;

			this.repository.Update(entity);
		}

		protected override void DoRemove(int id, Group entity)
		{
            if (!CurrentUser.Groups.Any(p => p.Id == id))
				return;

            var groupContactRepository = this.unitOfWork.Repository<GroupContact>();
            var sharedGroupContactRepository = this.unitOfWork.Repository<SharedGroupContact>();
            var contactRepository = this.unitOfWork.Repository<Contact>();
            
            if (systemParameters.ContactAtMostOneGroup) // 聯絡人只能對應至一個群組
            {
                var contactIds = groupContactRepository.GetMany(p => p.GroupId == entity.Id).Select(p => p.ContactId).ToList();

                // 刪除聯絡人與這個群組對應關係
                groupContactRepository.Delete(p => p.GroupId == entity.Id);

                // 刪除這個群組內所有聯絡人
                contactRepository.Delete(p => contactIds.Contains(p.Id));

                // 分享聯絡人 - 刪除其他帳號與這個群組的分享關係
                sharedGroupContactRepository.Delete(p => p.GroupId == entity.Id);

                // 刪除群組
                this.repository.Delete(entity);
            }
            else
            {
                // 刪除聯絡人與這個群組對應關係
                groupContactRepository.Delete(p => p.GroupId == entity.Id);

                // 分享聯絡人 - 刪除其他帳號與這個群組的分享關係
                sharedGroupContactRepository.Delete(p => p.GroupId == entity.Id);

                // 刪除群組            
                this.repository.Delete(entity);
            }
		}

		protected override void DoRemove(List<int> ids, List<Group> entities)
		{
            throw new NotImplementedException();

            //if (!CurrentUser.Groups.Any(p => ids.Contains(p.Id)))
            //    return;

            //this.repository.Delete(p => ids.Contains(p.Id));
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
