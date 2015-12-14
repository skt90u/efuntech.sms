using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using EFunTech.Sms.Schema;
using System.Web.Mvc;
using Microsoft.Practices.Unity;
using JUtilSharp.Database;

namespace EFunTech.Sms.Portal.Filters
{
    /// <summary>
    /// 將 System.Web.Mvc.AuthorizeAttribute 的角色以及使用者參數來源，改由資料庫載入
    /// 
    /// 參考資料：
    ///     System.Web.Mvc.AuthorizeAttribute.cs 程式碼
    ///     https://github.com/ASP-NET-MVC/aspnetwebstack/blob/master/src/System.Web.Mvc/AuthorizeAttribute.cs
    /// </summary>
    public class DbAuthorizeAttribute : System.Web.Mvc.AuthorizeAttribute
    {
        private void LoadData(string ControllerName, string ActionName)
        {
            using (var context = new ApplicationDbContext())
            {
                WebAuthorization result = null;

                if(result == null)
                {
                    result = context.WebAuthorizations.Where(x => 
                        x.ControllerName == ControllerName &&
                        x.ActionName == ActionName).FirstOrDefault();
                }

                if (result == null)
                {
                    result = context.WebAuthorizations.Where(x => 
                        x.ControllerName == ControllerName &&
                       (x.ActionName == string.Empty || x.ActionName == null)).FirstOrDefault();
                }

                if (result != null)
                {
                    this.Roles = result.Roles ?? string.Empty;
                    this.Users = result.Users ?? string.Empty;
                }
            }
        }

        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            LoadData(filterContext.ActionDescriptor.ControllerDescriptor.ControllerName, 
                     filterContext.ActionDescriptor.ActionName);

            base.OnAuthorization(filterContext);
        }
    }
}