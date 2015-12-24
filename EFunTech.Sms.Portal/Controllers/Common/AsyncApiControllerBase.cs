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

        public ApplicationUser CurrentUser
        {
            get
            {
                return User.Identity.GetUser();
            }
        }

        public Role CurrentUserRole
        {
            get
            {
                return User.Identity.GetUserRole();
            }
        }

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