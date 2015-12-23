using EFunTech.Sms.Schema;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EntityFramework.Extensions;
using EntityFramework.Caching;
using System.Security.Principal;
using System.Data.Entity;

namespace EFunTech.Sms.Portal
{
    public static class IdentityExtensions
    {
        public static Role GetUserRole(this IIdentity identity)
        {
            using (var context = new ApplicationDbContext())
            {
                var identityUsers = context.Users
                    .Include(p => p.Roles)
                    //.FromCache(tags: new[] { "GetUserRole-IdentityUser" })
                    .ToList();

                string userId = identity.GetUserId();
                var user = identityUsers.FirstOrDefault(p => p.Id == userId);
                if (user == null)
                    return Role.Unknown;

                IdentityUserRole role = user.Roles.FirstOrDefault();
                if (role == null)
                    return Role.Unknown;

                // 使用快取機制避免不太會異動的資料，重複性的被查詢
                //
                // 如需要取消快取時，請呼叫以下
                // CacheManager.Current.Expire("ApplicationUser");
                var identityRoles = context.Roles
                    .FromCache(tags: new[] { "GetUserRole-IdentityRole" })
                    .ToList();

                var identityRole = identityRoles.FirstOrDefault(p => p.Id == role.RoleId);
                if (identityRole == null)
                    return Role.Unknown;
                else
                {
                    Role result = Role.Unknown;

                    if (Enum.TryParse<Role>(identityRole.Name, out result))
                    {
                        return result;
                    }
                    else
                    {
                        return Role.Unknown;
                    }
                }
            }
        }

        public static ApplicationUser GetUser(this IIdentity identity)
        {
            using (var context = new ApplicationDbContext())
            {
                var identityUsers = context.Users
                   .Include(p => p.Roles)
                   .FromCache(tags: new[] { "GetUser-IdentityUser" })
                   .ToList();

                string userId = identity.GetUserId();
                return identityUsers.FirstOrDefault(p => p.Id == userId);
            }
        }

        
    }
}