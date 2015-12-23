using EFunTech.Sms.Portal.Models;
using EFunTech.Sms.Schema;
using System.Linq;
using EFunTech.Sms.Portal.Controllers.Common;
using EFunTech.Sms.Portal.Models.Common;
using JUtilSharp.Database;

using System.Collections.Generic;
using LinqKit;
using EFunTech.Sms.Portal.Models.Criteria;
using System;
using System.Data.Entity.Core.Objects;
using System.Web.Security;

namespace EFunTech.Sms.Portal.Controllers
{
    public class AllContactController : CrudApiController<ContactCriteriaModel, ContactModel, Contact, int>
    {
        public AllContactController(IUnitOfWork unitOfWork, ILogService logService) 
            : base(unitOfWork, logService) 
        { }

        protected override IQueryable<Contact> DoGetList(ContactCriteriaModel criteria)
        {
            var predicate = PredicateBuilder.True<Contact>();

            var searchText = criteria.SearchText;
            var groupIds = criteria.GroupIds;
            var sharedGroupIds = criteria.SharedGroupIds;
            
            if (!string.IsNullOrEmpty(searchText))
            {
                var innerPredicate = PredicateBuilder.False<Contact>();

                innerPredicate = innerPredicate.Or(p => !string.IsNullOrEmpty(p.Name) && p.Name.Contains(searchText));
                innerPredicate = innerPredicate.Or(p => !string.IsNullOrEmpty(p.Mobile) && p.Mobile.Contains(searchText));
                innerPredicate = innerPredicate.Or(p => !string.IsNullOrEmpty(p.Email) && p.Email.Contains(searchText));
                innerPredicate = innerPredicate.Or(p => !string.IsNullOrEmpty(p.Groups) && p.Groups.Contains(searchText));

                predicate = predicate.And(innerPredicate);
            }

            // 如果 groupIds 以及 sharedGroupIds 為空，將搜尋目前使用者建立的所有聯絡人
            if (string.IsNullOrEmpty(groupIds) &&
                string.IsNullOrEmpty(sharedGroupIds))
            {
                predicate = predicate.And(p => p.CreatedUserId == CurrentUserId);

                var result = this.repository.DbSet
                                .AsExpandable()
                                .Where(predicate)
                                .OrderByDescending(p => p.Id);

                return result;
            }
            // 否則 根據輸入的 groupIds 以及 sharedGroupIds 查詢聯絡人
            else
            {
                IQueryable<Contact> result = this.unitOfWork.Repository<Contact>().GetMany(p => false); // 使用 Enumerable.Empty<Contact>().AsQueryable(); 在 Union 會出錯

                // 如果 groupIds 不為空，查詢目前使用者建立的所有聯絡人，且群組ID包含在 groupIds 之中
                //if (!string.IsNullOrEmpty(groupIds) && groupIds != "-1")
                if (!string.IsNullOrEmpty(groupIds))
                {
                    var arrGroupIds = groupIds.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).Select(p => Convert.ToInt32(p));

                    var contacts = this.unitOfWork.Repository<GroupContact>().DbSet.Where(p => arrGroupIds.Contains(p.GroupId)).Select(p => p.Contact);

                    result = result.Union(contacts);
                }

                // 如果 criteria.SharedGroupIds 不為空，查詢SharedGroupContact.ShareToUser 為目前使用者建立的所有聯絡人，且SharedGroupContact.GroupId包含在 criteria.GroupIds 之中
                //if (!string.IsNullOrEmpty(criteria.SharedGroupIds) && criteria.SharedGroupIds != "-1")
                if (!string.IsNullOrEmpty(sharedGroupIds))
                {
                    var arrSharedGroupIds = sharedGroupIds.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).Select(p => Convert.ToInt32(p));
                    var arrGroupIds = this.unitOfWork.Repository<SharedGroupContact>().DbSet.Where(p => arrSharedGroupIds.Contains(p.GroupId)).Select(p => p.GroupId);
                    var contacts = this.unitOfWork.Repository<GroupContact>().DbSet.Where(p => arrGroupIds.Contains(p.GroupId)).Select(p => p.Contact);
                    result = result.Union(contacts);
                }

                return result.AsExpandable()
                             .Where(predicate)
                             .OrderByDescending(p => p.Id);
            }
        }

        protected override Contact DoCreate(ContactModel model, Contact entity, out int id)
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

            entity.Groups = null;
            entity.CreatedUserId = CurrentUserId;

            entity = this.repository.Insert(entity);
            id = entity.Id;

            return entity;
        }

        protected override void DoUpdate(ContactModel model, int id, Contact entity)
        {
            entity.E164Mobile = MobileUtil.GetE164PhoneNumber(model.Mobile);
            entity.Region = MobileUtil.GetRegionName(model.Mobile);
            
            this.repository.Update(entity);
        }

        protected override void DoRemove(int id)
        {
            this.unitOfWork.Repository<GroupContact>().Delete(p => p.ContactId == id);
            this.repository.Delete(p => p.Id == id);
        }

        protected override void DoRemove(int[] ids)
        {
            this.unitOfWork.Repository<GroupContact>().Delete(p => ids.Contains(p.ContactId));
            this.repository.Delete(p => ids.Contains(p.Id));
        }

        protected override IEnumerable<ContactModel> ConvertModel(IEnumerable<ContactModel> models)
        {
            var groupContacts = this.unitOfWork.Repository<GroupContact>().DbSet
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