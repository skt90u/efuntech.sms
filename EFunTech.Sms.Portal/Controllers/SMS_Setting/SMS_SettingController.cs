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
            var entity = await context.Set<ApplicationUser>().FindAsync(CurrentUserId);

            var model = Mapper.Map<ApplicationUser, ApplicationUserModel>(entity);

            return model;
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
                entity.CreditWarning.Enabled = model.CreditWarning.Enabled;
                entity.CreditWarning.BySmsMessage = model.CreditWarning.BySmsMessage;
                entity.CreditWarning.ByEmail = model.CreditWarning.ByEmail;
                entity.CreditWarning.SmsBalance = model.CreditWarning.SmsBalance;

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
