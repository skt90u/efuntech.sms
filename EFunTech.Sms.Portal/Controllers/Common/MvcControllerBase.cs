﻿using Microsoft.AspNet.Identity;
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
using Microsoft.AspNet.Identity.EntityFramework;
using LinqKit;

namespace EFunTech.Sms.Portal.Controllers.Common
{
    public abstract class MvcControllerBase : System.Web.Mvc.Controller
    {
        protected DbContext context;

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
            this.apiControllerHelper = new ApiControllerHelper(context, logService);
            this.validationService = new ValidationService(unitOfWork, logService);
            this.tradeService = new TradeService(unitOfWork, logService);

            this.context = this.unitOfWork.DbContext;
        }

        

        #region IdentityExtensions

        protected ApplicationUser CurrentUser
        {
            get
            {
                return GetUser(User.Identity.GetUserId());
            }
        }

        private Dictionary<string, ApplicationUser> userDict;
        protected ApplicationUser GetUser(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                return null;

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

        protected IdentityRole CurrentIdentityRole
        {
            get
            {
                return GetIdentityRole(User.Identity.GetUserId());
            }
        }

        private Role _CurrentUserRole;
        protected Role CurrentUserRole
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

        protected string CurrentUserId
        {
            get
            {
                return User.Identity.GetUserId();
            }
        }

        protected string CurrentUserName
        {
            get
            {
                return User.Identity.GetUserName();
            }
        }

        private Dictionary<string, IdentityRole> roleDict;
        protected IdentityRole GetIdentityRole(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                return null;

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

        protected string GetRoleName(string roleId)
        {
            return IdentityExtensions.GetRoleName(context, roleId);
        }

        #endregion


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

        public static Lazy<Dictionary<Role, List<MenuItemModel>>> MenuItemMap = new Lazy<Dictionary<Role, List<MenuItemModel>>>(() =>
        {
            new Startup().ConfigureMapper(null); // .Project().To<MenuItemModel>()

            var dict = new Dictionary<Role, List<MenuItemModel>>();

            using (var context = new ApplicationDbContext())
            {
                var models = context.Set<MenuItem>()
                        .Include(p => p.WebAuthorization)
                        .OrderBy(p => p.Order)
                        .Project().To<MenuItemModel>()
                        .ToList();

                foreach (Role role in Enum.GetValues(typeof(Role)))
                {
                    var menuItems = new List<MenuItemModel>();

                    foreach (var model in models)
                    {
                        if (model.Roles.Contains(role.ToString()))
                            menuItems.Add(model);
                    }

                    dict.Add(role, menuItems);
                }

                return dict;
            }
        });

        public List<MenuItemModel> GetMenuItems()
        {
            return MenuItemMap.Value[CurrentUserRole];
        }

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