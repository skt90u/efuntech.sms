using EFunTech.Sms.Portal.Models.Common;
using EFunTech.Sms.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EFunTech.Sms.Portal.Models.Criteria
{
    public class SendMessageRuleCriteriaModel : SearchTextCriteriaModel
    {
        public SendTimeType SendTimeType { get; set; }
    }
}