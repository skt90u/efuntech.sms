using EFunTech.Sms.Portal.Models;
using EFunTech.Sms.Schema;
using System.Linq;
using EFunTech.Sms.Portal.Controllers.Common;
using JUtilSharp.Database;

using System.Collections.Generic;
using LinqKit;
using System;
using EFunTech.Sms.Portal.Models.Criteria;
using EFunTech.Sms.Core;


namespace EFunTech.Sms.Portal.Controllers
{
    public class UploadedMessageReceiverController : CrudApiController<UploadedMessageReceiverCriteriaModel, UploadedMessageReceiverModel, UploadedMessageReceiver, int>
    {
        public UploadedMessageReceiverController(IUnitOfWork unitOfWork, ILogService logService)
            : base(unitOfWork, logService)
        {
        }

        protected override IQueryable<UploadedMessageReceiver> DoGetList(UploadedMessageReceiverCriteriaModel criteria)
        {
            var predicate = PredicateBuilder.True<UploadedMessageReceiver>();
            predicate = predicate.And(p => p.UploadedSessionId == criteria.UploadedSessionId);

            var searchText = criteria.SearchText;
            if (!string.IsNullOrEmpty(searchText))
            {
                var innerPredicate = PredicateBuilder.False<UploadedMessageReceiver>();

                innerPredicate = innerPredicate.Or(p => !string.IsNullOrEmpty(p.Name) && p.Name.Contains(searchText));
                innerPredicate = innerPredicate.Or(p => !string.IsNullOrEmpty(p.Mobile) && p.Mobile.Contains(searchText));
                innerPredicate = innerPredicate.Or(p => !string.IsNullOrEmpty(p.Email) && p.Email.Contains(searchText));

                predicate = predicate.And(innerPredicate);
            }

            // handle DataValidCondition
            {
                var innerPredicate = PredicateBuilder.False<UploadedMessageReceiver>();

                if (criteria.DataValidCondition.HasFlag(DataValidCondition.Valid))
                    innerPredicate = innerPredicate.Or(p => p.IsValid == true);

                if (criteria.DataValidCondition.HasFlag(DataValidCondition.Invalid))
                    innerPredicate = innerPredicate.Or(p => p.IsValid == false);

                predicate = predicate.And(innerPredicate);
            }

            var result = this.repository.DbSet
                             .AsExpandable()
                             .Where(predicate)
                             .OrderBy(p => p.RowNo);

            return result;
        }

        
        protected override UploadedMessageReceiver DoCreate(UploadedMessageReceiverModel model, UploadedMessageReceiver entity, out int id)
        {
            entity = new UploadedMessageReceiver();
            entity.RowNo = this.repository.Count(p => p.UploadedSessionId == model.UploadedSessionId) + 1;
            entity.Name = model.Name;
            entity.Mobile = model.Mobile;
            entity.E164Mobile = MobileUtil.GetE164PhoneNumber(model.Mobile);
            entity.Region = MobileUtil.GetRegionName(model.Mobile);
            entity.Email = model.Email;
            entity.SendTime = Converter.ToUniversalTime(model.SendTimeString, Converter.yyyyMMddHHmm, ClientTimezoneOffset);
            entity.ClientTimezoneOffset = ClientTimezoneOffset;

            //entity.IsValid = model.IsValid; // 經由 this.validationHelper.Validate
            //entity.InvalidReason = model.InvalidReason; // 經由 this.validationHelper.Validate
            entity.UseParam = model.UseParam;
            entity.Param1 = model.Param1 ?? string.Empty;
            entity.Param2 = model.Param2 ?? string.Empty;
            entity.Param3 = model.Param3 ?? string.Empty;
            entity.Param4 = model.Param4 ?? string.Empty;
            entity.Param5 = model.Param5 ?? string.Empty;
            entity.CreatedUserId = CurrentUserId;
            entity.CreatedTime = DateTime.UtcNow;
            entity.UploadedFile = null;
            entity.UploadedSessionId = model.UploadedSessionId;

            var error = string.Empty;
            var isValid = this.validationService.Validate(entity, out error);
            // 目前就算驗證不過也沒關係，仍然可以存檔
            entity = this.repository.Insert(entity);
            id = entity.Id;

            if (isValid)
            {
                return entity;
            }
            else
            {
                throw new Exception(error);
            }
        }

        protected override void DoUpdate(UploadedMessageReceiverModel model, int id, UploadedMessageReceiver entity)
        {
            entity.SendTime = Converter.ToUniversalTime(model.SendTimeString, Converter.yyyyMMddHHmm, ClientTimezoneOffset);
            entity.ClientTimezoneOffset = ClientTimezoneOffset;

            entity.E164Mobile = MobileUtil.GetE164PhoneNumber(model.Mobile);
            entity.Region = MobileUtil.GetRegionName(model.Mobile);
            entity.CreatedUserId = entity.CreatedUserId;

            var error = string.Empty;
            var isValid = this.validationService.Validate(entity, out error);
           
            // 目前就算驗證不過也沒關係，仍然可以存檔
            this.repository.Update(entity);

            if (isValid)
            {
            }
            else
            {
                throw new Exception(error);
            }
        }

        protected override void DoRemove(int id)
        {
            this.repository.Delete(p=> p.Id == id);
        }

        protected override void DoRemove(int[] ids)
        {
            this.repository.Delete(p => ids.Contains(p.Id));
        }

    }
}
