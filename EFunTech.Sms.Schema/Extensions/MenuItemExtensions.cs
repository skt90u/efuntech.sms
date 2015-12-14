using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFunTech.Sms.Schema
{
    public static class MenuItemExtensions
    {
        public static string GetHref(this MenuItem menuItem)
        {
            return string.Format("/{0}/{1}", menuItem.WebAuthorization.ControllerName, menuItem.WebAuthorization.ActionName);
        }
    }
}
