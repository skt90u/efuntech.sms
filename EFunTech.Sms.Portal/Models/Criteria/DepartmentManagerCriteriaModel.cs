using EFunTech.Sms.Portal.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EFunTech.Sms.Portal.Models.Criteria
{
    public class DepartmentManagerCriteriaModel : SearchTextCriteriaModel
    {
        // 姓名
        public string FullName { get; set; }

        // 帳號
        public string UserName { get; set; }
    }
}