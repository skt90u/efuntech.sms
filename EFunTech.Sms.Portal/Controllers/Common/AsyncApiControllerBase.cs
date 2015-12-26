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
    public abstract class AsyncApiControllerBase : System.Web.Http.ApiController
    {
        protected ILogService logService;
        protected DbContext context;

        protected AsyncApiControllerBase(DbContext context, ILogService logService)
        {
            this.context = context;
            this.logService = logService;
        }

        #region IdentityExtensions

        private ApplicationUser _CurrentUser;
        public ApplicationUser CurrentUser
        {
            get
            {
                if (_CurrentUser == null)
                {
                    _CurrentUser = IdentityExtensions.GetUser(context, User.Identity.GetUserId());
                }
                return _CurrentUser;
            }
        }

        public ApplicationUser GetUser(string userId)
        {
            return IdentityExtensions.GetUser(context, userId);
        }

        private IdentityRole _CurrentIdentityRole;
        public IdentityRole CurrentIdentityRole
        {
            get
            {
                if (_CurrentIdentityRole == null)
                {
                    _CurrentIdentityRole = GetIdentityRole(User.Identity.GetUserId());
                }
                return _CurrentIdentityRole;
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
                return CurrentUser.Id;
            }
        }

        public string CurrentUserName
        {
            get
            {
                return CurrentUser.UserName;
            }
        }

        public IdentityRole GetIdentityRole(string userId)
        {
            return IdentityExtensions.GetIdentityRole(context, userId);
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