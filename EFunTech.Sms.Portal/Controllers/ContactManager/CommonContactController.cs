using EFunTech.Sms.Portal.Models;
using EFunTech.Sms.Schema;
using System.Linq;
using EFunTech.Sms.Portal.Controllers.Common;
using EFunTech.Sms.Portal.Models.Common;

using System.Collections.Generic;
using LinqKit;
using System;
using System.Data.Entity;
using System.Threading.Tasks;

namespace EFunTech.Sms.Portal.Controllers
{
    public class CommonContactController : AsyncCrudApiController<SearchTextCriteriaModel, ContactModel, Contact, int>
    {
        public CommonContactController(DbContext context, ILogService logService)
            : base(context, logService)
        {
        }

        protected override IQueryable<Contact> DoGetList(SearchTextCriteriaModel criteria)
        {
            var predicate = PredicateBuilder.True<Contact>();
            var searchText = criteria.SearchText;
            if (!string.IsNullOrEmpty(searchText))
            {
                var innerPredicate = PredicateBuilder.False<Contact>();

                innerPredicate = innerPredicate.Or(p => !string.IsNullOrEmpty(p.Name) && p.Name.Contains(searchText));
                innerPredicate = innerPredicate.Or(p => !string.IsNullOrEmpty(p.Mobile) && p.Mobile.Contains(searchText));
                innerPredicate = innerPredicate.Or(p => !string.IsNullOrEmpty(p.Email) && p.Email.Contains(searchText));
                innerPredicate = innerPredicate.Or(p => !string.IsNullOrEmpty(p.Groups) && p.Groups.Contains(searchText));

                predicate = predicate.And(innerPredicate);
            }

            predicate = predicate.And(p => p.GroupContacts.Any(pp => pp.Group.Name == Group.CommonContactGroupName));
            predicate = predicate.And(p => p.CreatedUserId == CurrentUserId);

            var result = context.Set<Contact>()
                            .AsExpandable()
                            .Where(predicate)
                            .OrderByDescending(p => p.Id);

            return result;
        }

        protected override async Task<Contact> DoCreate(ContactModel model, Contact entity)
        {
            entity = new Contact();

            entity.Name = model.Name;
            entity.Mobile = model.Mobile;
            entity.E164Mobile = MobileUtil.GetE164PhoneNumber(model.Mobile);
            entity.Region = MobileUtil.GetRegionName(model.Mobile);
            entity.HomePhoneArea = model.HomePhoneArea;

            entity.HomePhone = model.HomePhone;
            entity.CompanyPhoneArea = model.CompanyPhoneArea;
            entity.CompanyPhone = model.CompanyPhone;
            entity.CompanyPhoneExt = model.CompanyPhoneExt;
            entity.Email = model.Email;

            entity.Msn = model.Msn;
            entity.Description = model.Description;
            entity.Birthday = model.Birthday;
            entity.ImportantDay = model.ImportantDay;
            entity.Gender = model.Gender;

            entity.Groups = Group.CommonContactGroupName;
            entity.CreatedUserId = CurrentUserId;

            entity = await context.InsertAsync(entity);

            var group = context.Set<Group>()
                .Where(p => p.Name == Group.CommonContactGroupName && p.CreatedUserId == CurrentUserId)
                .FirstOrDefault();

            if (group == null)
                throw new Exception(string.Format("使用者{0}常用聯絡人群組尚未建立", CurrentUser.UserName));

            var groupContact = new GroupContact();
            groupContact.GroupId = group.Id;
            groupContact.ContactId = entity.Id;
            groupContact = await context.InsertAsync(groupContact);

            return entity;
        }

        protected override async Task<int> DoUpdate(ContactModel model, int id, Contact entity)
        {
            entity.E164Mobile = MobileUtil.GetE164PhoneNumber(model.Mobile);
            entity.Region = MobileUtil.GetRegionName(model.Mobile);

            return await context.UpdateAsync(entity);
        }

        protected override async Task<int> DoRemove(int id)
        {
            await context.DeleteAsync<GroupContact>(p => p.ContactId == id);
            return await context.DeleteAsync<Contact>(p => p.Id == id);
        }

        protected override async Task<int> DoRemove(int[] ids)
        {
            await context.DeleteAsync<GroupContact>(p => ids.Contains(p.ContactId));
            return await context.DeleteAsync<Contact>(p => ids.Contains(p.Id));
        }

        protected override IEnumerable<ContactModel> ConvertModel(IEnumerable<ContactModel> models)
        {
            var groupContacts = context.Set<GroupContact>()
                                    .Select(p => new {
                                        ContactId = p.ContactId,
                                        GroupName = p.Group.Name,
                                    }).ToList();

            foreach (var model in models)
            {
                model.Groups = string.Join(",", groupContacts.Where(p => p.ContactId == model.Id).Select(p => p.GroupName));
            }

            return models;
        }
    }
}