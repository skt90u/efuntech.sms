using EFunTech.Sms.Portal.Models;
using EFunTech.Sms.Schema;

using JUtilSharp.Database;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using EFunTech.Sms.Portal.Controllers.Common;
using System.Data.Entity;

namespace EFunTech.Sms.Portal.Controllers
{
    /// <summary>
    /// 專門用來驗證使用者輸入
    /// </summary>
    public class ValidationApiController : ApiControllerBase
    {
        public ValidationApiController(DbContext context, ILogService logService)
            : base(context, logService)
        {
        }

        [HttpGet]
        [Route("api/ValidationApi/MakeSureUserNameNotExists")]
        public bool MakeSureUserNameNotExists(string UserName)
        {
            if(context.Set<ApplicationUser>().Any(p => p.UserName == UserName))
            {
                throw new Exception(string.Format("帳號 {0} 已經存在", UserName));
            }

            return true;
        }

      

        

    }
}
