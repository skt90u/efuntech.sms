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

        // �u��޲z�l�b��
        //protected override IQueryable<ApplicationUser> DoGetList(DepartmentManagerCriteriaModel criteria)
        //{
        //    // �M��ثe�ϥΪ̥H�Υثe�ϥΪ̪��l�b��
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
        /// �d�ߤl�b���H�Τl�b���ҫإߪ��b��
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns></returns>
        protected override IQueryable<ApplicationUser> DoGetList(DepartmentManagerCriteriaModel criteria)
        {
            HandleDeletedUserQueue();

            // �M��ثe�ϥΪ̥H�Υثe�ϥΪ̪��l�b���ή]�b��
            IEnumerable<ApplicationUser> users = this.apiControllerHelper.GetDescendingUsersAndUser(CurrentUserId);

            // �M��ثe�ϥΪ̥H�Υثe�ϥΪ̪��l�b��
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

            // �Ѥj��p�ƦC
            return result.OrderByDescending(p => p.Level).ThenBy(p => p.Id);
        }

        /// <summary>
        /// �إߤl�b��
        /// </summary>
        /// <param name="model"></param>
        /// <param name="entity"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        protected override async Task<ApplicationUser> DoCreate(ApplicationUserModel model, ApplicationUser entity)
        {
            // �T�{�b���O�_�s�b
            if (context.Set<ApplicationUser>().Any(p => p.UserName == model.UserName))
                throw new Exception(string.Format("�b�� {0} �w�g�s�b", model.UserName));

            // ����O DepartmentHead �Ϊ� Employee�A�������w����
            var roleName = GetRoleName(model.RoleId);
            var role = (Role)Enum.Parse(typeof(Role), roleName);
            if (role == Role.DepartmentHead || role == Role.Employee)
            {
                if (model.DepartmentId == 0)
                    throw new Exception("���⬰�����D�ީέ��u�ɡA�������w���ݳ���");
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

            // �إ߱b��
            var result = await userManager.CreateAsync(
                                        entity, 
                                        model.NewPassword);
            if (!result.Succeeded)
            {
                string errors = string.Join(",", result.Errors.ToList());
                throw new Exception(string.Format("�إߤl�b�����ѡA{0}", errors));
            }

            // �[�J�}��
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

            // �إ߹w�]�s�� [�`���p���H]
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
            // �ϥ� Single Page Application �}�o��A�p�G�ϥ� Mapper.Map ���i���s��e���W�S�������A�Ҧp�G�ثe�i�ξl�B(SmsBalance)
            entity = await DoGet(id);

            // �ҥ� | ����
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

            // �m�W
            entity.FullName = model.FullName;

            // ����
            entity.Department = model.DepartmentId == 0 ? null : context.Set<Department>().FirstOrDefault(p => p.Id == model.DepartmentId);

            // ����
            var roleName = GetRoleName(model.RoleId);
            var role = (Role)Enum.Parse(typeof(Role), roleName);
            if (role == Role.DepartmentHead || role == Role.Employee)
            {
                if (model.DepartmentId == 0)
                    throw new Exception("���⬰�����D�ީέ��u�ɡA�������w����");
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

            // ���u�s��
            entity.EmployeeNo = model.EmployeeNo;

            // ��ʹq��
            entity.PhoneNumber = model.PhoneNumber;

            // �q�l�l��
            entity.Email = model.Email;

            // ²�T�����������A�ΥH���w���n²�T���Ѱ�
            entity.SmsProviderType = model.SmsProviderType;

            // ���]�K�X
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
                string error = string.Format("�䤣��ϥΪ�(id = {0})", id);
                throw new Exception(error);
            }

            // �R�����w�b�������T�O�ӱb���ҫإߪ��Ҧ��ϥΪ̳��w�g�R��

            var childUsers = this.apiControllerHelper.GetDescendingUsers(entity.Id);
            if (childUsers.Count() != 0)
            {
                string error = string.Format("�ϥΪ̡i{0}�j�U�٦� {1} ��ϥΪ̡A���O�O {2}�A�Х��R���Ӧ��ϥΪ̤U�Ҧ��ϥΪ̡A����A�R�����ϥΪ�",
                    entity.UserName,
                    childUsers.Count(),
                    string.Join("�B", childUsers.Select(p => p.UserName)));
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
                // �R�� SendMessageRule ���e�����^���I��
                this.tradeService.DeleteSendMessageRules(sendMessageRules);

                var sendMessageRuleIds = sendMessageRules.Select(p => p.Id).ToList();

                // �R���P SendMessageRule �̪ۨ� SendMessageQueue
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

            // �R�� SendMessageRule
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
        //  �ثe�S��k�����b DoRemove, ����R���ϥΪ�, ���p�O�]�� CreateTransactionScope �ҳy����
        //
        // �ѨM�覡, ���ⶥ�q�~����u���R�����
        //  - (1) DoRemove
        //      step 1: �R���̪ۨ����
        //      step 2: EnqueueDeletedUser(id)
        //  - (2) DoGetList
        //      step 1: �R���ϥΪ� -> �z�L HandleDeletedUserQueue
        //      step 2: �d�ߨϥΪ̸��

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

            //�O�_���ק﨤��
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

                model.Activatable = !isCurrentUser; // �O�_�i�H�ҥΩ�����
                model.Maintainable = true; // �O�_�i�H�ק�b���]�w
                model.Deletable = !isCurrentUser; // �O�_�i�H�R���b��
                //model.Deletable = false; // TODO: 20160331 Norman, �ثe�|���ѨM�R���ϥΪ̷|�ߥX�ҥ~�����D�A���N�R�����s�v��
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
