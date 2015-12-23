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
	public class SharedGroupContactController : CrudApiController<SearchTextCriteriaModel, SharedGroupContactModel, SharedGroupContact, int>
	{
		public SharedGroupContactController(IUnitOfWork unitOfWork, ILogService logService)
			: base(unitOfWork, logService)
		{
		}

		protected override IQueryable<SharedGroupContact> DoGetList(SearchTextCriteriaModel criteria)
		{
			var predicate = PredicateBuilder.True<SharedGroupContact>();

            predicate = predicate.And(p => p.ShareToUser.Id == CurrentUserId);

			var searchText = criteria.SearchText;
			if (!string.IsNullOrEmpty(searchText))
			{
                //var innerPredicate = PredicateBuilder.False<SharedGroupContact>();

                //innerPredicate = innerPredicate.Or(p => !string.IsNullOrEmpty(p.ShareToUserId) && p.ShareToUserId.Contains(searchText));

                //predicate = predicate.And(innerPredicate);
			}

			var result = this.repository.DbSet
                             .AsExpandable()
                             .Where(predicate)
                             .OrderByDescending(p => p.GroupId);

            return result.OrderByDescending(p => p.GroupId);
		}

		protected override SharedGroupContact DoCreate(SharedGroupContactModel model, SharedGroupContact entity, out int id)
		{
            throw new NotImplementedException();
		}

		protected override void DoUpdate(SharedGroupContactModel model, int id, SharedGroupContact entity)
		{
            throw new NotImplementedException();
		}

		protected override void DoRemove(int id)
		{
            throw new NotImplementedException();
		}

        protected override void DoRemove(int[] ids)
		{
            throw new NotImplementedException();
		}
	}
}
