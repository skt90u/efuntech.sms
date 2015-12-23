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
	public class UserNotInSharedGroupController : CrudApiController<UserNotInSharedGroupCriteriaModel, ApplicationUserModel, ApplicationUser, string>
	{
		public UserNotInSharedGroupController(IUnitOfWork unitOfWork, ILogService logService)
			: base(unitOfWork, logService)
		{
		}

		protected override IQueryable<ApplicationUser> DoGetList(UserNotInSharedGroupCriteriaModel criteria)
		{
            var predicate = PredicateBuilder.True<ApplicationUser>();
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

            // �ư��w�g�bSharedGroup�������ϥΪ�ID
            
            var department = this.unitOfWork.Repository<Department>().Get(p => p.Id == criteria.DepartmentId);
            if (department != null)
            {
                // �����ɦ��s�ժ��Ҧ��ϥΪ�ID
                var userIds = this.unitOfWork.Repository<SharedGroupContact>().GetMany(p => p.GroupId == criteria.GroupId).Select(p => p.ShareToUserId);

                var result = department.Users.Where(p => !userIds.Contains(p.Id) && p.Id != CurrentUserId)
                            .AsQueryable()
                            .AsExpandable()
                            .Where(predicate)
                            .OrderByDescending(p => p.Id);

                return result;
            }
            else
            {
                // 20151029 Norman, ��ʿ�J���ɨϥΪ̡A���\���ɵ��Ҧ���L�t�ΨϥΪ�

                // ��ʿ�J�ϥΪ̮ɡAClient�ݷ|�ǰe DepartmentId = -1�A�d������w�����A�N��n�Ҧ��ϥΪ�
                
                var result = this.unitOfWork.Repository<ApplicationUser>().GetMany(p => p.Id != CurrentUserId)
                    .AsExpandable()
                    .Where(predicate)
                    .OrderByDescending(p => p.Id);

                return result;
            }
		}

		protected override ApplicationUser DoCreate(ApplicationUserModel model, ApplicationUser entity, out string id)
		{
            if (!this.unitOfWork.Repository<SharedGroupContact>().Any(p => p.GroupId == model.SharedGroupId && p.ShareToUserId == model.Id))
            {
                SharedGroupContact sharedGroupContact = new SharedGroupContact();
                //sharedGroupContact.GroupId = model.SharedGroupId;
                sharedGroupContact.GroupId = model.SharedGroupId.Value;
                sharedGroupContact.ShareToUserId = model.Id;
                this.unitOfWork.Repository<SharedGroupContact>().Insert(sharedGroupContact);
            }

            id = model.Id;

            return entity;
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
