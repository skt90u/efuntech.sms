using EFunTech.Sms.Portal.Models;
using EFunTech.Sms.Schema;
using System.Linq;
using EFunTech.Sms.Portal.Controllers.Common;
using EFunTech.Sms.Portal.Models.Common;
using JUtilSharp.Database;
using LinqKit;
using System;
using System.Data.Entity;
using System.Threading.Tasks;

namespace EFunTech.Sms.Portal.Controllers
{
	public class CommonMessageController : AsyncCrudApiController<SearchTextCriteriaModel, CommonMessageModel, CommonMessage, int>
	{
        public CommonMessageController(DbContext context, ILogService logService)
            : base(context, logService)
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

            return context.Set<CommonMessage>()
                       .AsExpandable()
                       .Where(predicate)
                       .OrderByDescending(p => p.Id);
		}

        protected override async Task<CommonMessage> DoCreate(CommonMessageModel model, CommonMessage entity)
		{
			entity = new CommonMessage();
			entity.Subject = model.Subject;
			entity.Content = model.Content;
            entity.UpdatedTime = DateTime.UtcNow;
			entity.CreatedUserId = CurrentUserId;

			entity = await context.InsertAsync(entity);

			return entity;
		}

        protected override async Task DoUpdate(CommonMessageModel model, int id, CommonMessage entity)
		{
            entity.UpdatedTime = DateTime.UtcNow;

			await context.UpdateAsync(entity);
		}

        protected override async Task DoRemove(int id)
        {
            await context.DeleteAsync<CommonMessageModel>(p => p.Id == id);
        }

        protected override async Task DoRemove(int[] ids)
        {
            await context.DeleteAsync<CommonMessageModel>(p => ids.Contains(p.Id));
        }
	}
}
