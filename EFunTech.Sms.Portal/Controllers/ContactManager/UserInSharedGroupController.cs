using EFunTech.Sms.Portal.Models;
using EFunTech.Sms.Schema;
using System.Linq;
using EFunTech.Sms.Portal.Controllers.Common;
using EFunTech.Sms.Portal.Models.Common;
using JUtilSharp.Database;

using System.Collections.Generic;
using LinqKit;
using System;
using EFunTech.Sms.Portal.Models.Criteria;

namespace EFunTech.Sms.Portal.Controllers
{
	public class UserInSharedGroupController : CrudApiController<UserInSharedGroupCriteriaModel, ApplicationUserModel, ApplicationUser, string>
	{
		public UserInSharedGroupController(IUnitOfWork unitOfWork, ILogService logService)
			: base(unitOfWork, logService)
		{
		}

		protected override IQueryable<ApplicationUser> DoGetList(UserInSharedGroupCriteriaModel criteria)
		{
            //IQueryable<ApplicationUser> result = this.unitOfWork.Repository<SharedGroupContact>().GetMany(p => p.GroupId == criteria.GroupId).Select(p => p.ShareToUser).AsQueryable();
            var userIds = this.unitOfWork.Repository<SharedGroupContact>().DbSet.Where(p => p.GroupId == criteria.GroupId).Select(p => p.ShareToUserId);
            
            var predicate = PredicateBuilder.True<ApplicationUser>();

            if (userIds.Count() == 0)
            {
                predicate = predicate.And(p => false);
            }
            else
            {
                predicate = predicate.And(p => userIds.Contains(p.Id));
            }
            
            var searchText = criteria.SearchText;
            if (!string.IsNullOrEmpty(searchText))
            {
                var innerPredicate = PredicateBuilder.False<ApplicationUser>();

                //innerPredicate = innerPredicate.Or(p => !string.IsNullOrEmpty(p.ParentId) && p.ParentId.Contains(searchText));
                innerPredicate = innerPredicate.Or(p => !string.IsNullOrEmpty(p.FullName) && p.FullName.Contains(searchText));
                //innerPredicate = innerPredicate.Or(p => !string.IsNullOrEmpty(p.ContactPhone) && p.ContactPhone.Contains(searchText));
                //innerPredicate = innerPredicate.Or(p => !string.IsNullOrEmpty(p.ContactPhoneExt) && p.ContactPhoneExt.Contains(searchText));
                //innerPredicate = innerPredicate.Or(p => !string.IsNullOrEmpty(p.AddressCountry) && p.AddressCountry.Contains(searchText));

                //innerPredicate = innerPredicate.Or(p => !string.IsNullOrEmpty(p.AddressArea) && p.AddressArea.Contains(searchText));
                //innerPredicate = innerPredicate.Or(p => !string.IsNullOrEmpty(p.AddressZip) && p.AddressZip.Contains(searchText));
                //innerPredicate = innerPredicate.Or(p => !string.IsNullOrEmpty(p.AddressStreet) && p.AddressStreet.Contains(searchText));
                //innerPredicate = innerPredicate.Or(p => !string.IsNullOrEmpty(p.EmployeeNo) && p.EmployeeNo.Contains(searchText));
                //innerPredicate = innerPredicate.Or(p => !string.IsNullOrEmpty(p.Email) && p.Email.Contains(searchText));

                //innerPredicate = innerPredicate.Or(p => !string.IsNullOrEmpty(p.PasswordHash) && p.PasswordHash.Contains(searchText));
                //innerPredicate = innerPredicate.Or(p => !string.IsNullOrEmpty(p.SecurityStamp) && p.SecurityStamp.Contains(searchText));
                //innerPredicate = innerPredicate.Or(p => !string.IsNullOrEmpty(p.PhoneNumber) && p.PhoneNumber.Contains(searchText));
                //innerPredicate = innerPredicate.Or(p => !string.IsNullOrEmpty(p.Id) && p.Id.Contains(searchText));
                innerPredicate = innerPredicate.Or(p => !string.IsNullOrEmpty(p.UserName) && p.UserName.Contains(searchText));

                predicate = predicate.And(innerPredicate);
            }

            var result = this.repository.DbSet
                            .AsExpandable()
                            .Where(predicate)
                            .OrderByDescending(p => p.Id);
            
            return result;
		}

		protected override ApplicationUser DoGet(string id)
		{
            var result = this.unitOfWork.Repository<SharedGroupContact>().Get(p => p.ShareToUserId == id);

            return result != null ? result.ShareToUser : null;
		}

		protected override ApplicationUser DoCreate(ApplicationUserModel model, ApplicationUser entity, out string id)
		{
            throw new NotImplementedException();
		}

		protected override void DoUpdate(ApplicationUserModel model, string id, ApplicationUser entity)
		{
            SharedGroupContact sharedGroupContact = this.unitOfWork.Repository<SharedGroupContact>().Get(p => p.GroupId == model.SharedGroupId && p.ShareToUserId == model.Id);
            if(sharedGroupContact != null)
                this.unitOfWork.Repository<SharedGroupContact>().Delete(sharedGroupContact);
		}

		protected override void DoRemove(string id)
		{
            throw new NotImplementedException();
		}

        protected override void DoRemove(string[] ids)
		{
            throw new NotImplementedException();
		}

	}
}
