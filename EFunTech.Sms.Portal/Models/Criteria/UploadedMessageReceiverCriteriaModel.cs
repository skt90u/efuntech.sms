using EFunTech.Sms.Portal.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EFunTech.Sms.Portal.Models.Criteria
{
    [Flags]
    public enum DataValidCondition { 
        Valid = 1 << 0, // 1
        Invalid = 1 << 1, // 2
        All = Valid | Invalid,
    }

    public class UploadedMessageReceiverCriteriaModel : SearchTextCriteriaModel
    {
        public int UploadedSessionId { get; set; }

        public DataValidCondition DataValidCondition { get; set; }
    }
}