using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using EFunTech.Sms.Schema;
using JUtilSharp.Database;

using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using EFunTech.Sms.Portal.Models;
using System.Collections.Concurrent;
using EntityFramework.Extensions;
using EntityFramework.Caching;
using System.Data.Entity;
using AutoMapper.QueryableExtensions;

namespace EFunTech.Sms.Portal.Controllers.Common
{
    public abstract class MvcControllerBase : System.Web.Mvc.Controller
    {
        protected IUnitOfWork unitOfWork;
        protected ILogService logService;
        protected ApiControllerHelper apiControllerHelper;
        protected ValidationService validationService;
        protected TradeService tradeService;

        protected MvcControllerBase(IUnitOfWork unitOfWork, ILogService logService)
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
                return User.Identity.GetUser(unitOfWork.DbContext);
            }
        }

        public Role CurrentUserRole
        {
            get
            {
                return User.Identity.GetUserRole(unitOfWork.DbContext);
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

        /// <summary>
        /// 目前只用在上傳失敗回傳失敗原因
        /// </summary>
        /// <example>
        /// return HttpExceptionResult("上傳簡訊接收者失敗", ex);
        /// </example>
        public System.Web.Mvc.JsonResult HttpExceptionResult(string description, Exception ex)
        {
            // 原本使用以下方式回傳失敗結果，但是透會 $http 取得的失敗結果是整個失敗回報 '網頁' 而非僅僅是 ErrorMessage
            //string message = string.Format("上傳黑名單失敗，錯誤訊息：{0})", ex.Message);
            //return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, message);

            Response.StatusCode = (int)System.Net.HttpStatusCode.InternalServerError;

            string message = string.Format("{0}，錯誤訊息：{1})", description, ex.Message);

            return Json(new
            {
                ExceptionMessage = message,
                StackTrace = ex.StackTrace.ToString()
            });
        }

        public List<MenuItemModel> GetMenuItems()
        {
            using (var context = new ApplicationDbContext())
            {
                var role = CurrentUserRole;

                string roleName = role.ToString();

                //var entities = context.MenuItems
                var models = context.MenuItems
                        .Include(p => p.WebAuthorization)
                        .Where(p => p.WebAuthorization.Roles.Contains(roleName))
                        .OrderBy(p => p.Order)
                        .Project().To<MenuItemModel>()
                        //.FromCache(tags: new [] { "MenuItems", roleName })
                        .ToList();

                //List<MenuItemModel> models = Mapper.Map<List<MenuItem>, List<MenuItemModel>>(entities);

                return models;
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
        //public TimeSpan ClientTimezoneOffset
        //{
        //    get
        //    {
        //        TimeSpan defaultTimezoneOffset = TimeZoneInfo.Local.BaseUtcOffset;

        //        try
        //        {
        //            // HttpContext.Current.Request.Headers.GetValues
        //            string timeZoneString = Request.Headers.GetValues("TimezoneOffset").FirstOrDefault();

        //            return !string.IsNullOrEmpty(timeZoneString)
        //                // http://stackoverflow.com/questions/8491071/how-could-i-convert-timezone-string-to-timespan-and-vice-versa
        //                ? TimeSpan.Parse(timeZoneString.Replace("+", ""))
        //                : defaultTimezoneOffset;
        //        }
        //        catch (Exception ex)
        //        {
        //            this.logService.Error(ex);

        //            return defaultTimezoneOffset;
        //        }
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

                    IEnumerable<string> values = Request.Headers.GetValues("TimezoneOffset");
                    if (values != null)
                    {
                        timeZoneString = values.FirstOrDefault();
                    }
                    else
                    {
                        var ckTimezoneOffset = Request.Cookies["TimezoneOffset"];
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