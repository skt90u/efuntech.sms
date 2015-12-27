using EFunTech.Sms.Portal.Models;
using EFunTech.Sms.Schema;
using System.Linq;
using EFunTech.Sms.Portal.Controllers.Common;
using LinqKit;
using EFunTech.Sms.Portal.Models.Criteria;
using System.Data.Entity;
using System.Threading.Tasks;

namespace EFunTech.Sms.Portal.Controllers
{
    public class UserNotInSharedGroupController : CrudApiController<UserNotInSharedGroupCriteriaModel, ApplicationUserModel, ApplicationUser, string>
	{
        public UserNotInSharedGroupController(DbContext context, ILogService logService)
			: base(context, logService)
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
            
            var department = context.Set<Department>().FirstOrDefault(p => p.Id == criteria.DepartmentId);
            if (department != null)
            {
                // �����ɦ��s�ժ��Ҧ��ϥΪ�ID
                var userIds = context.Set<SharedGroupContact>().Where(p => p.GroupId == criteria.GroupId).Select(p => p.ShareToUserId);

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

                // result = this.unitOfWork.Repository<ApplicationUser>().GetMany(p => p.Id != CurrentUser.Id);

                var result = context.Set<ApplicationUser>()
                    .Where(p => p.Id != CurrentUserId)
                    .AsExpandable()
                    .Where(predicate)
                    .OrderByDescending(p => p.Id);

                return result;
            }
		}

        protected override async Task<ApplicationUser> DoCreate(ApplicationUserModel model, ApplicationUser entity)
		{
            if (!context.Set<SharedGroupContact>().Any(p => p.GroupId == model.SharedGroupId && p.ShareToUserId == model.Id))
            {
                SharedGroupContact sharedGroupContact = new SharedGroupContact();
                sharedGroupContact.GroupId = model.SharedGroupId.Value;
                sharedGroupContact.ShareToUserId = model.Id;
                sharedGroupContact = await context.InsertAsync(sharedGroupContact);
            }

            return entity;
        }
	}
}
