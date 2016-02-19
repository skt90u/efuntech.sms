using EFunTech.Sms.Portal.Filters;
using System;
using System.Web;
using System.Web.Mvc;
using WebMarkupMin.Mvc.ActionFilters;

namespace EFunTech.Sms.Portal
{
    public static class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            //filters.Add(new DbAuthorizeAttribute());
            filters.Add(new UserMenuActionFilter());
            
            // 去掉多餘內容
            // 1. HTML
            // 2. CSS (不確定是否會與 BundleConfig 衝突)
            // 3. JS  (不確定是否會與 BundleConfig 衝突)
            // 
            // 設定方式：
            // (1) Web.config
            // https://webmarkupmin.codeplex.com/wikipage?title=WebMarkupMin%201.0.0
            // (2) 打開以下註解 ... MinifyHtmlAttribute ...
            //  filters.Add(new MinifyHtmlAttribute());
             
            // filters.Add(new JsonExceptionFilterAttribute()); // 20150912 Norman, 還沒測試完成

#if(DEBUG)
            // 取消所有快取
            //GlobalFilters.Filters.Add(new OutputCacheAttribute { NoStore = true, Duration = 0, VaryByParam = "*" });
#else

#endif
            // 20151013 Norman, 目前暫時使用 web.config 中設置 staticContent
            // 使用OutputCacheAttribute， 除非全部都使用 ajax 動態抓取資料，否則快取頁面反而會造成災難
            // CacheableAttribute 目前控制上還不太會用，要需要研究一下
            /*
            // 參考資料：
            //  https://github.com/mono/aspnetwebstack/blob/master/src/System.Web.Mvc/OutputCacheAttribute.cs
            //  https://msdn.microsoft.com/zh-tw/library/system.web.ui.outputcachelocation(v=vs.110).aspx
            //  http://blog.toright.com/posts/3414/初探-http-1-1-cache-機制.html
            //  http://forums.asp.net/t/1669662.aspx?error+trying+to+globally+register+output+cache+attribute+via+Global+asax
            // 從程式碼觀察，看起來是Server端快取
            // TODO: 使用了CacheableAttribute，不確定是否還需要OutputCacheAttribute，需再測試看看
            OutputCacheAttribute filterCache = new OutputCacheAttribute();
            filterCache.Location = System.Web.UI.OutputCacheLocation.Any; // Cache-Control: public,
            filterCache.Duration = (int)TimeSpan.FromDays(365).TotalSeconds;
            filters.Add(filterCache);

            // 自訂與Cache相關的Http Header (控制 Client 端 快取)
            filters.Add(new CacheableAttribute());
            // Client Side Cache
            */


        }
    }
}
