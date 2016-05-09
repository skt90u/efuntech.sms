using EFunTech.Sms.Portal.Models;
using EFunTech.Sms.Schema;
using System.Linq;
using EFunTech.Sms.Portal.Controllers.Common;

using System.Collections.Generic;
using System;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using EFunTech.Sms.Portal.Models.Criteria;
using LinqKit;
using System.Data.Entity;
using System.Threading.Tasks;
using JUtilSharp.Database;
using EFunTech.Sms.Core;
using System.Data.Entity.Validation;
using System.Collections.Concurrent;
using System.Transactions;

namespace EFunTech.Sms.Portal.Controllers
{
    public class DepartmentManagerController : CrudApiController<DepartmentManagerCriteriaModel, ApplicationUserModel, ApplicationUser, string>
    {
        protected ApiControllerHelper apiControllerHelper;
        protected TradeService tradeService;

        public DepartmentManagerController(ISystemParameters systemParameters, DbContext context, ILogService logService)
            : base(context, logService)
        {
            this.apiControllerHelper = new ApiControllerHelper(context, logService);
            this.tradeService = new TradeService(new UnitOfWork(context), logService);
        }

        // 只能管理子帳號
        //protected override IQueryable<ApplicationUser> DoGetList(DepartmentManagerCriteriaModel criteria)
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
        protected override IQueryable<ApplicationUser> DoGetList(DepartmentManagerCriteriaModel criteria)
        {
            HandleDeletedUserQueue();

            // 尋找目前使用者以及目前使用者的子帳號或孫帳號
            IEnumerable<ApplicationUser> users = this.apiControllerHelper.GetDescendingUsersAndUser(CurrentUserId);

            // 尋找目前使用者以及目前使用者的子帳號
            var result = users.AsQueryable();
            //var result = users.Where(p => !p.Deleted).AsQueryable();

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

        /// <summary>
        /// 建立子帳號
        /// </summary>
        /// <param name="model"></param>
        /// <param name="entity"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        protected override async Task<ApplicationUser> DoCreate(ApplicationUserModel model, ApplicationUser entity)
        {
            // 確認帳號是否存在
            if (context.Set<ApplicationUser>().Any(p => p.UserName == model.UserName))
                throw new Exception(string.Format("帳號 {0} 已經存在", model.UserName));

            // 當角色是 DepartmentHead 或者 Employee，必須指定部門
            var roleName = GetRoleName(model.RoleId);
            var role = (Role)Enum.Parse(typeof(Role), roleName);
            if (role == Role.DepartmentHead || role == Role.Employee)
            {
                if (model.DepartmentId == 0)
                    throw new Exception("當角色為部門主管或員工時，必須指定所屬部門");
            }

            entity = new ApplicationUser();
            entity.Level = CurrentUser.Level - 1;
            entity.ParentId = CurrentUserId;
            entity.UserName = model.UserName;
            entity.FullName = model.FullName;
            entity.SmsBalance = 0M;
            entity.SmsBalanceExpireDate = DateTime.MaxValue;
            entity.Department = model.DepartmentId == 0 
                ? null 
                : context.Set<Department>().FirstOrDefault(p => p.Id == model.DepartmentId);
            entity.EmployeeNo = model.EmployeeNo;
            entity.PhoneNumber = model.PhoneNumber;
            entity.Email = model.Email;
            entity.Enabled = true;
            entity.SmsProviderType = model.SmsProviderType;

            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));

            // 建立帳號
            var result = await userManager.CreateAsync(
                                        entity, 
                                        model.NewPassword);
            if (!result.Succeeded)
            {
                string errors = string.Join(",", result.Errors.ToList());
                throw new Exception(string.Format("建立子帳號失敗，{0}", errors));
            }

            // 加入腳色
            await userManager.AddToRoleAsync(
                            entity.Id, 
                            GetRoleName(model.RoleId));

            var creditWarning = new CreditWarning
            {
                Enabled = CreditWarning.DefaultValue_Enabled,
                BySmsMessage = CreditWarning.DefaultValue_BySmsMessage,
                ByEmail = CreditWarning.DefaultValue_ByEmail,
                LastNotifiedTime = null,
                NotifiedInterval = CreditWarning.DefaultValue_NotifiedInterval,
                OwnerId = entity.Id,
            };
            await context.InsertAsync(creditWarning);

            var replyCc = new ReplyCc
            {
                Enabled = ReplyCc.DefaultValue_Enabled,
                BySmsMessage = ReplyCc.DefaultValue_BySmsMessage,
                ByEmail = ReplyCc.DefaultValue_ByEmail,
                OwnerId = entity.Id,
            };
            await context.InsertAsync(replyCc);

            // 建立預設群組 [常用聯絡人]
            var group = new Group();
            group.Name = Group.CommonContactGroupName;
            group.Description = Group.CommonContactGroupName;
            group.Deletable = false;
            group.CreatedUser = entity;
            await context.InsertAsync(group);

            return entity;
        }

        protected override async Task DoUpdate(ApplicationUserModel model, string id, ApplicationUser entity)
        {
            // 使用 Single Page Application 開發後，如果使用 Mapper.Map 有可能更新到畫面上沒有的欄位，例如：目前可用餘額(SmsBalance)
            entity = await DoGet(id);

            // 啟用 | 關閉
            entity.Enabled = model.Enabled;
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

            // 姓名
            entity.FullName = model.FullName;

            // 部門
            entity.Department = model.DepartmentId == 0 ? null : context.Set<Department>().FirstOrDefault(p => p.Id == model.DepartmentId);

            // 角色
            var roleName = GetRoleName(model.RoleId);
            var role = (Role)Enum.Parse(typeof(Role), roleName);
            if (role == Role.DepartmentHead || role == Role.Employee)
            {
                if (model.DepartmentId == 0)
                    throw new Exception("當角色為部門主管或員工時，必須指定部門");
            }

            var newRoleId = model.RoleId;
            var oldRoleId = GetIdentityRole(model.Id).Id;
            if (newRoleId != oldRoleId)
            {
                var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
                var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));

                string oldRoleName = GetRoleName(oldRoleId);
                if (!string.IsNullOrEmpty(oldRoleName))
                    userManager.RemoveFromRoleAsync(model.Id, oldRoleName).Wait();

                userManager.AddToRoleAsync(model.Id, GetRoleName(newRoleId)).Wait();
            }

            // 員工編號
            entity.EmployeeNo = model.EmployeeNo;

            // 行動電話
            entity.PhoneNumber = model.PhoneNumber;

            // 電子郵件
            entity.Email = model.Email;

            // 簡訊供應商類型，用以指定首要簡訊提供商
            entity.SmsProviderType = model.SmsProviderType;

            // 重設密碼
            if (!string.IsNullOrEmpty(model.NewPassword))
            {
                // http://stackoverflow.com/questions/19524111/asp-net-identity-reset-password
                var store = new UserStore<ApplicationUser>(context);
                var userManager = new UserManager<ApplicationUser>(store);
                var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));

                String hashedNewPassword = userManager.PasswordHasher.HashPassword(model.NewPassword);
                var user = await DoGet(model.Id);

                store.SetPasswordHashAsync(user, hashedNewPassword).Wait();
                store.UpdateAsync(user).Wait();
            }

            await context.UpdateAsync(entity);
        }

        protected override async Task DoRemove(string id)
        {
            ApplicationUser entity = await DoGet(id);

            if (entity == null)
            {
                string error = string.Format("找不到使用者(id = {0})", id);
                throw new Exception(error);
            }

            // 刪除指定帳號必須確保該帳號所建立的所有使用者都已經刪除

            var childUsers = this.apiControllerHelper.GetDescendingUsers(entity.Id);
            if (childUsers.Count() != 0)
            {
                string error = string.Format("使用者【{0}】下還有 {1} 位使用者，分別是 {2}，請先刪除該此使用者下所有使用者，之後再刪除此使用者",
                    entity.UserName,
                    childUsers.Count(),
                    string.Join("、", childUsers.Select(p => p.UserName)));
                throw new Exception(error);
            }

            // (1)
            context.Delete<AllotSetting>(p => p.Owner != null && p.Owner.Id == id);
            // (2)
            // AspNetUserClaims
            // (3)
            // AspNetUserLogins
            // (4)
            // AspNetUserRoles
            // (5)
            context.Delete<CommonMessage>(p => p.CreatedUserId == id);

            var contactIds = context.Set<Contact>().Where(p => p.CreatedUserId == id).Select(p => p.Id);
            var groupIds = context.Set<Group>().Where(p => p.CreatedUserId == id).Select(p => p.Id);

            // (6)
            context.Delete<GroupContact>(p => contactIds.Contains(p.ContactId));
            context.Delete<GroupContact>(p => groupIds.Contains(p.GroupId));
            context.Delete<Contact>(p => p.CreatedUserId == id);

            // (7)
            context.Delete<CreditWarning>(p => p.Owner.Id == id);

            var departmentIds = context.Set<Department>().Where(p => p.CreatedUserId == id).Select(p => p.Id).ToList();
            if (departmentIds.Count != 0)
            {
                var users = context.Set<ApplicationUser>().Where(p => p.Department != null && departmentIds.Contains(p.Department.Id)).ToList();
                foreach (var user in users)
                {
                    user.Department = null;
                    context.Update(user);
                }
            }
            context.Delete<Department>(p => p.CreatedUserId == id);

            // (8)
            context.Delete<SharedGroupContact>(p => p.ShareToUserId == id);
            context.Delete<Group>(p => p.CreatedUserId == id);

            // (9)
            context.Delete<ReplyCc>(p => p.Owner.Id == id);

            // (10)
            var sendMessageRules = context.Set<SendMessageRule>().Where(p => p.CreatedUserId == id).ToList();

            if (sendMessageRules.Count != 0)
            {
                // 刪除 SendMessageRule 之前必須回補點數
                this.tradeService.DeleteSendMessageRules(sendMessageRules);

                var sendMessageRuleIds = sendMessageRules.Select(p => p.Id).ToList();

                // 刪除與 SendMessageRule 相依的 SendMessageQueue
                context.Delete<SendMessageQueue>(p => sendMessageRuleIds.Contains(p.SendMessageRuleId));

                context.Delete<MessageReceiver>(p => sendMessageRuleIds.Contains(p.SendMessageRuleId));
                context.Delete<RecipientFromCommonContact>(p => sendMessageRuleIds.Contains(p.SendMessageRuleId));
                context.Delete<RecipientFromFileUpload>(p => sendMessageRuleIds.Contains(p.SendMessageRuleId));
                context.Delete<RecipientFromGroupContact>(p => sendMessageRuleIds.Contains(p.SendMessageRuleId));
                context.Delete<RecipientFromManualInput>(p => sendMessageRuleIds.Contains(p.SendMessageRuleId));
                context.Delete<SendCycleEveryDay>(p => sendMessageRuleIds.Contains(p.SendMessageRuleId));
                context.Delete<SendCycleEveryMonth>(p => sendMessageRuleIds.Contains(p.SendMessageRuleId));
                context.Delete<SendCycleEveryWeek>(p => sendMessageRuleIds.Contains(p.SendMessageRuleId));
                context.Delete<SendCycleEveryYear>(p => sendMessageRuleIds.Contains(p.SendMessageRuleId));
                context.Delete<SendDeliver>(p => sendMessageRuleIds.Contains(p.SendMessageRuleId));
            }

            // 刪除 SendMessageRule
            context.Delete<SendMessageRule>(p => p.CreatedUserId == id);

            // (11)
            context.Delete<Signature>(p => p.CreatedUserId == id);

            // (12)
            context.Delete<SystemAnnouncement>(p => p.CreatedUserId == id);

            // (13)
            context.Delete<Blacklist>(p => p.CreatedUserId == id);
            context.Delete<UploadedMessageReceiver>(p => p.CreatedUserId == id);
            context.Delete<UploadedFile>(p => p.CreatedUserId == id);

            context.Delete<SendMessageHistory>(p => p.CreatedUserId == id);
            context.Delete<SendMessageStatistic>(p => p.CreatedUserId == id);

            this.tradeService.DeleteUser(entity);

            EnqueueDeletedUser(id);
        }

        // 20160509 Norman, 
        //  目前沒辦法直接在 DoRemove, 完整刪除使用者, 估計是因為 CreateTransactionScope 所造成的
        //
        // 解決方式, 分兩階段才能夠真正刪除資料
        //  - (1) DoRemove
        //      step 1: 刪除相依的資料
        //      step 2: EnqueueDeletedUser(id)
        //  - (2) DoGetList
        //      step 1: 刪除使用者 -> 透過 HandleDeletedUserQueue
        //      step 2: 查詢使用者資料

        private static ConcurrentQueue<string> DeletedUserIds = new ConcurrentQueue<string>();

        private void EnqueueDeletedUser(string id)
        {
            DeletedUserIds.Enqueue(id);
        }

        private void HandleDeletedUserQueue()
        {
            using (TransactionScope scope = context.CreateTransactionScope())
            {
                while (DeletedUserIds.Count != 0)
                {
                    string id = null;
                    if (DeletedUserIds.TryDequeue(out id))
                    {
                        DeleteUserById(id);
                    }
                }

                scope.Complete();
            }
        }

        private void DeleteUserById(string id)
        {
            ApplicationUser entity = DoGet(id).GetAwaiter().GetResult();

            var logins = entity.Logins.ToList();
            var claims = entity.Claims.ToList();
            foreach (var login in logins) entity.Logins.Remove(login);
            foreach (var claim in claims) entity.Claims.Remove(claim);

            //是否有修改角色
            var childUserRoleId = GetIdentityRole(id).Id;
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));

            string childUserRoleName = GetRoleName(childUserRoleId);
            if (!string.IsNullOrEmpty(childUserRoleName))
                userManager.RemoveFromRoles(id, childUserRoleName);

            userManager.Delete(entity);
        }


        protected override async Task DoRemove(string[] ids) 
        {
            for (var i = 0; i < ids.Length; i++)
            {
                await DoRemove(ids[i]);
            }
        }

        protected override IEnumerable<ApplicationUserModel> ConvertModel(IEnumerable<ApplicationUserModel> models)
        {
            foreach (var model in models)
            {
                var isCurrentUser = model.Id == CurrentUserId;

                model.Activatable = !isCurrentUser; // 是否可以啟用或關閉
                model.Maintainable = true; // 是否可以修改帳號設定
                model.Deletable = !isCurrentUser; // 是否可以刪除帳號
                //model.Deletable = false; // TODO: 20160331 Norman, 目前尚未解決刪除使用者會拋出例外的問題，先將刪除按鈕影藏
                model.DepartmentId = model.Department != null ? model.Department.Id : 0;
                model.RoleId = GetIdentityRole(model.Id).Id;
                model.NewPassword = string.Empty;
                model.NewPasswordConfirmed = string.Empty;

                var user = GetUser(model.Id);
                if (user != null && user.Parent != null)
                    model.CreatedUserName = user.Parent.UserName;
            }

            return models;
        }
    }
}
