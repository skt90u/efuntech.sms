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

            var result = this.repository.DbSet.AsExpandable().Where(predicate);

            //if (criteria.IncludeAllGroup)
            //{
            //    var list = result.OrderByDescending(p => p.Id).ToList();

            //    list.Insert(0, new Group
            //    {
            //        Id = 0, // Id = 0 stand for All Group
            //        Name = "AllGroup",
            //        Description = "�Ҧ��s��",
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
                    Description = "�Ҧ��s��",
                    Deletable = false,
                }});
            }
            
            return result.OrderByDescending(p => p.Id);
		}

		protected override Group DoCreate(GroupModel model, Group entity, out int id)
		{
			entity = new Group();
            entity.CreatedUserId = CurrentUserId;
			entity.Name = model.Name;
			entity.Description = model.Description;
			entity.Deletable = true;

			entity = this.repository.Insert(entity);
			id = entity.Id;

			return entity;
		}

		protected override void DoUpdate(GroupModel model, int id, Group entity)
		{
			this.repository.Update(entity);
		}

		protected override void DoRemove(int id)
		{
            var groupId = id;

            var groupContactRepository = this.unitOfWork.Repository<GroupContact>();
            var sharedGroupContactRepository = this.unitOfWork.Repository<SharedGroupContact>();
            var contactRepository = this.unitOfWork.Repository<Contact>();
            
            if (systemParameters.ContactAtMostOneGroup) // �p���H�u������ܤ@�Ӹs��
            {
                var contactIds = groupContactRepository.DbSet.Where(p => p.GroupId == groupId).Select(p => p.ContactId).ToList();

                // �R���p���H�P�o�Ӹs�չ������Y
                groupContactRepository.Delete(p => p.GroupId == groupId);

                // �R���o�Ӹs�դ��Ҧ��p���H
                if (contactIds.Count != 0)
                    contactRepository.Delete(p => contactIds.Contains(p.Id));

                // �����p���H - �R����L�b���P�o�Ӹs�ժ��������Y
                sharedGroupContactRepository.Delete(p => p.GroupId == groupId);

                // �R���s��
                this.repository.Delete(p => p.Id == groupId);
            }
            else
            {
                // �R���p���H�P�o�Ӹs�չ������Y
                groupContactRepository.Delete(p => p.GroupId == groupId);

                // �����p���H - �R����L�b���P�o�Ӹs�ժ��������Y
                sharedGroupContactRepository.Delete(p => p.GroupId == groupId);

                // �R���s��            
                this.repository.Delete(p => p.Id == groupId);
            }
		}

        protected override void DoRemove(int[] ids)
		{
            throw new NotImplementedException();
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
