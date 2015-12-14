using EFunTech.Sms.Portal.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EFunTech.Sms.Portal.Models.Criteria
{
    public class GroupCriteriaModel : SearchTextCriteriaModel
    {
        public bool IncludeAllGroup { get; set; }
    }
}