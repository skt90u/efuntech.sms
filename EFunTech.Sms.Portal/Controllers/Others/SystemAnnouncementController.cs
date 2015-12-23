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
using EntityFramework.Caching;

namespace EFunTech.Sms.Portal.Controllers
{
	public class SystemAnnouncementController : CrudApiController<SystemAnnouncementCriteriaModel, SystemAnnouncementModel, SystemAnnouncement, int>
	{
		public SystemAnnouncementController(IUnitOfWork unitOfWork, ILogService logService) : base(unitOfWork, logService) {}

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

            var result = this.repository.DbSet
                                .AsExpandable()
                                .Where(predicate)
                                .OrderByDescending(p => p.PublishDate)
                                .ThenByDescending(p => p.CreatedTime);

            return result;
		}

		protected override SystemAnnouncement DoCreate(SystemAnnouncementModel model, SystemAnnouncement entity, out int id)
		{
			entity = new SystemAnnouncement();
			entity.IsVisible = model.IsVisible;
			entity.PublishDate = model.PublishDate;
			entity.Announcement = model.Announcement;
            entity.CreatedTime = DateTime.UtcNow;
            entity.CreatedUserId = CurrentUserId;

			entity = this.repository.Insert(entity);
			id = entity.Id;

            CacheManager.Current.Expire("SystemAnnouncements");
			return entity;
		}

		protected override void DoUpdate(SystemAnnouncementModel model, int id, SystemAnnouncement entity)
		{
			this.repository.Update(entity);
            CacheManager.Current.Expire("SystemAnnouncements");
        }

        protected override void DoRemove(int id)
        {
            this.repository.Delete(p=> p.Id == id);
            CacheManager.Current.Expire("SystemAnnouncements");
        }

        protected override void DoRemove(int[] ids)
        {
            this.repository.Delete(p => ids.Contains(p.Id));
            CacheManager.Current.Expire("SystemAnnouncements");
        }

	}
}
