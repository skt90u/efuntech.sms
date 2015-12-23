using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EFunTech.Sms.Portal
{
    public static class DateTimeExtensions
    {
        public static DateTime DateEnd(this DateTime @this)
        {
            return @this.Date.AddDays(1).AddSeconds(-1);
        }
    }
}