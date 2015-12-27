using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using EFunTech.Sms.Schema;
using JUtilSharp.Database;

using Microsoft.AspNet.Identity.EntityFramework;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Data.Entity;

namespace EFunTech.Sms.Portal.Controllers.Common
{
    public abstract class ApiControllerBase : System.Web.Http.ApiController
    {
        protected ILogService logService;
        protected DbContext context;

        protected ApiControllerBase(DbContext context, ILogService logService)
        {
            this.context = context;
            this.logService = logService;
        }

        #region IdentityExtensions

        public ApplicationUser CurrentUser
        {
            get
            {
                return GetUser(User.Identity.GetUserId());
            }
        }

        private Dictionary<string, ApplicationUser> userDict;
        public ApplicationUser GetUser(string userId)
        {
            if (userDict == null)
                userDict = new Dictionary<string, ApplicationUser>();

            if (!userDict.ContainsKey(userId))
            {
                var user = IdentityExtensions.GetUser(context, userId);
                if (user != null)
                    userDict.Add(userId, user);
            }

            return userDict.ContainsKey(userId)
                ? userDict[userId]
                : null;
        }

        public IdentityRole CurrentIdentityRole
        {
            get
            {
                return GetIdentityRole(User.Identity.GetUserId());
            }
        }

        private Role _CurrentUserRole;
        public Role CurrentUserRole
        {
            get
            {
                if (_CurrentUserRole == Role.Unknown)
                {
                    if (CurrentIdentityRole != null)
                        Enum.TryParse<Role>(CurrentIdentityRole.Name, out _CurrentUserRole);
                    else
                        _CurrentUserRole = Role.Unknown;
                }
                return _CurrentUserRole;
            }
        }

        public string CurrentUserId
        {
            get
            {
                return User.Identity.GetUserId();
            }
        }

        public string CurrentUserName
        {
            get
            {
                return User.Identity.GetUserName();
            }
        }

        private Dictionary<string, IdentityRole> roleDict;
        public IdentityRole GetIdentityRole(string userId)
        {
            if (roleDict == null)
                roleDict = new Dictionary<string, IdentityRole>();

            if (!roleDict.ContainsKey(userId))
            {
                var role = IdentityExtensions.GetIdentityRole(context, userId);
                if (role != null)
                    roleDict.Add(userId, role);
            }

            return roleDict.ContainsKey(userId)
                ? roleDict[userId]
                : null;
        }

        public string GetRoleName(string roleId)
        {
            return IdentityExtensions.GetRoleName(context, roleId);
        }

        #endregion


        public TimeSpan ClientTimezoneOffset
        {
            get
            {
                TimeSpan defaultTimezoneOffset = TimeZoneInfo.Local.BaseUtcOffset;

                try
                {
                    string timeZoneString = string.Empty;

                    IEnumerable<string> values = null;
                    if (Request.Headers.TryGetValues("TimezoneOffset", out values))
                    {
                        timeZoneString = values.FirstOrDefault();
                    }
                    else
                    {
                        var ckTimezoneOffset = HttpContext.Current.Request.Cookies["TimezoneOffset"];
                        timeZoneString = System.Web.HttpUtility.UrlDecode(ckTimezoneOffset.Value);
                    }
                    
                    return !string.IsNullOrEmpty(timeZoneString)
                        // http://stackoverflow.com/questions/8491071/how-could-i-convert-timezone-string-to-timespan-and-vice-versa
                        ? TimeSpan.Parse(timeZoneString.Replace("+", ""))
                        : defaultTimezoneOffset;
                }
                catch (Exception ex)
                {
                    this.logService.Error(ex);

                    return defaultTimezoneOffset;
                }
            }
        }
    }
}