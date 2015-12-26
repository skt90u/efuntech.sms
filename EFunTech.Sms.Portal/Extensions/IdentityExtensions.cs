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
        public static ApplicationUser GetUser(DbContext context, string userId)
        {
            return context.Set<ApplicationUser>()
                   .Include(p => p.Parent)
                   .Include(p => p.Department)
                   .Include(p => p.CreditWarning)
                   .Include(p => p.ReplyCc)
                   .Include(p => p.Blacklists)
                   .Include(p => p.CommonMessages)
                   .Include(p => p.UploadedFiles)
                   .Include(p => p.Signatures)
                   .Include(p => p.SendMessageRules)
                   .Include(p => p.Contacts)
                   .Include(p => p.Groups)
                   .Include(p => p.AllotSetting)
                   .Include(p => p.Claims)
                   .Include(p => p.Logins)
                   .Include(p => p.Roles)
                   //.FromCache(CachePolicy.WithDurationExpiration(TimeSpan.FromSeconds(30)), tags: new[] { "ApplicationUsers" })
                   .FirstOrDefault(p => p.Id == userId);
        }

        public static IdentityRole GetIdentityRole(DbContext context, string userId)
        {
            var user = GetUser(context, userId);
            if (user == null)
                return null;

            var role = user.Roles.FirstOrDefault();

            if (role == null)
                throw new Exception(string.Format("指定使用者 ({0}) 尚未指定任何角色", userId));

            var identityRole = context.Set<IdentityRole>()
                .FromCache(tags: new[] { "IdentityRoles" })
                .FirstOrDefault(p => p.Id == role.RoleId);

            return identityRole;
        }

        public static string GetRoleName(DbContext context, string roleId)
        {
            var identityRole = context.Set<IdentityRole>()
                .FromCache(tags: new[] { "IdentityRoles" })
                .FirstOrDefault(p => p.Id == roleId);

            return identityRole == null
                ? null
                : identityRole.Name;
        }
    }
}