using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EFunTech.Sms.Portal.Services
{
    public static class ExcelBugFix
    {
        public static string GetInformation(List<string> data)
        {
            var output = string.Join("、", data);

            return string.Format("共{0}筆，[{1}]",
                data.Count,
                output);
        }

    }
}