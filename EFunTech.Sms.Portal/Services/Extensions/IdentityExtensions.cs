using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using Microsoft.AspNet.Identity;
using JUtilSharp.Database;
using EFunTech.Sms.Schema;

namespace EFunTech.Sms.Portal.Identity
{
    public static class IdentityExtensions
    {
        public static string GetSmsBalance(this IIdentity identity)
        {
            using(IUnitOfWork unitOfWork = new UnitOfWork(new ApplicationDbContext())){
                
                string userId = identity.GetUserId();

                if(string.IsNullOrEmpty(userId))
                    return "None1";

                ApplicationUser user = unitOfWork.Repository<ApplicationUser>().GetById(userId);

                if(user == null)
                    return "None2";

                return string.Format("{0:0.0}", user.SmsBalance);
            }
        }
    }
}