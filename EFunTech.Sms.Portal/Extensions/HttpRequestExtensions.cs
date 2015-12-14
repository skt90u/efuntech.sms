using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace EFunTech.Sms.Portal.Extensions
{
    /// <summary>
    /// http://madskristensen.net/post/get-language-and-country-from-a-browser-in-aspnet
    /// 
    /// HttpContext.Current.Request
    /// </summary>
    public static class HttpRequestExtensions
    {
        public static CultureInfo GetCultureInfo(this HttpRequest request)
        {
            string[] languages = request.UserLanguages;

            if (languages == null || languages.Length == 0)
                return null;

            try
            {
                string language = languages[0].ToLowerInvariant().Trim();
                return CultureInfo.CreateSpecificCulture(language);
            }
            catch (ArgumentException)
            {
                return null;
            }
        }

        public static RegionInfo GetRegionInfo(this HttpRequest request)
        {
            CultureInfo culture = request.GetCultureInfo();

            if (culture != null)
                return new RegionInfo(culture.LCID);

            return null;
        }
    }
}