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

        protected override IOrderedQueryable<Blacklist> DoGetList(SearchTextCriteriaModel criteria)
        {
            var result = CurrentUser.Blacklists.AsQueryable();

            var predicate = PredicateBuilder.True<Blacklist>();

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
            result = result.AsExpandable().Where(predicate);

            return result.OrderByDescending(p => p.Id);
        }

        protected override Blacklist DoGet(int id)
        {
            return CurrentUser.Blacklists.Where(p => p.Id == id).FirstOrDefault();
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
            entity.CreatedUser = CurrentUser;
            entity.UpdatedUserName = CurrentUser.UserName;

            entity = this.repository.Insert(entity);
            id = entity.Id;

            return entity;
        }

        protected override void DoUpdate(BlacklistModel model, int id, Blacklist entity)
        {
            if (!CurrentUser.Blacklists.Any(p => p.Id == id))
                return;

            entity.E164Mobile = MobileUtil.GetE164PhoneNumber(model.Mobile);
            entity.Region = MobileUtil.GetRegionName(model.Mobile);
            entity.CreatedUser = entity.CreatedUser;

            entity.UpdatedTime = DateTime.UtcNow;
            entity.UpdatedUserName = CurrentUser.UserName;

            this.repository.Update(entity);
        }

        protected override void DoRemove(int id, Blacklist entity)
        {
            if (!CurrentUser.Blacklists.Any(p => p.Id == id))
                return;

            this.repository.Delete(entity);
        }

        protected override void DoRemove(List<int> ids, List<Blacklist> entities)
        {
            if (!CurrentUser.Blacklists.Any(p => ids.Contains(p.Id)))
                return;

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
