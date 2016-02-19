using System;
using System.IO;
using System.Security.Cryptography;
using System.Web;
using System.Web.Mvc;

namespace EFunTech.Sms.Portal.Filters
{
    /// <summary>
    /// http://stackoverflow.com/questions/6642815/create-etag-filter-in-asp-net-mvc
    /// </summary>
    [Obsolete("20151015 Norman, 僅供參考，目前沒有在專案中使用")]
    public class ETagAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            try
            {
                filterContext.HttpContext.Response.Filter = new ETagFilter(filterContext.HttpContext.Response);
            }
            catch
            {
                // Do Nothing
            }
        }
    }

    public class ETagFilter : MemoryStream
    {
        private HttpResponseBase o = null;
        private Stream filter = null;

        public ETagFilter(HttpResponseBase response)
        {
            o = response;
            filter = response.Filter;
        }

        private string GetToken(Stream stream)
        {
            byte[] checksum = new byte[0];
            checksum = MD5.Create().ComputeHash(stream);
            return Convert.ToBase64String(checksum, 0, checksum.Length);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            byte[] data = new byte[count];
            Buffer.BlockCopy(buffer, offset, data, 0, count);
            filter.Write(data, 0, count);
            o.AddHeader("ETag", GetToken(new MemoryStream(data)));
        }
    }
}