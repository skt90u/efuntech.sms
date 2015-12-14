using EFunTech.Sms.Core;
using EFunTech.Sms.Portal.Models.Common;
using EFunTech.Sms.Schema;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace EFunTech.Sms.Portal.Models.Criteria
{
    public class SectorSendMessageStatisticCriteriaModel : SearchTextCriteriaModel
    {

        public SearchType SearchType { get; set; }
        public DownloadType DownloadType { get; set; }

        public string DepartmentIds { get; set; }

        public string UserIds { get; set; }
        
        // 依發送時間
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public override string ToString()
        {
            return string.Format("SearchType: {0}, DownloadType: {1}, DepartmentIds: {2}, UserIds: {3}, StartDate: {4}, EndDate: {5}, {6}",
                AttributeHelper.GetColumnDescription(SearchType),
                AttributeHelper.GetColumnDescription(DownloadType),
                DepartmentIds,
                UserIds,
                StartDate.ToString(Converter.Every8d_SentTime),
                EndDate.ToString(Converter.Every8d_SentTime),
                base.ToString());
        }
    }
}