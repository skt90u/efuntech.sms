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
using AutoMapper;
using System.Web.Http;
using System.Net.Http;
using System.Net;
using System.Transactions;

namespace EFunTech.Sms.Portal.Controllers
{
    public class SMS_SettingController : ApiControllerBase
    {
        public SMS_SettingController(IUnitOfWork unitOfWork, ILogService logService)
            : base(unitOfWork, logService)
        {
        }

        [System.Web.Http.HttpGet]
        public ApplicationUserModel GetCurrentUser()
        {
            var user = this.unitOfWork.Repository<ApplicationUser>().GetById(CurrentUserId);

            var model = Mapper.Map<ApplicationUser, ApplicationUserModel>(user);

            return model;
        }

        [System.Web.Http.HttpPut]
        public HttpResponseMessage UpdateCurrentUser(string id, [FromBody] ApplicationUserModel model)
        {
            try
            {
                ApplicationUser entity = this.unitOfWork.Repository<ApplicationUser>().GetById(model.Id);
                if (entity == null)
                {
                    throw new HttpResponseException(HttpStatusCode.NotFound);
                }

                Mapper.Map(model, entity);

                using (TransactionScope scope = this.unitOfWork.CreateTransactionScope())
                {
                    //¬O§_¦³­×§ï±K½X
                    if (!string.IsNullOrEmpty(model.NewPassword))
                    {
                        // http://stackoverflow.com/questions/19524111/asp-net-identity-reset-password
                        var store = new UserStore<ApplicationUser>(this.unitOfWork.DbContext);
                        var userManager = new UserManager<ApplicationUser>(store);
                        var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(this.unitOfWork.DbContext));

                        String hashedNewPassword = userManager.PasswordHasher.HashPassword(model.NewPassword);
                        var user = this.unitOfWork.Repository<ApplicationUser>().GetById(CurrentUserId);

                        EFunTech.Sms.Core.AsyncHelper.RunSync(() => store.SetPasswordHashAsync(user, hashedNewPassword));
                        EFunTech.Sms.Core.AsyncHelper.RunSync(() => store.UpdateAsync(user));
                    }

                    this.unitOfWork.Repository<ApplicationUser>().Update(entity);
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
