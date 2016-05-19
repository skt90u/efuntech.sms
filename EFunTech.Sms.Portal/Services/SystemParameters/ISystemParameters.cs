
using EFunTech.Sms.Schema;

using JUtilSharp.Database;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFunTech.Sms.Portal
{
    public interface ISystemParameters
    {
        string EMailUserName { get; set; } 
        string EMailPassword { get; set; }

        string InfobipNormalQualityUserName { get; set; }
        string InfobipNormalQualityPassword { get; set; }

        string InfobipHighQualityUserName { get; set; }
        string InfobipHighQualityPassword { get; set; }


        string Every8dUserName { get; set; }
        string Every8dPassword { get; set; }

        int RetryMaxTimes { get; set; }

        int ExpireTimeSpan { get; set; }
        int ValidateInterval { get; set; }

        // 聯絡人只能對應至一個群組
        bool ContactAtMostOneGroup { get; set; }

        bool AllowSendMessage { get; set; }
    }
}
