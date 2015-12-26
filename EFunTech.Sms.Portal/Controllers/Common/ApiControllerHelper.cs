using EFunTech.Sms.Schema;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using EntityFramework.Extensions;
using EntityFramework.Caching;

namespace EFunTech.Sms.Portal.Controllers.Common
{
    public class ApiControllerHelper
    {
        protected DbContext context;
        protected ILogService logService;


        public ApiControllerHelper(DbContext context, ILogService logService)
        {
            this.context = context;
            this.logService = logService;
        }

        //public Role GetMaxPriorityRole(ApplicationUser user)
        //{
        //    if (user == null) return Role.Unknown;

        //    var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(this.unitOfWork.DbContext));

        //    List<Role> roles = new List<Role>();
        //    var userRoles = user.Roles.ToList();
        //    foreach (var AspNetUserRole in userRoles)
        //    {
        //        var aspNetRole = roleManager.Roles.Where(AspNetRole => AspNetRole.Id == AspNetUserRole.RoleId).FirstOrDefault();
        //        Role aRole = Role.Unknown;
        //        if (Enum.TryParse<Role>(aspNetRole.Name, out aRole))
        //        {
        //            roles.Add(aRole);
        //        }

        //    }
        //    if (roles.Count != 0)
        //    {
        //        var result = roles.OrderByDescending(role => (int)role).First();
        //        return result;
        //    }
        //    else
        //    {
        //        return Role.Unknown;
        //    }
        //}

        //public string GetRoleId(string applicationUserId)
        //{
        //    var user = this.unitOfWork.Repository<ApplicationUser>().GetById(applicationUserId);
        //    if (user != null)
        //    {
        //        var role = user.Roles.FirstOrDefault();
        //        if (role != null)
        //        {
        //            return role.RoleId;
        //        }
        //    }
        //    return null;
        //}

        //public string GetRoleName(string roleId)
        //{
        //    var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(this.unitOfWork.DbContext));

        //    return roleManager.Roles.Where(role => role.Id == roleId).Select(role => role.Name).FirstOrDefault();
        //}

        private IEnumerable<ApplicationUser> GetAllUsers()
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
                   //.FromCache(CachePolicy.WithDurationExpiration(TimeSpan.FromSeconds(5)), tags: new[] { "ApplicationUsers" })
                   ;
        }

        private IEnumerable<ApplicationUser> _GetDescendingUsers(IEnumerable<ApplicationUser> users, ApplicationUser user)
        {
            var childUsers = users.Where(p => p.ParentId == user.Id);

            foreach (var childUser in childUsers)
            {
                childUsers = childUsers.Union(_GetDescendingUsers(users, childUser));
            }

            return childUsers.Distinct();
        }

        public IEnumerable<ApplicationUser> GetDescendingUsersAndUser(ApplicationUser user)
        {
            var users = GetAllUsers().ToList();

            var result = _GetDescendingUsers(users, user);

            result = result.Union(new List<ApplicationUser> { user });

            // 由小到大排列
            return result.OrderBy(p => p.Level).ThenBy(p => p.Id);
        }

        public IEnumerable<ApplicationUser> GetDescendingUsers(ApplicationUser user)
        {
            var users = GetAllUsers().ToList();

            var result = _GetDescendingUsers(users, user);

            // 由小到大排列
            return result.OrderBy(p => p.Level).ThenBy(p => p.Id);
        }

        

      
    }
}