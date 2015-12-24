using EFunTech.Sms.Portal.Models;
using EFunTech.Sms.Schema;
using System.Linq;
using EFunTech.Sms.Portal.Controllers.Common;
using LinqKit;
using System;
using EFunTech.Sms.Portal.Models.Criteria;
using EntityFramework.Caching;
using System.Data.Entity;
using System.Threading.Tasks;

namespace EFunTech.Sms.Portal.Controllers
{
    public class SystemAnnouncementController : AsyncCrudApiController<SystemAnnouncementCriteriaModel, SystemAnnouncementModel, SystemAnnouncement, int>
	{
        public SystemAnnouncementController(DbContext context, ILogService logService)
            : base(context, logService)
        {
        }

		protected override IQueryable<SystemAnnouncement> DoGetList(SystemAnnouncementCriteriaModel criteria)
		{
			var predicate = PredicateBuilder.True<SystemAnnouncement>();

			var searchText = criteria.SearchText;
			if (!string.IsNullOrEmpty(searchText))
			{
				var innerPredicate = PredicateBuilder.False<SystemAnnouncement>();
				innerPredicate = innerPredicate.Or(p => !string.IsNullOrEmpty(p.Announcement) && p.Announcement.Contains(searchText));
				predicate = predicate.And(innerPredicate);
			}

            var result = context.Set<SystemAnnouncement>()
                                .AsExpandable()
                                .Where(predicate)
                                .OrderByDescending(p => p.PublishDate)
                                .ThenByDescending(p => p.CreatedTime);

            return result;
		}

        protected override async Task<SystemAnnouncement> DoCreate(SystemAnnouncementModel model, SystemAnnouncement entity)
		{
			entity = new SystemAnnouncement();
			entity.IsVisible = model.IsVisible;
			entity.PublishDate = model.PublishDate;
			entity.Announcement = model.Announcement;
            entity.CreatedTime = DateTime.UtcNow;
            entity.CreatedUserId = CurrentUserId;

            entity = await context.InsertAsync(entity);

            CacheManager.Current.Expire("SystemAnnouncements");

			return entity;
		}

        protected override async Task<int> DoUpdate(SystemAnnouncementModel model, int id, SystemAnnouncement entity)
        {
            int result = await context.UpdateAsync(entity);
            CacheManager.Current.Expire("SystemAnnouncements");
            return result;
        }

        protected override async Task<int> DoRemove(int id) 
        {
            int result = await context.DeleteAsync<SystemAnnouncement>(p=> p.Id == id);
            CacheManager.Current.Expire("SystemAnnouncements");
            return result;
        }

        protected override async Task<int> DoRemove(int[] ids) 
        {
            int result = await context.DeleteAsync<SystemAnnouncement>(p => ids.Contains(p.Id));
            CacheManager.Current.Expire("SystemAnnouncements");
            return result;
        }

    }
}
