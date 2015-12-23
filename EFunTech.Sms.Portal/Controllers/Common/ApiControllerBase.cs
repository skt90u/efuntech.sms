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

namespace EFunTech.Sms.Portal.Controllers.Common
{
    public abstract class ApiControllerBase : System.Web.Http.ApiController
    {
        protected IUnitOfWork unitOfWork;
        protected ILogService logService;
        protected ApiControllerHelper apiControllerHelper;
        protected ValidationService validationService;
        protected TradeService tradeService;
        
        protected ApiControllerBase(IUnitOfWork unitOfWork, ILogService logService)
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");
            System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("en-US");

            this.unitOfWork = unitOfWork;
            this.logService = logService;
            this.apiControllerHelper = new ApiControllerHelper(unitOfWork, logService);
            this.validationService = new ValidationService(unitOfWork, logService);
            this.tradeService = new TradeService(unitOfWork, logService);
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

        //private TimeSpan _ClientTimezoneOffset = TimeSpan.Zero;
        //public TimeSpan ClientTimezoneOffset
        //{
        //    get
        //    {
        //        if (_ClientTimezoneOffset == TimeSpan.Zero)
        //        {
        //            TimeSpan defaultTimezoneOffset = TimeZoneInfo.Local.BaseUtcOffset;

        //            try
        //            {
        //                string timeZoneString = Request.Headers.GetValues("TimezoneOffset").FirstOrDefault();

        //                _ClientTimezoneOffset = !string.IsNullOrEmpty(timeZoneString)
        //                    // http://stackoverflow.com/questions/8491071/how-could-i-convert-timezone-string-to-timespan-and-vice-versa
        //                    ? TimeSpan.Parse(timeZoneString.Replace("+", ""))
        //                    : defaultTimezoneOffset;
        //            }
        //            catch (Exception ex)
        //            {
        //                this.logService.Error(ex);

        //                _ClientTimezoneOffset = defaultTimezoneOffset;
        //            }

        //        }

        //        return _ClientTimezoneOffset;
        //    }
        //}
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

        ////////////////////////////////////////

        

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

        
    }
}