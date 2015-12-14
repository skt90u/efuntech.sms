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
        public AllContactController(IUnitOfWork unitOfWork, ILogService logService) : base(unitOfWork, logService) { }

        protected override IOrderedQueryable<Contact> DoGetList(ContactCriteriaModel criteria)
        {
            IQueryable<Contact> result = this.unitOfWork.Repository<Contact>().GetMany(p => false); // 使用 Enumerable.Empty<Contact>().AsQueryable(); 在 Union 會出錯
            // IQueryable<Contact> result = Enumerable.Empty<Contact>().AsQueryable();

            // 如果 criteria.GroupIds 以及 criteria.SharedGroupIds 為空，將搜尋目前使用者建立的所有聯絡人
            if (string.IsNullOrEmpty(criteria.GroupIds) && string.IsNullOrEmpty(criteria.SharedGroupIds))
            {
                result = CurrentUser.Contacts.AsQueryable();
            }
            // 否則 根據輸入的 criteria.GroupIds 以及 criteria.SharedGroupIds 查詢聯絡人
            else
            {
                // 如果 criteria.GroupIds 不為空，查詢目前使用者建立的所有聯絡人，且群組ID包含在 criteria.GroupIds 之中
                //if (!string.IsNullOrEmpty(criteria.GroupIds) && criteria.GroupIds != "-1")
                if (!string.IsNullOrEmpty(criteria.GroupIds))
                {
                    var groupIds = criteria.GroupIds.Split(new string[] {","}, StringSplitOptions.RemoveEmptyEntries).Select(p => Convert.ToInt32(p));
                    var contacts = this.unitOfWork.Repository<GroupContact>().GetMany(p => groupIds.Contains(p.GroupId)).Select(p => p.Contact);
                    result = result.Union(contacts);
                }

                // 如果 criteria.SharedGroupIds 不為空，查詢SharedGroupContact.ShareToUser 為目前使用者建立的所有聯絡人，且SharedGroupContact.GroupId包含在 criteria.GroupIds 之中
                //if (!string.IsNullOrEmpty(criteria.SharedGroupIds) && criteria.SharedGroupIds != "-1")
                if (!string.IsNullOrEmpty(criteria.SharedGroupIds))
                {
                    var SharedGroupIds = criteria.SharedGroupIds.Split(new string[] {","}, StringSplitOptions.RemoveEmptyEntries).Select(p => Convert.ToInt32(p));
                    var groupIds = this.unitOfWork.Repository<SharedGroupContact>().GetMany(p => SharedGroupIds.Contains(p.GroupId)).Select(p => p.GroupId);
                    var contacts = this.unitOfWork.Repository<GroupContact>().GetMany(p => groupIds.Contains(p.GroupId)).Select(p => p.Contact);
                    result = result.Union(contacts);
                }
            }

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
            result = result.AsExpandable().Where(predicate);


            return result.OrderByDescending(p => p.Id);
        }

        protected override Contact DoGet(int id)
        {
            return CurrentUser.Contacts.Where(p => p.Id == id).FirstOrDefault();
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
            entity.CreatedUser = CurrentUser;

            entity = this.repository.Insert(entity);
            id = entity.Id;

            return entity;
        }

        protected override void DoUpdate(ContactModel model, int id, Contact entity)
        {
            if (!CurrentUser.Contacts.Any(p => p.Id == id)) return;

            entity.E164Mobile = MobileUtil.GetE164PhoneNumber(model.Mobile);
            entity.Region = MobileUtil.GetRegionName(model.Mobile);
            entity.CreatedUser = entity.CreatedUser;
            
            this.repository.Update(entity);
        }

        protected override void DoRemove(int id, Contact entity)
        {
            if (!CurrentUser.Contacts.Any(p => p.Id == id)) return;

            this.unitOfWork.Repository<GroupContact>().Delete(p => p.ContactId == id);

            this.repository.Delete(entity);
        }

        protected override void DoRemove(List<int> ids, List<Contact> entities)
        {
            if (!CurrentUser.Contacts.Any(p => ids.Contains(p.Id))) return;

            this.unitOfWork.Repository<GroupContact>().Delete(p => ids.Contains(p.ContactId));
            
            this.repository.Delete(p => ids.Contains(p.Id));
        }

        protected override IEnumerable<ContactModel> ConvertModel(IEnumerable<ContactModel> models)
        {
            // 快取目前聯絡人對應群組
            // 用以避免再ContactProfile重複查詢相同資料
            // .ForMember(dst => dst.Groups, opt => opt.MapFrom(src => string.Join(",", src.GroupContacts.Select(p => p.Group.Name))))
            // 解決方式是在 CommonContactController.cs 以及 AllContactController.cs 複寫 CrudApiController.ConvertModel 對於Groups為空的資料進行查詢並回存，而不使用AutoMapper

            var groupContactRepository = this.unitOfWork.Repository<GroupContact>();

            foreach (var model in models)
            {
                //if (string.IsNullOrEmpty(model.Groups))
                //{
                //    string Groups = string.Join(",", groupContactRepository.GetMany(p => p.ContactId == model.Id).Select(p => p.Group.Name));

                //    if (!string.IsNullOrEmpty(Groups))
                //    {
                //        model.Groups = Groups;

                //        var entity = this.repository.GetById(model.Id);
                //        entity.Groups = Groups;
                //        entity.CreatedUser = CurrentUser;
                //        this.repository.Update(entity);
                //    }
                //}

                // TODO: 如果效能沒問題，就將 entity.Groups 欄位拿掉
                model.Groups = string.Join(",", groupContactRepository.GetMany(p => p.ContactId == model.Id).Select(p => p.Group.Name));
            }

            return models;
        }
    }
}