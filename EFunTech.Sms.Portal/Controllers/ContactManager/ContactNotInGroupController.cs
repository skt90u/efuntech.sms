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
    public class ContactNotInGroupController : CrudApiController<ContactNotInGroupCriteriaModel, ContactModel, Contact, int>
	{
        private ISystemParameters systemParameters;

        public ContactNotInGroupController(ISystemParameters systemParameters, DbContext context, ILogService logService)
			: base(context, logService)
		{
            this.systemParameters = systemParameters;
        }

        protected override IQueryable<Contact> DoGetList(ContactNotInGroupCriteriaModel criteria)
		{
            var predicate = PredicateBuilder.True<Contact>();

            predicate = predicate.And(p => p.CreatedUserId == CurrentUserId);

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

            if (systemParameters.ContactAtMostOneGroup) // �p���H�u������ܤ@�Ӹs��
            {
                // �M��S������s�ժ��H
                predicate = predicate.And(p => !p.GroupContacts.Any());
            }
            else
            {
                // �M�䤣�b���w�s�ժ��H
                predicate = predicate.And(p => !p.GroupContacts.Any(pp => pp.GroupId == criteria.GroupId));
            }

            var result = context.Set<Contact>()
                            .AsExpandable()
                            .Where(predicate)
                            .OrderByDescending(p => p.Id);

            return result;
		}

        /// <summary>
        /// �N�p���H�[�J�ܫ��w�s�դ�
        /// </summary>
        protected override async Task DoUpdate(ContactModel model, int id, Contact entity)
		{
            if (!context.Set<GroupContact>().Any(p => p.ContactId == model.Id && p.GroupId == model.JoinToGroupId))
            {
                // �N�p���H�[�J�ܫ��w�s�դ�
                var groupContact = new GroupContact();
                groupContact.GroupId = model.JoinToGroupId;
                groupContact.ContactId = entity.Id;
                groupContact = await context.InsertAsync(groupContact);

                // ��s�s�է֨�
                entity.Groups = string.Join(",", context.Set<GroupContact>().Where(p => p.ContactId == model.Id).Select(p => p.Group.Name));
                await context.UpdateAsync(entity);
            }
		}
	}
}
