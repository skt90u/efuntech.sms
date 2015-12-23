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
	public class DepartmentUserController : CrudApiController<DepartmentUserCriteriaModel, ApplicationUserModel, ApplicationUser, string>
	{
        public DepartmentUserController(IUnitOfWork unitOfWork, ILogService logService)
			: base(unitOfWork, logService)
		{
		}

		protected override IQueryable<ApplicationUser> DoGetList(DepartmentUserCriteriaModel criteria)
		{
            IQueryable<ApplicationUser> result = (criteria.DepartmentId == -1)
                ? this.apiControllerHelper.GetDescendingUsersAndUser(CurrentUser).AsQueryable()
                : this.repository.GetMany(p => p.Department != null && p.Department.Id == criteria.DepartmentId);

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
			result = result.AsExpandable().Where(predicate);

			return result.OrderByDescending(p => p.Id);
		}

		protected override ApplicationUser DoGet(string id)
		{
            throw new NotImplementedException();
		}

		protected override ApplicationUser DoCreate(ApplicationUserModel model, ApplicationUser entity, out string id)
		{
            throw new NotImplementedException();
		}

		protected override void DoUpdate(ApplicationUserModel model, string id, ApplicationUser entity)
		{
            throw new NotImplementedException();
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
