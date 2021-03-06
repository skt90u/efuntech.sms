using EFunTech.Sms.Portal.Models;
using EFunTech.Sms.Schema;
using System.Linq;
using EFunTech.Sms.Portal.Controllers.Common;
using EFunTech.Sms.Portal.Models.Common;
using System;
using LinqKit;
using System.Threading.Tasks;
using System.Data.Entity;

namespace EFunTech.Sms.Portal.Controllers
{
    public class BlacklistController : CrudApiController<SearchTextCriteriaModel, BlacklistModel, Blacklist, int>
    {
        public BlacklistController(DbContext context, ILogService logService)
            : base(context, logService)
        {
        }

        protected override IQueryable<Blacklist> DoGetList(SearchTextCriteriaModel criteria)
        {
            var predicate = PredicateBuilder.True<Blacklist>();

            predicate = predicate.And(p => p.CreatedUserId == CurrentUserId);

            var searchText = criteria.SearchText;
            if (!string.IsNullOrEmpty(searchText))
            {
                var innerPredicate = PredicateBuilder.False<Blacklist>();

                innerPredicate = innerPredicate.Or(p => !string.IsNullOrEmpty(p.Name) && p.Name.Contains(searchText));
                innerPredicate = innerPredicate.Or(p => !string.IsNullOrEmpty(p.Mobile) && p.Mobile.Contains(searchText));
                innerPredicate = innerPredicate.Or(p => !string.IsNullOrEmpty(p.UpdatedUserName) && p.UpdatedUserName.Contains(searchText));
                innerPredicate = innerPredicate.Or(p => !string.IsNullOrEmpty(p.Remark) && p.Remark.Contains(searchText));

                predicate = predicate.And(innerPredicate);
            }

            var result = context.Set<Blacklist>()
                            .AsExpandable()
                            .Where(predicate)
                            .OrderByDescending(p => p.Id);

            return result;
        }

        protected override async Task<Blacklist> DoCreate(BlacklistModel model, Blacklist entity)
        {
            entity = new Blacklist();

            entity.Name = model.Name;
            entity.Mobile = model.Mobile;
            entity.E164Mobile = MobileUtil.GetE164PhoneNumber(model.Mobile);
            entity.Region = MobileUtil.GetRegionName(model.Mobile);
            entity.Enabled = model.Enabled;

            entity.Remark = model.Remark;
            entity.UpdatedTime = DateTime.UtcNow;
            entity.CreatedUserId = CurrentUserId;
            entity.UpdatedUserName = CurrentUserName;

            entity = await context.InsertAsync(entity);

            return entity;
        }

        protected override async Task DoUpdate(BlacklistModel model, int id, Blacklist entity)
        {
            entity.E164Mobile = MobileUtil.GetE164PhoneNumber(model.Mobile);
            entity.Region = MobileUtil.GetRegionName(model.Mobile);
            entity.UpdatedTime = DateTime.UtcNow;
            entity.UpdatedUserName = CurrentUserName;

            await context.UpdateAsync(entity);
        }

        protected override async Task DoRemove(int id) 
        {
            await context.DeleteAsync<Blacklist>(p => p.Id == id);
        }

        protected override async Task DoRemove(int[] ids) 
        {
            await context.DeleteAsync<Blacklist>(p => ids.Contains(p.Id));
        }
    }
}
