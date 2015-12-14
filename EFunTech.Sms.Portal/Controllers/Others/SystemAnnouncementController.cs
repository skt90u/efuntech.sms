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
	public class SystemAnnouncementController : CrudApiController<SystemAnnouncementCriteriaModel, SystemAnnouncementModel, SystemAnnouncement, int>
	{
		public SystemAnnouncementController(IUnitOfWork unitOfWork, ILogService logService)
			: base(unitOfWork, logService)
		{
		}

		protected override IOrderedQueryable<SystemAnnouncement> DoGetList(SystemAnnouncementCriteriaModel criteria)
		{
            IQueryable<SystemAnnouncement> result = this.repository.GetAll().AsQueryable();

			var predicate = PredicateBuilder.True<SystemAnnouncement>();

            //predicate = predicate.And(p => p.PublishDate >= criteria.StartDate);
            //predicate = predicate.And(p => p.PublishDate <= criteria.EndDate);

            //predicate = predicate.And(p => p.IsVisible == criteria.IsVisible);

			var searchText = criteria.SearchText;
			if (!string.IsNullOrEmpty(searchText))
			{
				var innerPredicate = PredicateBuilder.False<SystemAnnouncement>();

				innerPredicate = innerPredicate.Or(p => !string.IsNullOrEmpty(p.Announcement) && p.Announcement.Contains(searchText));

				predicate = predicate.And(innerPredicate);
			}
			result = result.AsExpandable().Where(predicate);

            return result.OrderByDescending(p => p.PublishDate).ThenByDescending(p => p.CreatedTime);
		}

		protected override SystemAnnouncement DoGet(int id)
		{
            return this.repository.GetById(id);
		}

		protected override SystemAnnouncement DoCreate(SystemAnnouncementModel model, SystemAnnouncement entity, out int id)
		{
			entity = new SystemAnnouncement();
			entity.IsVisible = model.IsVisible;
			entity.PublishDate = model.PublishDate;
			entity.Announcement = model.Announcement;
            entity.CreatedTime = DateTime.UtcNow;
			entity.CreatedUser = CurrentUser;

			entity = this.repository.Insert(entity);
			id = entity.Id;

            AccountController.ReloadSystemAnnouncements();

			return entity;
		}

		protected override void DoUpdate(SystemAnnouncementModel model, int id, SystemAnnouncement entity)
		{
            entity.CreatedTime = DateTime.UtcNow;
            entity.CreatedUser = CurrentUser;
            //if (!CurrentUser.SystemAnnouncements.Any(p => p.Id == id))
            //    return;

			this.repository.Update(entity);

            AccountController.ReloadSystemAnnouncements();
        }

		protected override void DoRemove(int id, SystemAnnouncement entity)
		{
            //if (!CurrentUser.SystemAnnouncements.Any(p => p.Id == id))
            //    return;

			this.repository.Delete(entity);

            AccountController.ReloadSystemAnnouncements();
		}

		protected override void DoRemove(List<int> ids, List<SystemAnnouncement> entities)
		{
            //if (!CurrentUser.SystemAnnouncements.Any(p => ids.Contains(p.Id)))
            //    return;

			this.repository.Delete(p => ids.Contains(p.Id));

            AccountController.ReloadSystemAnnouncements();
		}

	}
}
