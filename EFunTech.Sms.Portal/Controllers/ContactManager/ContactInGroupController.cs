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

namespace EFunTech.Sms.Portal.Controllers
{
	public class ContactInGroupController : CrudApiController<ContactInGroupCriteriaModel, ContactModel, Contact, int>
	{
		public ContactInGroupController(IUnitOfWork unitOfWork, ILogService logService)
			: base(unitOfWork, logService)
		{
		}

		protected override IOrderedQueryable<Contact> DoGetList(ContactInGroupCriteriaModel criteria)
		{
            IQueryable<Contact> result = CurrentUser.Contacts.AsQueryable();

            result = result.Where(p => p.GroupContacts.Any(pp => pp.GroupId == criteria.GroupId));// 只包含指定群組
            
			var predicate = PredicateBuilder.True<Contact>();
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
			result = result.AsExpandable().Where(predicate);

			return result.OrderByDescending(p => p.Id);
		}

		protected override Contact DoGet(int id)
		{
			return CurrentUser.Contacts.Where(p => p.Id == id).FirstOrDefault();
		}

		protected override Contact DoCreate(ContactModel model, Contact entity, out int id)
		{
            throw new NotImplementedException();
		}

        /// <summary>
        /// 將聯絡人由指定群組中移除
        /// </summary>
		protected override void DoUpdate(ContactModel model, int id, Contact entity)
		{
            var groupContactRepository = this.unitOfWork.Repository<GroupContact>();

            // 將聯絡人由指定群組中移除
            groupContactRepository.Delete(p => p.ContactId == model.Id && p.GroupId == model.RemoveFromGroupId);

            // 更新群組快取
            entity = this.repository.GetById(model.Id);
            if (entity != null)
            {
                /*
                 * Exceptions: ['一個或多個實體的驗證失敗。如需詳細資料，請參閱 'EntityValidationErrors' 屬性。 The validation errors are: CreatedUser 欄位是必要項。'], StackTrace:    於 JUtilSharp.Database.Repository`1.SaveChanges() 於 c:\Project\efuntech.sms\JUtilSharp\Database\Repository.cs: 行 124
                 *    於 JUtilSharp.Database.Repository`1.Update(TEntity entity) 於 c:\Project\efuntech.sms\JUtilSharp\Database\Repository.cs: 行 80
                 *    於 EFunTech.Sms.Portal.Controllers.ContactInGroupController.DoUpdate(ContactModel model, Int32 id, Contact entity) 於 c:\Project\efuntech.sms\EFunTech.Sms.Portal\Controllers\ContactInGroupController.cs: 行 78
                 *    於 EFunTech.Sms.Portal.Controllers.Common.CrudApiController`4.Update(TIdentity id, TModel model) 於 c:\Project\efuntech.sms\EFunTech.Sms.Portal\Controllers\Common\CrudApiController.cs: 行 196
                 *    
                 *  發生上面錯誤，不知道要怎麼解決，但是加上這一行就好了
                 */
                entity.CreatedUser = CurrentUser;

                entity.Groups = string.Join(",", groupContactRepository.GetMany(p => p.ContactId == model.Id).Select(p => p.Group.Name));
                this.repository.Update(entity);
            }
		}

        /// </summary>
		protected override void DoRemove(int id, Contact entity)
		{
            throw new NotImplementedException();
		}

		protected override void DoRemove(List<int> ids, List<Contact> entities)
		{
            throw new NotImplementedException();
		}

	}
}
