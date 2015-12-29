using EFunTech.Sms.Schema;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using EntityFramework.Extensions;
using EntityFramework.Caching;

namespace EFunTech.Sms.Portal.Controllers.Common
{
    public class ApiControllerHelper
    {
        protected DbContext context;
        protected ILogService logService;


        public ApiControllerHelper(DbContext context, ILogService logService)
        {
            this.context = context;
            this.logService = logService;
        }

        public IEnumerable<ApplicationUser> GetDescendingUsersAndUser(string userId)
        {
            return IdentityExtensions.GetDescendingUsersAndUser(context, userId);
        }

        public IEnumerable<ApplicationUser> GetDescendingUsers(string userId)
        {
            return IdentityExtensions.GetDescendingUsers(context, userId);
        }
      
    }
}