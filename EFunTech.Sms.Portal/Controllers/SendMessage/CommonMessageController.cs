using EFunTech.Sms.Portal.Models;
using EFunTech.Sms.Schema;
using System.Linq;
using EFunTech.Sms.Portal.Controllers.Common;
using EFunTech.Sms.Portal.Models.Common;
using JUtilSharp.Database;
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

		protected override IQueryable<CommonMessage> DoGetList(SearchTextCriteriaModel criteria)
		{
			var predicate = PredicateBuilder.True<CommonMessage>();

            predicate = predicate.And(p => p.CreatedUserId == CurrentUserId);

			var searchText = criteria.SearchText;
			if (!string.IsNullOrEmpty(searchText))
			{
                var innerPredicate = PredicateBuilder.False<CommonMessage>();

                innerPredicate = innerPredicate.Or(p => !string.IsNullOrEmpty(p.Subject) && p.Subject.Contains(searchText));
                innerPredicate = innerPredicate.Or(p => !string.IsNullOrEmpty(p.Content) && p.Content.Contains(searchText));

                predicate = predicate.And(innerPredicate);
			}

            return this.repository.DbSet
                       .AsExpandable()
                       .Where(predicate)
                       .OrderByDescending(p => p.Id);
		}

		protected override CommonMessage DoCreate(CommonMessageModel model, CommonMessage entity, out int id)
		{
			entity = new CommonMessage();
			entity.Subject = model.Subject;
			entity.Content = model.Content;
            entity.UpdatedTime = DateTime.UtcNow;
			entity.CreatedUserId = CurrentUserId;

			entity = this.repository.Insert(entity);
			id = entity.Id;

			return entity;
		}

		protected override void DoUpdate(CommonMessageModel model, int id, CommonMessage entity)
		{
            entity.UpdatedTime = DateTime.UtcNow;

			this.repository.Update(entity);
		}

		protected override void DoRemove(int id)
		{
			this.repository.Delete(p=> p.Id == id);
		}

		protected override void DoRemove(int[] ids)
		{
			this.repository.Delete(p => ids.Contains(p.Id));
		}

	}
}
