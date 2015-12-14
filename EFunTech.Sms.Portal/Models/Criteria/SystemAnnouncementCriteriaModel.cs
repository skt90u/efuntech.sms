using EFunTech.Sms.Portal.Models.Common;
using EFunTech.Sms.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EFunTech.Sms.Portal.Models.Criteria
{
    public class SystemAnnouncementCriteriaModel : SearchTextCriteriaModel
    {
        public DateTime StartDate { get; set; } // 發布日期起
        public DateTime EndDate { get; set; } // 發布日期迄

    }
}