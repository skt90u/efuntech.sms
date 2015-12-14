using EFunTech.Sms.Core;
using EFunTech.Sms.Portal.Models.Common;
using EFunTech.Sms.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EFunTech.Sms.Portal.Models.Criteria
{
    public class SectorSendMessageHistoryCriteriaModel : SearchTextCriteriaModel
    {
        public string UserId { get; set; }

        // 依發送時間
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public override string ToString()
        {
            return string.Format("UserId: {0}, StartDate: {1}, EndDate: {2}, {3}",
                UserId,
                StartDate.ToString(Converter.Every8d_SentTime),
                EndDate.ToString(Converter.Every8d_SentTime),
                base.ToString());
        }
    }
}

