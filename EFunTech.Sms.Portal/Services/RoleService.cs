using EFunTech.Sms.Schema;
using JUtilSharp.Database;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Claims;
using System.Web;

namespace EFunTech.Sms.Portal.Services
{
    /// <summary>
    /// 模擬 System.Web.Security.Roles 提供的方法
    /// 
    /// 在 web.config又試不出怎麼使用他，因此自己寫一個對應的功能
    /// 
    /// http://thebojan.ninja/2015/03/12/custom-role-provider/
    /// http://www.codeproject.com/Articles/607392/Custom-Role-Providers
    /// 
    /// TODO: 待完成
    /// </summary>
    public class RoleService
    {
        private IUnitOfWork unitOfWork;
        private ILogService logService;
        private DbContext context
        {
            get
            {
                return unitOfWork.DbContext;
            }
        }

        private RoleManager<IdentityRole> _roleManager;
        RoleManager<IdentityRole> roleManager
        {
            get { 
                if(_roleManager == null)
                {
                    _roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
                }
                return _roleManager;
            }
        }

        private UserManager<IdentityUser> _userManager;
        UserManager<IdentityUser> userManager
        {
            get
            {
                if (_userManager == null)
                {
                    _userManager = new UserManager<IdentityUser>(new UserStore<IdentityUser>(context));
                }
                return _userManager;
            }
        }

        public RoleService(IUnitOfWork unitOfWork, ILogService logService)
        {
            this.unitOfWork = unitOfWork;
            this.logService = logService;
        }

        

        public IdentityResult AddUserToRole(string username, string roleName)
        {
            IdentityUser user = userManager.FindByName(username);

            return userManager.AddToRole(user.Id, roleName);
        }

        #region RoleManagerExtensions
        
        public IdentityResult CreateRole(string roleName)
        {
            if (RoleExists(roleName)) return null;
            IdentityResult.Failed(string.Format("角色{0}已經存在", roleName));
            var role = new IdentityRole(roleName);

            IdentityResult result = roleManager.Create(role);

            return result;
        }

        public IdentityResult DeleteRole(string roleName, bool throwOnPopulatedRole)
        {
            if (!RoleExists(roleName)) return null;

            IdentityRole role = roleManager.FindByName(roleName);

            IdentityResult result = roleManager.Delete(role);

            return result;
        }

        public bool RoleExists(string roleName)
        {
            return roleManager.RoleExists(roleName);
        }

        #endregion

        public string[] FindUsersInRole(string roleName, string usernameToMatch)
        {
            throw new NotImplementedException();
        }

        public IQueryable<IdentityRole> GetAllRoles()
        {
            return roleManager.Roles.AsQueryable();
        }

        //public static List<string> Roles(this ClaimsIdentity identity)
        //{
        //    return identity.Claims
        //                   .Where(c => c.Type == ClaimTypes.Role)
        //                   .Select(c => c.Value)
        //                   .ToList();
        //}

        public string[] GetRolesForUser(string username)
        {
            throw new NotImplementedException();
        }

        public string[] GetUsersInRole(string roleName)
        {
            throw new NotImplementedException();
        }

        public bool IsUserInRole(string username, string roleName)
        {
            throw new NotImplementedException();
        }

        public IdentityResult RemoveUserFromRole(string username, string roleName)
        {
            IdentityUser user = userManager.FindByName(username);

            return userManager.RemoveFromRole(user.Id, roleName);
        }

        ////////////////////////////////////////

        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public IdentityResult CreateUser(IdentityUser user, string password)
        {
            return userManager.Create(user, password);
        }

        public IdentityResult DeleteUser(IdentityUser user)
        {
            return userManager.Delete(user);
        }
    }
}