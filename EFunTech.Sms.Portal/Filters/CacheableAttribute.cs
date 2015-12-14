using EFunTech.Sms.Portal.Controllers.Common;
using EFunTech.Sms.Portal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EFunTech.Sms.Portal.Filters
{
    /// <summary>
    /// ADD AN EXPIRES OR A CACHE-CONTROL HEADER
    /// 
    /// http://thomasardal.com/yslow-rule-3-add-an-expires-or-a-cache-control-header/
    /// </summary>
    [Obsolete("20151015 Norman, 僅供參考，目前沒有在專案中使用")]
    public class CacheableAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            HttpCachePolicyBase cache = filterContext.HttpContext.Response.Cache;

            // 在關閉瀏覽器或者切換使用者時，刪除快取資料 (http://blog.toright.com/posts/3414/初探-http-1-1-cache-機制.html)
            cache.SetCacheability(HttpCacheability.Public);

            // 在 http response header 裡設 expire time 減少 client 送出 http request (http://fcamel-life.blogspot.tw/2010/06/http-response-header-expire-request.html)
            cache.SetExpires(DateTime.Now.AddDays(365)); // HTTP/1.0
            // filterContext.HttpContext.Response.Cache.SetValidUntilExpires(true);

            cache.SetMaxAge(TimeSpan.FromDays(365)); // HTTP/1.1, Cache-Control: max-age
            cache.SetProxyMaxAge(TimeSpan.FromDays(365)); // HTTP/1.1, Cache-Control: s-maxage

            cache.SetETagFromFileDependencies();
            cache.SetLastModifiedFromFileDependencies();
            cache.SetAllowResponseInBrowserHistory(true);

            // http://stackoverflow.com/questions/6151292/httpcontext-current-response-cache-setcacheabilityhttpcacheability-nocache-not
            // 按下登出按鈕所需要的快取設定
            //cache.SetCacheability(HttpCacheability.NoCache);
            //cache.SetExpires(DateTime.UtcNow.AddHours(-1));
            //cache.SetNoStore();

            // http://stackoverflow.com/questions/4078011/how-to-switch-off-caching-for-mvc-requests-but-not-for-static-files-in-iis7
            // 關閉 Cache 功能 
            //cache.SetExpires(DateTime.UtcNow.AddDays(-1));
            //cache.SetValidUntilExpires(false);
            //cache.SetRevalidation(HttpCacheRevalidation.AllCaches);
            //cache.SetCacheability(HttpCacheability.NoCache);
            //cache.SetNoStore();
        }
    }
}