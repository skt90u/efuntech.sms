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
using EFunTech.Sms.Portal.Models.Criteria;
using EFunTech.Sms.Core;

namespace EFunTech.Sms.Portal.Controllers
{
    public class UploadedMessageReceiverController : AsyncCrudApiController<UploadedMessageReceiverCriteriaModel, UploadedMessageReceiverModel, UploadedMessageReceiver, int>
    {
        protected ValidationService validationService;

        public UploadedMessageReceiverController(DbContext context, ILogService logService)
            : base(context, logService)
        {
            this.validationService = new ValidationService(new UnitOfWork(context), logService);
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

            var result = context.Set<UploadedMessageReceiver>()
                             .AsExpandable()
                             .Where(predicate)
                             .OrderBy(p => p.RowNo);

            return result;
        }

        
        protected override async Task<UploadedMessageReceiver> DoCreate(UploadedMessageReceiverModel model, UploadedMessageReceiver entity)
        {
            entity = new UploadedMessageReceiver();
            entity.RowNo = context.Set<UploadedMessageReceiver>().Count(p => p.UploadedSessionId == model.UploadedSessionId) + 1;
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
            entity = await context.InsertAsync(entity);

            if (isValid)
            {
                return entity;
            }
            else
            {
                throw new Exception(error);
            }
        }

        protected override async Task DoUpdate(UploadedMessageReceiverModel model, int id, UploadedMessageReceiver entity)
        {
            entity.SendTime = Converter.ToUniversalTime(model.SendTimeString, Converter.yyyyMMddHHmm, ClientTimezoneOffset);
            entity.ClientTimezoneOffset = ClientTimezoneOffset;

            entity.E164Mobile = MobileUtil.GetE164PhoneNumber(model.Mobile);
            entity.Region = MobileUtil.GetRegionName(model.Mobile);
            entity.CreatedUserId = entity.CreatedUserId;

            var error = string.Empty;
            var isValid = this.validationService.Validate(entity, out error);
           
            // 目前就算驗證不過也沒關係，仍然可以存檔
            await context.UpdateAsync(entity);

            if (isValid)
            {
            }
            else
            {
                throw new Exception(error);
            }
        }

        protected override async Task DoRemove(int id)
        {
            await context.DeleteAsync<UploadedMessageReceiver>(p => p.Id == id);
        }

        protected override async Task DoRemove(int[] ids)
        {
            await context.DeleteAsync<UploadedMessageReceiver>(p => ids.Contains(p.Id));
        }
    }
}
