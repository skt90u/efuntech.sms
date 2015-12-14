using EFunTech.Sms.Portal.Models;
using EFunTech.Sms.Schema;
using System.Linq;
using EFunTech.Sms.Portal.Controllers.Common;
using EFunTech.Sms.Portal.Models.Common;
using JUtilSharp.Database;

using System.Collections.Generic;
using LinqKit;
using System;

namespace EFunTech.Sms.Portal.Controllers
{
	public class CommonMessageController : CrudApiController<SearchTextCriteriaModel, CommonMessageModel, CommonMessage, int>
	{
		public CommonMessageController(IUnitOfWork unitOfWork, ILogService logService)
			: base(unitOfWork, logService)
		{
		}

		protected override IOrderedQueryable<CommonMessage> DoGetList(SearchTextCriteriaModel criteria)
		{
			IQueryable<CommonMessage> result = CurrentUser.CommonMessages.AsQueryable();

			var predicate = PredicateBuilder.True<CommonMessage>();
			var searchText = criteria.SearchText;
			if (!string.IsNullOrEmpty(searchText))
			{
                var innerPredicate = PredicateBuilder.False<CommonMessage>();

                innerPredicate = innerPredicate.Or(p => !string.IsNullOrEmpty(p.Subject) && p.Subject.Contains(searchText));
                innerPredicate = innerPredicate.Or(p => !string.IsNullOrEmpty(p.Content) && p.Content.Contains(searchText));

                predicate = predicate.And(innerPredicate);
			}
			result = result.AsExpandable().Where(predicate);

			return result.OrderByDescending(p => p.Id);
		}

		protected override CommonMessage DoGet(int id)
		{
			return CurrentUser.CommonMessages.Where(p => p.Id == id).FirstOrDefault();
		}

		protected override CommonMessage DoCreate(CommonMessageModel model, CommonMessage entity, out int id)
		{
			entity = new CommonMessage();
			entity.Subject = model.Subject;
			entity.Content = model.Content;
            entity.UpdatedTime = DateTime.UtcNow;
			entity.CreatedUser = CurrentUser;

			entity = this.repository.Insert(entity);
			id = entity.Id;

			return entity;
		}

		protected override void DoUpdate(CommonMessageModel model, int id, CommonMessage entity)
		{
			if (!CurrentUser.CommonMessages.Any(p => p.Id == id))
				return;

            entity.UpdatedTime = DateTime.UtcNow;

			this.repository.Update(entity);
		}

		protected override void DoRemove(int id, CommonMessage entity)
		{
			if (!CurrentUser.CommonMessages.Any(p => p.Id == id))
				return;

			this.repository.Delete(entity);
		}

		protected override void DoRemove(List<int> ids, List<CommonMessage> entities)
		{
			if (!CurrentUser.CommonMessages.Any(p => ids.Contains(p.Id)))
				return;

			this.repository.Delete(p => ids.Contains(p.Id));
		}

	}
}
