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

        // �u��޲z�l�b��
        //protected override IOrderedQueryable<ApplicationUser> DoGetList(DepartmentManagerCriteriaModel criteria)
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
        protected override IOrderedQueryable<ApplicationUser> DoGetList(DepartmentManagerCriteriaModel criteria)
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

        protected override ApplicationUser DoGet(string id)
        {
            return this.unitOfWork.Repository<ApplicationUser>().GetById(id);
        }

        /// <summary>
        /// �إߤl�b��
        /// </summary>
        /// <param name="model"></param>
        /// <param name="entity"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        protected override ApplicationUser DoCreate(ApplicationUserModel model, ApplicationUser entity, out string id)
        {
            // �T�{�b���O�_�s�b
            if (this.unitOfWork.Repository<ApplicationUser>().Any(p => p.UserName == model.UserName))
                throw new Exception(string.Format("�b�� {0} �w�g�s�b", model.UserName));

            // ����O DepartmentHead �Ϊ� Employee�A�������w����
            var roleName = this.apiControllerHelper.GetRoleName(model.RoleId);
            Role role = (Role)Enum.Parse(typeof(Role), roleName);
            if (role == Role.DepartmentHead || role == Role.Employee)
            {
                if (model.DepartmentId == 0)
                    throw new Exception("���⬰�����D�ީέ��u�ɡA�������w����");
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

            // �إ߱b��
            var result = EFunTech.Sms.Core.AsyncHelper.RunSync<IdentityResult>(() => userManager.CreateAsync(entity, model.NewPassword));
            if (!result.Succeeded)
            {
                string errors = string.Join(",", result.Errors.ToList());
                throw new Exception(string.Format("�إߤl�b�����ѡA{0}", errors));
            }

            // �[�J�}��
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

            // �إ߹w�]�s�� [�`���p���H]
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
            // ����O DepartmentHead �Ϊ� Employee�A�������w����
            var roleName = this.apiControllerHelper.GetRoleName(model.RoleId);
            Role role = (Role)Enum.Parse(typeof(Role), roleName);
            if (role == Role.DepartmentHead || role == Role.Employee)
            {
                if (model.DepartmentId == 0)
                    throw new Exception("���⬰�����D�ީέ��u�ɡA�������w����");
            }

            //�]�w����
            entity.Department = model.DepartmentId == 0 ? null : this.unitOfWork.Repository<Department>().Get(p => p.Id == model.DepartmentId);

            //�O�_���ק﨤��
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

            //�O�_���ק�K�X
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

        protected override void DoRemove(string id, ApplicationUser entity)
        {
            var user = this.repository.GetById(id);

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

            this.unitOfWork.Repository<Blacklist>().Delete(p => p.CreatedUser.Id == id);
            this.unitOfWork.Repository<CommonMessage>().Delete(p => p.CreatedUser.Id == id);
            this.unitOfWork.Repository<UploadedFile>().Delete(p => p.CreatedUser.Id == id);
            this.unitOfWork.Repository<Signature>().Delete(p => p.CreatedUser.Id == id);

            var sendMessageRuleRepository = this.unitOfWork.Repository<SendMessageRule>();
            var sendMessageRules = sendMessageRuleRepository.GetMany(p => p.CreatedUser.Id == id);
            var sendMessageRuleIds = sendMessageRules.Select(p => p.Id);
            // �R���P SendMessageRule �̪ۨ� SendMessageQueue
            this.unitOfWork.Repository<SendMessageQueue>().Delete(p => sendMessageRuleIds.Contains(p.SendMessageRuleId));
            // �R�� SendMessageRule ���e�����^���I��
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

            // this.unitOfWork.Repository<TradeDetail>().Delete(p => p.OwnerId == childUser.Id); // ���T�w�O�_�ݭn�R���������

            this.unitOfWork.Repository<AllotSetting>().Delete(p => p.Owner.Id == id);
            this.unitOfWork.Repository<CreditWarning>().Delete(p => p.Owner.Id == id);
            this.unitOfWork.Repository<ReplyCc>().Delete(p => p.Owner.Id == id);

            //�O�_���ק﨤��
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

                model.Activatable = !isCurrentUser; // �O�_�i�H�ҥΩ�����
                model.Maintainable = true; // �O�_�i�H�ק�b���]�w
                model.Deletable = !isCurrentUser; // �O�_�i�H�R���b��
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
