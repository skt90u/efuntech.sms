using EFunTech.Sms.Portal.Models;
using EFunTech.Sms.Schema;
using System.Linq;
using EFunTech.Sms.Portal.Controllers.Common;
using EFunTech.Sms.Portal.Models.Common;
using JUtilSharp.Database;

using System.Collections.Generic;
using System;
using LinqKit;

namespace EFunTech.Sms.Portal.Controllers
{
    public class BlacklistController : CrudApiController<SearchTextCriteriaModel, BlacklistModel, Blacklist, int>
    {
        public BlacklistController(IUnitOfWork unitOfWork, ILogService logService)
            : base(unitOfWork, logService)
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

            var result = this.repository.DbSet
                            .AsExpandable()
                            .Where(predicate)
                            .OrderByDescending(p => p.Id);

            return result;
        }

        protected override Blacklist DoCreate(BlacklistModel model, Blacklist entity, out int id)
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

            entity = this.repository.Insert(entity);
            id = entity.Id;

            return entity;
        }

        protected override void DoUpdate(BlacklistModel model, int id, Blacklist entity)
        {
            entity.E164Mobile = MobileUtil.GetE164PhoneNumber(model.Mobile);
            entity.Region = MobileUtil.GetRegionName(model.Mobile);
            entity.UpdatedTime = DateTime.UtcNow;
            entity.UpdatedUserName = CurrentUserName;

            this.repository.Update(entity);
        }

        protected override void DoRemove(int id)
        {
            this.repository.Delete(p => p.Id == id);
        }

        protected override void DoRemove(int[] ids)
        {
            this.repository.Delete(p => ids.Contains(p.Id));
        }

        protected override IEnumerable<BlacklistModel> ConvertModel(IEnumerable<BlacklistModel> models)
        {
            int index = 0;
            foreach (var model in models)
            {
                model.DecoratedValue_SequenceNo = (index + 1).ToString();
                model.DecoratedValue_Enabled = model.Enabled ? "¶}±Ò" : "Ãö³¬";
                model.DecoratedValue_UpdatedTime = model.UpdatedTime.ToString("yyyy/MM/dd HH:mm");

                index++;
            }
            return models;
        }
    }
}
