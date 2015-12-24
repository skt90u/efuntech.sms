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
using System.Data.Entity;
using System.Threading.Tasks;

namespace EFunTech.Sms.Portal.Controllers
{
	public class UserInSharedGroupController : AsyncCrudApiController<UserInSharedGroupCriteriaModel, ApplicationUserModel, ApplicationUser, string>
	{
        public UserInSharedGroupController(DbContext context, ILogService logService)
			: base(context, logService)
		{
        }

        protected override IQueryable<ApplicationUser> DoGetList(UserInSharedGroupCriteriaModel criteria)
		{
            var userIds = context.Set<SharedGroupContact>()
                            .Where(p => p.GroupId == criteria.GroupId)
                            .Select(p => p.ShareToUserId);
            
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

            var result = context.Set<ApplicationUser>()
                            .AsExpandable()
                            .Where(predicate)
                            .OrderByDescending(p => p.Id);
            
            return result;
		}

        public override async Task<ApplicationUser> DoGet(string id)
        {
            var result = await context.Set<SharedGroupContact>().FirstOrDefaultAsync(p => p.ShareToUserId == id);

            return result != null ? result.ShareToUser : null;
        }

		protected override async Task<int> DoUpdate(ApplicationUserModel model, string id, ApplicationUser entity)
		{
            SharedGroupContact sharedGroupContact = await context.Set<SharedGroupContact>().FirstOrDefaultAsync(p => p.GroupId == model.SharedGroupId && p.ShareToUserId == model.Id);
            if (sharedGroupContact != null)
                return await context.DeleteAsync(sharedGroupContact);
            else
                return 0;
		}

		

	}
}
