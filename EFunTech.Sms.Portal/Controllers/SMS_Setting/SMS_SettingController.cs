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

                Mapper.Map(model, entity);

                using (TransactionScope scope = context.CreateTransactionScope())
                {
                    //¬O§_¦³­×§ï±K½X
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
