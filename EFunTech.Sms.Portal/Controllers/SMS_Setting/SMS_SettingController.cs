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

                    switch (CurrentUserRole) // CurrentUserRole 不會被改變，可以使用
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

                // 點數預警設定
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

                // 開啟國際簡訊發送
                entity.ForeignSmsEnabled = model.ForeignSmsEnabled;

                // 個人資料維護
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

                //Mapper.Map(model, entity); 使用 Single Page Application 開發後，如果使用 Mapper.Map 有可能更新到畫面上沒有的欄位，例如：目前可用餘額(SmsBalance)

                using (TransactionScope scope = context.CreateTransactionScope())
                {
                    //是否有修改密碼
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
