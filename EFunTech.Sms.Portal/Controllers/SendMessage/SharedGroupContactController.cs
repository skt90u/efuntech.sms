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
	public class SharedGroupContactController : CrudApiController<SearchTextCriteriaModel, SharedGroupContactModel, SharedGroupContact, int>
	{
		public SharedGroupContactController(IUnitOfWork unitOfWork, ILogService logService)
			: base(unitOfWork, logService)
		{
		}

		protected override IOrderedQueryable<SharedGroupContact> DoGetList(SearchTextCriteriaModel criteria)
		{
            IQueryable<SharedGroupContact> result = this.unitOfWork.Repository<SharedGroupContact>().GetAll();

			var predicate = PredicateBuilder.True<SharedGroupContact>();
            predicate = predicate.And(p => p.ShareToUser.Id == CurrentUser.Id);

			var searchText = criteria.SearchText;
			if (!string.IsNullOrEmpty(searchText))
			{
                //var innerPredicate = PredicateBuilder.False<SharedGroupContact>();

                //innerPredicate = innerPredicate.Or(p => !string.IsNullOrEmpty(p.ShareToUserId) && p.ShareToUserId.Contains(searchText));

                //predicate = predicate.And(innerPredicate);
			}
			result = result.AsExpandable().Where(predicate);

            return result.OrderByDescending(p => p.GroupId);
		}

		protected override SharedGroupContact DoGet(int id)
		{
            throw new NotImplementedException();
		}

		protected override SharedGroupContact DoCreate(SharedGroupContactModel model, SharedGroupContact entity, out int id)
		{
            throw new NotImplementedException();
		}

		protected override void DoUpdate(SharedGroupContactModel model, int id, SharedGroupContact entity)
		{
            throw new NotImplementedException();
		}

		protected override void DoRemove(int id, SharedGroupContact entity)
		{
            throw new NotImplementedException();
		}

		protected override void DoRemove(List<int> ids, List<SharedGroupContact> entities)
		{
            throw new NotImplementedException();
		}

	}
}
