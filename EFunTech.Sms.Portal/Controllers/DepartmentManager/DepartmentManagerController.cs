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

namespace EFunTech.Sms.Portal.Controllers
{
    public class DepartmentManagerController : AsyncCrudApiController<DepartmentManagerCriteriaModel, ApplicationUserModel, ApplicationUser, string>
    {
        protected ApiControllerHelper apiControllerHelper;
        protected TradeService tradeService;

        public DepartmentManagerController(ISystemParameters systemParameters, DbContext context, ILogService logService)
            : base(context, logService)
        {
            this.apiControllerHelper = new ApiControllerHelper(new UnitOfWork(context), logService);
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
            // �M��ثe�ϥΪ̥H�Υثe�ϥΪ̪��l�b���ή]�b��
            List<ApplicationUser> users = this.apiControllerHelper.GetDescendingUsersAndUser(CurrentUser);

            // �M��ثe�ϥΪ̥H�Υثe�ϥΪ̪��l�b��
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
            var roleName = this.apiControllerHelper.GetRoleName(model.RoleId);
            Role role = (Role)Enum.Parse(typeof(Role), roleName);
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

            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));

            // �إ߱b��
            var result = userManager.CreateAsync(
                                        entity, 
                                        model.NewPassword)
                                    .GetAwaiter()
                                    .GetResult();
            if (!result.Succeeded)
            {
                string errors = string.Join(",", result.Errors.ToList());
                throw new Exception(string.Format("�إߤl�b�����ѡA{0}", errors));
            }

            // �[�J�}��
            userManager.AddToRoleAsync(
                            entity.Id, 
                            this.apiControllerHelper.GetRoleName(model.RoleId))
                        .Wait();

            CreditWarning CreditWarning = new CreditWarning
            {
                Enabled = CreditWarning.DefaultValue_Enabled,
                BySmsMessage = CreditWarning.DefaultValue_BySmsMessage,
                ByEmail = CreditWarning.DefaultValue_ByEmail,
                LastNotifiedTime = null,
                NotifiedInterval = CreditWarning.DefaultValue_NotifiedInterval,
                Owner = entity,
            };
            await context.InsertAsync(CreditWarning);

            ReplyCc ReplyCc = new ReplyCc
            {
                Enabled = ReplyCc.DefaultValue_Enabled,
                BySmsMessage = ReplyCc.DefaultValue_BySmsMessage,
                ByEmail = ReplyCc.DefaultValue_ByEmail,
                Owner = entity,
            };
            await context.InsertAsync(ReplyCc);

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
            // ����O DepartmentHead �Ϊ� Employee�A�������w����
            var roleName = this.apiControllerHelper.GetRoleName(model.RoleId);
            Role role = (Role)Enum.Parse(typeof(Role), roleName);
            if (role == Role.DepartmentHead || role == Role.Employee)
            {
                if (model.DepartmentId == 0)
                    throw new Exception("���⬰�����D�ީέ��u�ɡA�������w����");
            }

            //�]�w����
            entity.Department = model.DepartmentId == 0 ? null : context.Set<Department>().FirstOrDefault(p => p.Id == model.DepartmentId);

            //�O�_���ק﨤��
            var newRoleId = model.RoleId;
            var oldRoleId = this.apiControllerHelper.GetRoleId(model.Id);
            if (newRoleId != oldRoleId)
            {
                var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
                var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));

                string oldRoleName = this.apiControllerHelper.GetRoleName(oldRoleId);
                if (!string.IsNullOrEmpty(oldRoleName))
                    userManager.RemoveFromRoleAsync(model.Id, oldRoleName).Wait();

                userManager.AddToRoleAsync(model.Id, this.apiControllerHelper.GetRoleName(newRoleId)).Wait();
            }

            //�O�_���ק�K�X
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

            await context.UpdateAsync(entity);
        }

        //protected override void DoRemove(string id, ApplicationUser entity)
        //{
        //    var currentUser = this.repository.GetById(CurrentUser.Id);
        //    var childUser = this.repository.GetById(id);

        //    // �u�ಾ���l�b��
        //    if (childUser.ParentId != currentUser.Id) return;


        //    this.unitOfWork.Repository<Blacklist>().Delete(p => p.CreatedUser.Id == childUser.Id);
        //    this.unitOfWork.Repository<CommonMessage>().Delete(p => p.CreatedUser.Id == childUser.Id);
        //    this.unitOfWork.Repository<UploadedFile>().Delete(p => p.CreatedUser.Id == childUser.Id);
        //    this.unitOfWork.Repository<Signature>().Delete(p => p.CreatedUser.Id == childUser.Id);

        //    var sendMessageRuleRepository = this.unitOfWork.Repository<SendMessageRule>();
        //    var sendMessageRules = sendMessageRuleRepository.GetMany(p => p.CreatedUser.Id == childUser.Id);
        //    var sendMessageRuleIds = sendMessageRules.Select(p => p.Id);
        //    // �R���P SendMessageRule �̪ۨ� SendMessageQueue
        //    this.unitOfWork.Repository<SendMessageQueue>().Delete(p => sendMessageRuleIds.Contains(p.SendMessageRuleId));
        //    // �R�� SendMessageRule ���e�����^���I��
        //    sendMessageRules.ForEach(p => this.tradeService.DeleteSendMessageRule(p)); 
        //    this.unitOfWork.Repository<SendMessageRule>().Delete(p => p.CreatedUser.Id == childUser.Id);

        //    var contactIds = this.unitOfWork.Repository<Contact>().GetMany(p => p.CreatedUser.Id == childUser.Id).Select(p => p.Id);
        //    this.unitOfWork.Repository<GroupContact>().Delete(p => contactIds.Contains(p.ContactId));
        //    this.unitOfWork.Repository<Contact>().Delete(p => p.CreatedUser.Id == childUser.Id);

        //    var groupIds = this.unitOfWork.Repository<Group>().GetMany(p => p.CreatedUser.Id == childUser.Id).Select(p => p.Id);
        //    this.unitOfWork.Repository<GroupContact>().Delete(p => groupIds.Contains(p.GroupId));
        //    this.unitOfWork.Repository<Group>().Delete(p => p.CreatedUser.Id == childUser.Id);

        //    // this.unitOfWork.Repository<TradeDetail>().Delete(p => p.OwnerId == childUser.Id); // ���T�w�O�_�ݭn�R���������

        //    this.unitOfWork.Repository<AllotSetting>().Delete(p => p.Owner.Id == childUser.Id);
        //    this.unitOfWork.Repository<CreditWarning>().Delete(p => p.Owner.Id == childUser.Id);
        //    this.unitOfWork.Repository<ReplyCc>().Delete(p => p.Owner.Id == childUser.Id);

        //    //�O�_���ק﨤��
        //    var childUserRoleId = this.apiControllerHelper.GetRoleId(childUser.Id);
        //    var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(this.unitOfWork.DbContext));

        //    string childUserRoleName = this.apiControllerHelper.GetRoleName(childUserRoleId);
        //    if (!string.IsNullOrEmpty(childUserRoleName))
        //        userManager.RemoveFromRoleAsync(childUser.Id, childUserRoleName).Wait();

        //    this.repository.Delete(childUser);

        //    this.tradeService.DeleteChildUser(currentUser, childUser);
        //}

        protected override async Task DoRemove(string id) 
        {
            ApplicationUser entity = await DoGet(id);

            // �R�����w�b�������T�O�ӱb���ҫإߪ��Ҧ��ϥΪ̳��w�g�R��

            var childUsers = this.apiControllerHelper.GetDescendingUsers(entity);
            if (childUsers.Count != 0)
            {
                string error = string.Format("�ϥΪ̡i{0}�j�U�٦� {1} ��ϥΪ̡A���O�O {2}�A�Х��R���Ӧ��ϥΪ̤U�Ҧ��ϥΪ̡A����A�R�����ϥΪ�",
                    entity.UserName,
                    childUsers.Count,
                    string.Join("�B", childUsers.Select(p => p.UserName)));
                throw new Exception(error);
            }

            // �����ϥΪ̬������

            await context.DeleteAsync<Blacklist>(p => p.CreatedUser.Id == id);
            await context.DeleteAsync<CommonMessage>(p => p.CreatedUser.Id == id);
            await context.DeleteAsync<UploadedFile>(p => p.CreatedUser.Id == id);
            await context.DeleteAsync<Signature>(p => p.CreatedUser.Id == id);

            var sendMessageRules = context.Set<SendMessageRule>().Where(p => p.CreatedUser.Id == id);
            var sendMessageRuleIds = sendMessageRules.Select(p => p.Id);

            // �R���P SendMessageRule �̪ۨ� SendMessageQueue
            await context.DeleteAsync<SendMessageQueue>(p => sendMessageRuleIds.Contains(p.SendMessageRuleId));
            // �R�� SendMessageRule ���e�����^���I��
            sendMessageRules.ForEach(p => this.tradeService.DeleteSendMessageRule(p));
            await context.DeleteAsync<SendMessageRule>(p => p.CreatedUser.Id == id);

            var contactIds = context.Set<Contact>().Where(p => p.CreatedUser.Id == id).Select(p => p.Id);
            await context.DeleteAsync<GroupContact>(p => contactIds.Contains(p.ContactId));
            await context.DeleteAsync<Contact>(p => p.CreatedUser.Id == id);

            var groupIds = context.Set<Group>().Where(p => p.CreatedUser.Id == id).Select(p => p.Id);
            await context.DeleteAsync<GroupContact>(p => groupIds.Contains(p.GroupId));
            await context.DeleteAsync<Group>(p => p.CreatedUser.Id == id);

            await context.DeleteAsync<SharedGroupContact>(p => p.ShareToUserId == id);

            // this.unitOfWork.Repository<TradeDetail>().Delete(p => p.OwnerId == childUser.Id); // ���T�w�O�_�ݭn�R���������

            await context.DeleteAsync<AllotSetting>(p => p.Owner.Id == id);
            await context.DeleteAsync<CreditWarning>(p => p.Owner.Id == id);
            await context.DeleteAsync<ReplyCc>(p => p.Owner.Id == id);

            //�O�_���ק﨤��
            var childUserRoleId = this.apiControllerHelper.GetRoleId(id);
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));

            string childUserRoleName = this.apiControllerHelper.GetRoleName(childUserRoleId);
            if (!string.IsNullOrEmpty(childUserRoleName))
                userManager.RemoveFromRoleAsync(id, childUserRoleName).Wait();

            this.tradeService.DeleteUser(entity);

            await context.DeleteAsync<ApplicationUser>(entity);
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
                model.DepartmentId = model.Department != null ? model.Department.Id : 0;
                model.RoleId = this.apiControllerHelper.GetRoleId(model.Id);
                model.NewPassword = string.Empty;
                model.NewPasswordConfirmed = string.Empty;

                var user = DoGet(model.Id).GetAwaiter().GetResult();
                if (user != null && user.Parent != null)
                    model.CreatedUserName = user.Parent.UserName;
            }

            return models;
        }
    }
}
