using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EFunTech.Sms.Core
{
    public static class GrottyHacks
    {
        public static T Cast<T>(object target, T example)
        {
            return (T)target;
        }
    }
}