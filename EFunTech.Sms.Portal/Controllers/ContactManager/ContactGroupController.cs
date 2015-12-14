using EFunTech.Sms.Portal.Models;
using EFunTech.Sms.Schema;
using System.Linq;
using EFunTech.Sms.Portal.Controllers.Common;
using EFunTech.Sms.Portal.Models.Common;
using JUtilSharp.Database;

using System.Collections.Generic;
using System;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using EFunTech.Sms.Portal.Models.Criteria;
using LinqKit;
using AutoMapper;
using System.Web.Http;
using System.Net.Http;
using System.Net;
using System.Transactions;

namespace EFunTech.Sms.Portal.Controllers
{
    /// <summary>
    /// 20151128 Norman, 似乎沒有用到，先註解掉
    /// </summary>
    //public class ContactGroupController : ApiControllerBase
    //{
    //    public ContactGroupController(IUnitOfWork unitOfWork, ILogService logService)
    //        : base(unitOfWork, logService)
    //    {
    //    }
        
    //    [System.Web.Http.HttpGet]
    //    public IEnumerable<GroupModel> GetGroups()
    //    {
    //        return Mapper.Map<IEnumerable<Group>, IEnumerable<GroupModel>>(CurrentUser.Groups);
    //    }

    //}
}
