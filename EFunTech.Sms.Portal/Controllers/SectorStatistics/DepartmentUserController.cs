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

namespace EFunTech.Sms.Portal.Controllers
{
	public class DepartmentUserController : AsyncCrudApiController<DepartmentUserCriteriaModel, ApplicationUserModel, ApplicationUser, string>
	{
        protected ApiControllerHelper apiControllerHelper;

        public DepartmentUserController(DbContext context, ILogService logService)
            : base(context, logService)
        {
            this.apiControllerHelper = new ApiControllerHelper(context, logService);
        }

		protected override IQueryable<ApplicationUser> DoGetList(DepartmentUserCriteriaModel criteria)
		{
			var predicate = PredicateBuilder.True<ApplicationUser>();

			var searchText = criteria.SearchText;
			if (!string.IsNullOrEmpty(searchText))
			{
				var innerPredicate = PredicateBuilder.False<ApplicationUser>();

				innerPredicate = innerPredicate.Or(p => !string.IsNullOrEmpty(p.FullName) && p.FullName.Contains(searchText));
				//innerPredicate = innerPredicate.Or(p => !string.IsNullOrEmpty(p.ContactPhone) && p.ContactPhone.Contains(searchText));
                //innerPredicate = innerPredicate.Or(p => !string.IsNullOrEmpty(p.ContactPhoneExt) && p.ContactPhoneExt.Contains(searchText));
                //innerPredicate = innerPredicate.Or(p => !string.IsNullOrEmpty(p.AddressCountry) && p.AddressCountry.Contains(searchText));
                //innerPredicate = innerPredicate.Or(p => !string.IsNullOrEmpty(p.AddressArea) && p.AddressArea.Contains(searchText));
                ////innerPredicate = innerPredicate.Or(p => !string.IsNullOrEmpty(p.AddressZip) && p.AddressZip.Contains(searchText));
                //innerPredicate = innerPredicate.Or(p => !string.IsNullOrEmpty(p.AddressStreet) && p.AddressStreet.Contains(searchText));
				innerPredicate = innerPredicate.Or(p => !string.IsNullOrEmpty(p.EmployeeNo) && p.EmployeeNo.Contains(searchText));
                //innerPredicate = innerPredicate.Or(p => !string.IsNullOrEmpty(p.Email) && p.Email.Contains(searchText));
                //innerPredicate = innerPredicate.Or(p => !string.IsNullOrEmpty(p.PasswordHash) && p.PasswordHash.Contains(searchText));
                //innerPredicate = innerPredicate.Or(p => !string.IsNullOrEmpty(p.SecurityStamp) && p.SecurityStamp.Contains(searchText));
                //innerPredicate = innerPredicate.Or(p => !string.IsNullOrEmpty(p.PhoneNumber) && p.PhoneNumber.Contains(searchText));
                //innerPredicate = innerPredicate.Or(p => !string.IsNullOrEmpty(p.Id) && p.Id.Contains(searchText));
				innerPredicate = innerPredicate.Or(p => !string.IsNullOrEmpty(p.UserName) && p.UserName.Contains(searchText));

				predicate = predicate.And(innerPredicate);
			}

            if (criteria.DepartmentId == -1)
            {
                return this.apiControllerHelper.GetDescendingUsersAndUser(CurrentUser)
                    .AsQueryable()
                    .AsExpandable()
                    .Where(predicate)
                    .OrderByDescending(p => p.Id);
            }
            else
            {
                return context.Set<ApplicationUser>()
                    .Where(p => p.Department != null && p.Department.Id == criteria.DepartmentId)
                    .AsExpandable()
                    .Where(predicate)
                    .OrderByDescending(p => p.Id);
            }
		}

		

	}
}
