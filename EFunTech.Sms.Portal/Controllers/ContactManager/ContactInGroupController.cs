using EFunTech.Sms.Portal.Models;
using EFunTech.Sms.Schema;
using System.Linq;
using EFunTech.Sms.Portal.Controllers.Common;
using LinqKit;
using EFunTech.Sms.Portal.Models.Criteria;
using System.Data.Entity;
using System.Threading.Tasks;

namespace EFunTech.Sms.Portal.Controllers
{
    public class ContactInGroupController : CrudApiController<ContactInGroupCriteriaModel, ContactModel, Contact, int>
	{
        public ContactInGroupController(DbContext context, ILogService logService)
			: base(context, logService)
		{
        }

        protected override IQueryable<Contact> DoGetList(ContactInGroupCriteriaModel criteria)
		{
			var predicate = PredicateBuilder.True<Contact>();

            predicate = predicate.And(p => p.CreatedUserId == CurrentUserId);
            predicate = predicate.And(p => p.GroupContacts.Any(pp => pp.GroupId == criteria.GroupId));// 只包含指定群組

			var searchText = criteria.SearchText;
			if (!string.IsNullOrEmpty(searchText))
			{
                var innerPredicate = PredicateBuilder.False<Contact>();

                innerPredicate = innerPredicate.Or(p => !string.IsNullOrEmpty(p.Name) && p.Name.Contains(searchText));
                innerPredicate = innerPredicate.Or(p => !string.IsNullOrEmpty(p.Mobile) && p.Mobile.Contains(searchText));
                //innerPredicate = innerPredicate.Or(p => !string.IsNullOrEmpty(p.HomePhoneArea) && p.HomePhoneArea.Contains(searchText));
                //innerPredicate = innerPredicate.Or(p => !string.IsNullOrEmpty(p.HomePhone) && p.HomePhone.Contains(searchText));
                //innerPredicate = innerPredicate.Or(p => !string.IsNullOrEmpty(p.CompanyPhoneArea) && p.CompanyPhoneArea.Contains(searchText));

                //innerPredicate = innerPredicate.Or(p => !string.IsNullOrEmpty(p.CompanyPhone) && p.CompanyPhone.Contains(searchText));
                //innerPredicate = innerPredicate.Or(p => !string.IsNullOrEmpty(p.CompanyPhoneExt) && p.CompanyPhoneExt.Contains(searchText));
                //innerPredicate = innerPredicate.Or(p => !string.IsNullOrEmpty(p.Email) && p.Email.Contains(searchText));
                //innerPredicate = innerPredicate.Or(p => !string.IsNullOrEmpty(p.Msn) && p.Msn.Contains(searchText));
                //innerPredicate = innerPredicate.Or(p => !string.IsNullOrEmpty(p.Description) && p.Description.Contains(searchText));

                //innerPredicate = innerPredicate.Or(p => !string.IsNullOrEmpty(p.Birthday) && p.Birthday.Contains(searchText));
                //innerPredicate = innerPredicate.Or(p => !string.IsNullOrEmpty(p.ImportantDay) && p.ImportantDay.Contains(searchText));
                //innerPredicate = innerPredicate.Or(p => !string.IsNullOrEmpty(p.Groups) && p.Groups.Contains(searchText));

                predicate = predicate.And(innerPredicate);
			}

            var result = context.Set<Contact>()
                            .AsExpandable()
                            .Where(predicate)
                            .OrderByDescending(p => p.Id);

			return result;
		}

        /// <summary>
        /// 將聯絡人由指定群組中移除
        /// </summary>
        protected override async Task DoUpdate(ContactModel model, int id, Contact entity)
		{
            // 將聯絡人由指定群組中移除
            await context.DeleteAsync<GroupContact>(p => p.ContactId == model.Id && p.GroupId == model.RemoveFromGroupId);

            // 更新群組快取
            entity.Groups = string.Join(",", context.Set<GroupContact>().Where(p => p.ContactId == model.Id).Select(p => p.Group.Name));
            await context.UpdateAsync(entity);
        }

	}
}
