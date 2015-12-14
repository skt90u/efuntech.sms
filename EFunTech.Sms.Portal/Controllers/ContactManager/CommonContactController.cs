using EFunTech.Sms.Portal.Models;
using EFunTech.Sms.Schema;
using System.Linq;
using EFunTech.Sms.Portal.Controllers.Common;
using EFunTech.Sms.Portal.Models.Common;
using JUtilSharp.Database;

using System.Collections.Generic;
using LinqKit;
using System;

namespace EFunTech.Sms.Portal.Controllers
{
    public class CommonContactController : CrudApiController<SearchTextCriteriaModel, ContactModel, Contact, int>
    {
        public CommonContactController(IUnitOfWork unitOfWork, ILogService logService) : base(unitOfWork, logService) { }

        protected override IOrderedQueryable<Contact> DoGetList(SearchTextCriteriaModel criteria)
        {
            var result = CurrentUser.Contacts.Where(p => p.GroupContacts.Any(pp => pp.Group.Name == Group.CommonContactGroupName))
                                             .AsQueryable();

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
            entity.Groups = string.Empty;
            entity.CreatedUser = CurrentUser;

            entity = this.repository.Insert(entity);
            id = entity.Id;

            var group = this.unitOfWork.Repository<Group>().Get(p => p.Name == Group.CommonContactGroupName && p.CreatedUser == CurrentUser);
            if (group == null)
                throw new Exception(string.Format("使用者{0}常用聯絡人群組尚未建立", CurrentUser.UserName));

            var groupContact = new GroupContact();
            groupContact.GroupId = group.Id;
            groupContact.ContactId = entity.Id;
            groupContact = this.unitOfWork.Repository<GroupContact>().Insert(groupContact);

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