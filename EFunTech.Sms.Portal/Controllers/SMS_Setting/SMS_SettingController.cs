using EFunTech.Sms.Portal.Models;
using EFunTech.Sms.Schema;
using EFunTech.Sms.Portal.Controllers.Common;
using System;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using AutoMapper;
using System.Web.Http;
using System.Net.Http;
using System.Net;
using System.Transactions;
using System.Data.Entity;
using System.Threading.Tasks;

namespace EFunTech.Sms.Portal.Controllers
{
    public class SMS_SettingController : ApiControllerBase
    {
        public SMS_SettingController(DbContext context, ILogService logService)
            : base(context, logService)
        {
        }

        [System.Web.Http.HttpGet]
        public async Task<ApplicationUserModel> GetCurrentUser()
        {
            try
            {
                var entity = await context.Set<ApplicationUser>().FindAsync(CurrentUserId);

                var model = Mapper.Map<ApplicationUser, ApplicationUserModel>(entity);

                if (model != null)
                {
                    model.CanEditDepartment = false;
                    model.CanEditSmsProviderType = false;

                    switch (CurrentUserRole) // CurrentUserRole ���|�Q���ܡA�i�H�ϥ�
                    {
                        case Role.Administrator:
                            model.CanEditDepartment = true;
                            model.CanEditSmsProviderType = true;
                            break;
                        case Role.Supervisor:
                            model.CanEditDepartment = true;
                            break;
                    }
                }

                return model;
            }
            catch (Exception ex)
            {
                logService.Error(ex);
                return null;
            }
        }

        [System.Web.Http.HttpPut]
        public async Task<HttpResponseMessage> UpdateCurrentUser(string id, [FromBody] ApplicationUserModel model)
        {
            try
            {
                var entity = await context.Set<ApplicationUser>().FindAsync(CurrentUserId);

                if (entity == null)
                {
                    throw new HttpResponseException(HttpStatusCode.NotFound);
                }

                // �I�ƹwĵ�]�w
                if(model.CreditWarning != null)
                {
                    entity.CreditWarning = entity.CreditWarning ?? new CreditWarning();
                    entity.CreditWarning.Enabled = model.CreditWarning.Enabled;
                    entity.CreditWarning.BySmsMessage = model.CreditWarning.BySmsMessage;
                    entity.CreditWarning.ByEmail = model.CreditWarning.ByEmail;
                    entity.CreditWarning.SmsBalance = model.CreditWarning.SmsBalance;
                }
                
                //CreditWarning creditWarning = new CreditWarning
                //{
                //    Enabled = CreditWarning.DefaultValue_Enabled,
                //    BySmsMessage = CreditWarning.DefaultValue_BySmsMessage,
                //    ByEmail = CreditWarning.DefaultValue_ByEmail,
                //    LastNotifiedTime = null,
                //    NotifiedInterval = CreditWarning.DefaultValue_NotifiedInterval,
                //    Owner = user,
                //};
                //context.CreditWarnings.Add(creditWarning);
                //context.SaveChanges();

                // �}�Ұ��²�T�o�e
                entity.ForeignSmsEnabled = model.ForeignSmsEnabled;

                // �ӤH��ƺ��@
                entity.FullName = model.FullName;
                entity.PhoneNumber = model.PhoneNumber;
                entity.ContactPhone = model.ContactPhone;
                entity.ContactPhoneExt = model.ContactPhoneExt;
                entity.Email = model.Email;
                entity.Gender = model.Gender;
                entity.AddressCountry = model.AddressCountry;
                entity.AddressArea = model.AddressArea;
                entity.AddressZip = model.AddressZip;
                entity.AddressStreet = model.AddressStreet;
                entity.SmsProviderType = model.SmsProviderType;

                //Mapper.Map(model, entity); �ϥ� Single Page Application �}�o��A�p�G�ϥ� Mapper.Map ���i���s��e���W�S�������A�Ҧp�G�ثe�i�ξl�B(SmsBalance)

                using (TransactionScope scope = context.CreateTransactionScope())
                {
                    //�O�_���ק�K�X
                    if (!string.IsNullOrEmpty(model.NewPassword))
                    {
                        // http://stackoverflow.com/questions/19524111/asp-net-identity-reset-password
                        var store = new UserStore<ApplicationUser>(context);
                        var userManager = new UserManager<ApplicationUser>(store);
                        var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));

                        String hashedNewPassword = userManager.PasswordHasher.HashPassword(model.NewPassword);

                        await store.SetPasswordHashAsync(entity, hashedNewPassword);
                        await store.UpdateAsync(entity);
                    }

                    await context.UpdateAsync(entity);
                    scope.Complete();
                }

                return Request.CreateResponse(HttpStatusCode.OK);

            }
            catch (Exception ex)
            {
                this.logService.Error(ex);

                throw;
            }
        }

        
    }
}
