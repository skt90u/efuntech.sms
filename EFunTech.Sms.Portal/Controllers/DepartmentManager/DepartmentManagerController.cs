using EFunTech.Sms.Portal.Models;
using EFunTech.Sms.Schema;
using System.Linq;
using EFunTech.Sms.Portal.Controllers.Common;
using EFunTech.Sms.Portal.Models.Common;
using JUtilSharp.Database;

using System.Collections.Generic;
using System;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using EFunTech.Sms.Portal.Models.Criteria;
using LinqKit;

namespace EFunTech.Sms.Portal.Controllers
{
    public class DepartmentManagerController : CrudApiController<DepartmentManagerCriteriaModel, ApplicationUserModel, ApplicationUser, string>
    {
        public DepartmentManagerController(ISystemParameters systemParameters, IUnitOfWork unitOfWork, ILogService logService)
            : base(unitOfWork, logService)
        {
        }

        // 只能管理子帳號
        //protected override IOrderedQueryable<ApplicationUser> DoGetList(DepartmentManagerCriteriaModel criteria)
        //{
        //    // 尋找目前使用者以及目前使用者的子帳號
        //    var result = this.repository.GetMany(p => p.ParentId == CurrentUser.Id || p.Id == CurrentUser.Id).AsQueryable();

        //    var predicate = PredicateBuilder.True<ApplicationUser>();
        //    var fullName = criteria.FullName;
        //    var userName = criteria.UserName;
        //    if (!string.IsNullOrEmpty(fullName))
        //    {
        //        predicate = predicate.And(p => p.FullName.Contains(fullName));
        //    }
        //    if (!string.IsNullOrEmpty(userName))
        //    {
        //        predicate = predicate.And(p => p.UserName.Contains(userName));
        //    }
        //    result = result.AsExpandable().Where(predicate);

        //    return result.OrderByDescending(p => p.Id);
        //}

        /// <summary>
        /// 查詢子帳號以及子帳號所建立的帳號
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns></returns>
        protected override IOrderedQueryable<ApplicationUser> DoGetList(DepartmentManagerCriteriaModel criteria)
        {
            // 尋找目前使用者以及目前使用者的子帳號或孫帳號
            List<ApplicationUser> users = this.apiControllerHelper.GetDescendingUsersAndUser(CurrentUser);


            // 尋找目前使用者以及目前使用者的子帳號
            var result = users.AsQueryable();

            var predicate = PredicateBuilder.True<ApplicationUser>();
            var fullName = criteria.FullName;
            var userName = criteria.UserName;
            
            if (!string.IsNullOrEmpty(fullName))
            {
                predicate = predicate.And(p => p.FullName.Contains(fullName));
            }
            
            if (!string.IsNullOrEmpty(userName))
            {
                predicate = predicate.And(p => p.UserName.Contains(userName));
            }
            
            result = result.AsExpandable().Where(predicate);

            // 由大到小排列
            return result.OrderByDescending(p => p.Level).ThenBy(p => p.Id);
        }

        protected override ApplicationUser DoGet(string id)
        {
            return this.unitOfWork.Repository<ApplicationUser>().GetById(id);
        }

        /// <summary>
        /// 建立子帳號
        /// </summary>
        /// <param name="model"></param>
        /// <param name="entity"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        protected override ApplicationUser DoCreate(ApplicationUserModel model, ApplicationUser entity, out string id)
        {
            // 確認帳號是否存在
            if (this.unitOfWork.Repository<ApplicationUser>().Any(p => p.UserName == model.UserName))
                throw new Exception(string.Format("帳號 {0} 已經存在", model.UserName));

            // 當角色是 DepartmentHead 或者 Employee，必須指定部門
            var roleName = this.apiControllerHelper.GetRoleName(model.RoleId);
            Role role = (Role)Enum.Parse(typeof(Role), roleName);
            if (role == Role.DepartmentHead || role == Role.Employee)
            {
                if (model.DepartmentId == 0)
                    throw new Exception("當角色為部門主管或員工時，必須指定部門");
            }

            entity = new ApplicationUser();
            entity.Level = CurrentUser.Level - 1;
            entity.ParentId = CurrentUser.Id;
            entity.UserName = model.UserName;
            entity.FullName = model.FullName;
            entity.SmsBalance = 0M;
            entity.SmsBalanceExpireDate = DateTime.MaxValue;
            entity.Department = model.DepartmentId == 0 ? null : this.unitOfWork.Repository<Department>().Get(p => p.Id == model.DepartmentId);
            entity.EmployeeNo = model.EmployeeNo;
            entity.PhoneNumber = model.PhoneNumber;
            entity.Email = model.Email;
            entity.Enabled = true;

            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(this.unitOfWork.DbContext));

            // 建立帳號
            var result = EFunTech.Sms.Core.AsyncHelper.RunSync<IdentityResult>(() => userManager.CreateAsync(entity, model.NewPassword));
            if (!result.Succeeded)
            {
                string errors = string.Join(",", result.Errors.ToList());
                throw new Exception(string.Format("建立子帳號失敗，{0}", errors));
            }

            // 加入腳色
            userManager.AddToRoleAsync(entity.Id, this.apiControllerHelper.GetRoleName(model.RoleId)).Wait();

            id = entity.Id;

            CreditWarning CreditWarning = new CreditWarning
            {
                Enabled = CreditWarning.DefaultValue_Enabled,
                BySmsMessage = CreditWarning.DefaultValue_BySmsMessage,
                ByEmail = CreditWarning.DefaultValue_ByEmail,
                LastNotifiedTime = null,
                NotifiedInterval = CreditWarning.DefaultValue_NotifiedInterval,
                Owner = entity,
            };
            this.unitOfWork.Repository<CreditWarning>().Insert(CreditWarning);

            ReplyCc ReplyCc = new ReplyCc
            {
                Enabled = ReplyCc.DefaultValue_Enabled,
                BySmsMessage = ReplyCc.DefaultValue_BySmsMessage,
                ByEmail = ReplyCc.DefaultValue_ByEmail,
                Owner = entity,
            };
            this.unitOfWork.Repository<ReplyCc>().Insert(ReplyCc);

            // 建立預設群組 [常用聯絡人]
            var group = new Group();
            group.Name = Group.CommonContactGroupName;
            group.Description = Group.CommonContactGroupName;
            group.Deletable = false;
            group.CreatedUser = entity;
            group = this.unitOfWork.Repository<Group>().Insert(group);

            return entity;
        }

        protected override void DoUpdate(ApplicationUserModel model, string id, ApplicationUser entity)
        {
            // 當角色是 DepartmentHead 或者 Employee，必須指定部門
            var roleName = this.apiControllerHelper.GetRoleName(model.RoleId);
            Role role = (Role)Enum.Parse(typeof(Role), roleName);
            if (role == Role.DepartmentHead || role == Role.Employee)
            {
                if (model.DepartmentId == 0)
                    throw new Exception("當角色為部門主管或員工時，必須指定部門");
            }

            //設定部門
            entity.Department = model.DepartmentId == 0 ? null : this.unitOfWork.Repository<Department>().Get(p => p.Id == model.DepartmentId);

            //是否有修改角色
            var newRoleId = model.RoleId;
            var oldRoleId = this.apiControllerHelper.GetRoleId(model.Id);
            if (newRoleId != oldRoleId)
            {
                var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(this.unitOfWork.DbContext));
                var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(this.unitOfWork.DbContext));

                string oldRoleName = this.apiControllerHelper.GetRoleName(oldRoleId);
                if (!string.IsNullOrEmpty(oldRoleName))
                    userManager.RemoveFromRoleAsync(model.Id, oldRoleName).Wait();

                userManager.AddToRoleAsync(model.Id, this.apiControllerHelper.GetRoleName(newRoleId)).Wait();
            }

            //是否有修改密碼
            if (!string.IsNullOrEmpty(model.NewPassword))
            {
                // http://stackoverflow.com/questions/19524111/asp-net-identity-reset-password
                var store = new UserStore<ApplicationUser>(this.unitOfWork.DbContext);
                var userManager = new UserManager<ApplicationUser>(store);
                var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(this.unitOfWork.DbContext));

                String hashedNewPassword = userManager.PasswordHasher.HashPassword(model.NewPassword);
                var user = this.repository.GetById(model.Id);

                EFunTech.Sms.Core.AsyncHelper.RunSync(() => store.SetPasswordHashAsync(user, hashedNewPassword));
                EFunTech.Sms.Core.AsyncHelper.RunSync(() => store.UpdateAsync(user));
            }

            if (entity.Enabled)
            {
                entity.LockoutEnabled = false;
                entity.LockoutEndDateUtc = null;
            }
            else
            {
                entity.LockoutEnabled = true;
                entity.LockoutEndDateUtc = DateTime.MaxValue;
            }

            this.repository.Update(entity);
        }

        //protected override void DoRemove(string id, ApplicationUser entity)
        //{
        //    var currentUser = this.repository.GetById(CurrentUser.Id);
        //    var childUser = this.repository.GetById(id);

        //    // 只能移除子帳號
        //    if (childUser.ParentId != currentUser.Id) return;


        //    this.unitOfWork.Repository<Blacklist>().Delete(p => p.CreatedUser.Id == childUser.Id);
        //    this.unitOfWork.Repository<CommonMessage>().Delete(p => p.CreatedUser.Id == childUser.Id);
        //    this.unitOfWork.Repository<UploadedFile>().Delete(p => p.CreatedUser.Id == childUser.Id);
        //    this.unitOfWork.Repository<Signature>().Delete(p => p.CreatedUser.Id == childUser.Id);

        //    var sendMessageRuleRepository = this.unitOfWork.Repository<SendMessageRule>();
        //    var sendMessageRules = sendMessageRuleRepository.GetMany(p => p.CreatedUser.Id == childUser.Id);
        //    var sendMessageRuleIds = sendMessageRules.Select(p => p.Id);
        //    // 刪除與 SendMessageRule 相依的 SendMessageQueue
        //    this.unitOfWork.Repository<SendMessageQueue>().Delete(p => sendMessageRuleIds.Contains(p.SendMessageRuleId));
        //    // 刪除 SendMessageRule 之前必須回補點數
        //    sendMessageRules.ForEach(p => this.tradeService.DeleteSendMessageRule(p)); 
        //    this.unitOfWork.Repository<SendMessageRule>().Delete(p => p.CreatedUser.Id == childUser.Id);

        //    var contactIds = this.unitOfWork.Repository<Contact>().GetMany(p => p.CreatedUser.Id == childUser.Id).Select(p => p.Id);
        //    this.unitOfWork.Repository<GroupContact>().Delete(p => contactIds.Contains(p.ContactId));
        //    this.unitOfWork.Repository<Contact>().Delete(p => p.CreatedUser.Id == childUser.Id);

        //    var groupIds = this.unitOfWork.Repository<Group>().GetMany(p => p.CreatedUser.Id == childUser.Id).Select(p => p.Id);
        //    this.unitOfWork.Repository<GroupContact>().Delete(p => groupIds.Contains(p.GroupId));
        //    this.unitOfWork.Repository<Group>().Delete(p => p.CreatedUser.Id == childUser.Id);

        //    // this.unitOfWork.Repository<TradeDetail>().Delete(p => p.OwnerId == childUser.Id); // 不確定是否需要刪除交易明細

        //    this.unitOfWork.Repository<AllotSetting>().Delete(p => p.Owner.Id == childUser.Id);
        //    this.unitOfWork.Repository<CreditWarning>().Delete(p => p.Owner.Id == childUser.Id);
        //    this.unitOfWork.Repository<ReplyCc>().Delete(p => p.Owner.Id == childUser.Id);

        //    //是否有修改角色
        //    var childUserRoleId = this.apiControllerHelper.GetRoleId(childUser.Id);
        //    var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(this.unitOfWork.DbContext));

        //    string childUserRoleName = this.apiControllerHelper.GetRoleName(childUserRoleId);
        //    if (!string.IsNullOrEmpty(childUserRoleName))
        //        userManager.RemoveFromRoleAsync(childUser.Id, childUserRoleName).Wait();

        //    this.repository.Delete(childUser);

        //    this.tradeService.DeleteChildUser(currentUser, childUser);
        //}

        protected override void DoRemove(string id, ApplicationUser entity)
        {
            var user = this.repository.GetById(id);

            // 刪除指定帳號必須確保該帳號所建立的所有使用者都已經刪除

            var childUsers = this.apiControllerHelper.GetDescendingUsers(entity);
            if (childUsers.Count != 0)
            {
                string error = string.Format("使用者【{0}】下還有 {1} 位使用者，分別是 {2}，請先刪除該此使用者下所有使用者，之後再刪除此使用者",
                    entity.UserName,
                    childUsers.Count,
                    string.Join("、", childUsers.Select(p => p.UserName)));
                throw new Exception(error);
            }

            // 移除使用者相關資料

            this.unitOfWork.Repository<Blacklist>().Delete(p => p.CreatedUser.Id == id);
            this.unitOfWork.Repository<CommonMessage>().Delete(p => p.CreatedUser.Id == id);
            this.unitOfWork.Repository<UploadedFile>().Delete(p => p.CreatedUser.Id == id);
            this.unitOfWork.Repository<Signature>().Delete(p => p.CreatedUser.Id == id);

            var sendMessageRuleRepository = this.unitOfWork.Repository<SendMessageRule>();
            var sendMessageRules = sendMessageRuleRepository.GetMany(p => p.CreatedUser.Id == id);
            var sendMessageRuleIds = sendMessageRules.Select(p => p.Id);
            // 刪除與 SendMessageRule 相依的 SendMessageQueue
            this.unitOfWork.Repository<SendMessageQueue>().Delete(p => sendMessageRuleIds.Contains(p.SendMessageRuleId));
            // 刪除 SendMessageRule 之前必須回補點數
            sendMessageRules.ForEach(p => this.tradeService.DeleteSendMessageRule(p));
            this.unitOfWork.Repository<SendMessageRule>().Delete(p => p.CreatedUser.Id == id);

            var contactIds = this.unitOfWork.Repository<Contact>().GetMany(p => p.CreatedUser.Id == id).Select(p => p.Id);
            this.unitOfWork.Repository<GroupContact>().Delete(p => contactIds.Contains(p.ContactId));
            this.unitOfWork.Repository<Contact>().Delete(p => p.CreatedUser.Id == id);

            var groupIds = this.unitOfWork.Repository<Group>().GetMany(p => p.CreatedUser.Id == id).Select(p => p.Id);
            this.unitOfWork.Repository<GroupContact>().Delete(p => groupIds.Contains(p.GroupId));
            this.unitOfWork.Repository<Group>().Delete(p => p.CreatedUser.Id == id);

            this.unitOfWork.Repository<SharedGroupContact>().Delete(p => p.ShareToUserId == id);
            
            //FK_dbo.SharedGroupContacts_dbo.AspNetUsers_ShareToUserId

            // this.unitOfWork.Repository<TradeDetail>().Delete(p => p.OwnerId == childUser.Id); // 不確定是否需要刪除交易明細

            this.unitOfWork.Repository<AllotSetting>().Delete(p => p.Owner.Id == id);
            this.unitOfWork.Repository<CreditWarning>().Delete(p => p.Owner.Id == id);
            this.unitOfWork.Repository<ReplyCc>().Delete(p => p.Owner.Id == id);

            //是否有修改角色
            var childUserRoleId = this.apiControllerHelper.GetRoleId(id);
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(this.unitOfWork.DbContext));

            string childUserRoleName = this.apiControllerHelper.GetRoleName(childUserRoleId);
            if (!string.IsNullOrEmpty(childUserRoleName))
                userManager.RemoveFromRoleAsync(id, childUserRoleName).Wait();

            this.tradeService.DeleteUser(user);

            this.repository.Delete(user);
        }

        protected override void DoRemove(List<string> ids, List<ApplicationUser> entities)
        {
            for (var i = 0; i < entities.Count; i++)
            {
                DoRemove(ids[i], entities[i]);
            }
        }

        protected override IEnumerable<ApplicationUserModel> ConvertModel(IEnumerable<ApplicationUserModel> models)
        {
            foreach (var model in models)
            {
                var isCurrentUser = model.Id == CurrentUser.Id;

                model.Activatable = !isCurrentUser; // 是否可以啟用或關閉
                model.Maintainable = true; // 是否可以修改帳號設定
                model.Deletable = !isCurrentUser; // 是否可以刪除帳號
                model.DepartmentId = model.Department != null ? model.Department.Id : 0;
                model.RoleId = this.apiControllerHelper.GetRoleId(model.Id);
                model.NewPassword = string.Empty;
                model.NewPasswordConfirmed = string.Empty;

                var user = this.unitOfWork.Repository<ApplicationUser>().GetById(model.Id);
                if (user != null && user.Parent != null)
                    model.CreatedUserName = user.Parent.UserName;
            }

            return models;
        }
    }
}
