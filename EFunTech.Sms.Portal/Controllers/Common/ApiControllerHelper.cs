using Microsoft.AspNet.Identity;
using EFunTech.Sms.Schema;
using JUtilSharp.Database;

using Microsoft.AspNet.Identity.EntityFramework;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace EFunTech.Sms.Portal.Controllers.Common
{
    public class ApiControllerHelper
    {
        protected IUnitOfWork unitOfWork;
        protected ILogService logService;


        public ApiControllerHelper(IUnitOfWork unitOfWork, ILogService logService)
        {
            this.unitOfWork = unitOfWork;
            this.logService = logService;
        }

        public Role GetMaxPriorityRole(ApplicationUser user)
        {
            if (user == null) return Role.Unknown;

            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(this.unitOfWork.DbContext));

            List<Role> roles = new List<Role>();
            var userRoles = user.Roles.ToList();
            foreach (var AspNetUserRole in userRoles)
            {
                var aspNetRole = roleManager.Roles.Where(AspNetRole => AspNetRole.Id == AspNetUserRole.RoleId).FirstOrDefault();
                Role aRole = Role.Unknown;
                if (Enum.TryParse<Role>(aspNetRole.Name, out aRole))
                {
                    roles.Add(aRole);
                }

            }
            if (roles.Count != 0)
            {
                var result = roles.OrderByDescending(role => (int)role).First();
                return result;
            }
            else
            {
                return Role.Unknown;
            }
        }

        public string GetRoleId(string applicationUserId)
        {
            var user = this.unitOfWork.Repository<ApplicationUser>().GetById(applicationUserId);
            if (user != null)
            {
                var role = user.Roles.FirstOrDefault();
                if (role != null)
                {
                    return role.RoleId;
                }
            }
            return null;
        }

        public string GetRoleName(string roleId)
        {
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(this.unitOfWork.DbContext));

            return roleManager.Roles.Where(role => role.Id == roleId).Select(role => role.Name).FirstOrDefault();
        }

        public List<ApplicationUser> GetDescendingUsersAndUser(ApplicationUser user)
        {
            List<ApplicationUser> result = _GetDescendingUsers(user);
            result.Add(user);
            // 由小到大排列
            return result.OrderBy(p => p.Level).ThenBy(p => p.Id).ToList();
        }

        public List<ApplicationUser> GetDescendingUsers(ApplicationUser user)
        {
            List<ApplicationUser> result = _GetDescendingUsers(user);
            // 由小到大排列
            return result.OrderBy(p => p.Level).ThenBy(p => p.Id).ToList();
        }

        private List<ApplicationUser> _GetDescendingUsers(ApplicationUser user)
        {
            var childUsers = this.unitOfWork.Repository<ApplicationUser>().GetMany(p => p.ParentId == user.Id).ToList();

            var descendingUsers = new List<ApplicationUser>();

            foreach (var childUser in childUsers)
            {
                descendingUsers.AddRange(_GetDescendingUsers(childUser));
            }

            childUsers.AddRange(descendingUsers);

            return childUsers.Distinct().ToList();
        }

        


    }
}