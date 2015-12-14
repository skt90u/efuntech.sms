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
	public class SignatureController : CrudApiController<SearchTextCriteriaModel, SignatureModel, Signature, int>
	{
		public SignatureController(IUnitOfWork unitOfWork, ILogService logService)
			: base(unitOfWork, logService)
		{
		}

		protected override IOrderedQueryable<Signature> DoGetList(SearchTextCriteriaModel criteria)
		{
			IQueryable<Signature> result = CurrentUser.Signatures.AsQueryable();

			var predicate = PredicateBuilder.True<Signature>();
			var searchText = criteria.SearchText;
			if (!string.IsNullOrEmpty(searchText))
			{
                var innerPredicate = PredicateBuilder.False<Signature>();

                innerPredicate = innerPredicate.Or(p => !string.IsNullOrEmpty(p.Subject) && p.Subject.Contains(searchText));
                innerPredicate = innerPredicate.Or(p => !string.IsNullOrEmpty(p.Content) && p.Content.Contains(searchText));

                predicate = predicate.And(innerPredicate);
			}
			result = result.AsExpandable().Where(predicate);

			return result.OrderByDescending(p => p.Id);
		}

		protected override Signature DoGet(int id)
		{
			return CurrentUser.Signatures.Where(p => p.Id == id).FirstOrDefault();
		}

		protected override Signature DoCreate(SignatureModel model, Signature entity, out int id)
		{
			entity = new Signature();
			entity.Subject = model.Subject;
			entity.Content = model.Content;
            entity.UpdatedTime = DateTime.UtcNow;
			entity.CreatedUser = CurrentUser;

			entity = this.repository.Insert(entity);
			id = entity.Id;

			return entity;
		}

		protected override void DoUpdate(SignatureModel model, int id, Signature entity)
		{
			if (!CurrentUser.Signatures.Any(p => p.Id == id))
				return;

            entity.UpdatedTime = DateTime.UtcNow;

			this.repository.Update(entity);
		}

		protected override void DoRemove(int id, Signature entity)
		{
			if (!CurrentUser.Signatures.Any(p => p.Id == id))
				return;

			this.repository.Delete(entity);
		}

		protected override void DoRemove(List<int> ids, List<Signature> entities)
		{
			if (!CurrentUser.Signatures.Any(p => ids.Contains(p.Id)))
				return;

			this.repository.Delete(p => ids.Contains(p.Id));
		}

	}
}
