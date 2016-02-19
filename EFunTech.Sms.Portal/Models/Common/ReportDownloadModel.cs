using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;

namespace EFunTech.Sms.Portal.Models.Common
{
    //public class ReportDownloadModel
    //{
    //    private string _DownloadPath;
    //    private string _WebPath;
    //    private string _FileName;
    //    private string _Folder = System.Web.HttpContext.Current.Server.MapPath("~/Reports/Downloads/");
    //    public string DownloadPath
    //    {
    //        get { return _DownloadPath; }
    //        set
    //        {
    //            _DownloadPath = value;
    //        }
    //    }
    //    public string FileName
    //    {
    //        get { return _FileName; }
    //        set
    //        {
    //            _FileName = value;

    //            _DownloadPath = _Folder + value;
    //            if (!System.IO.Directory.Exists(_Folder))
    //            {
    //                System.IO.Directory.CreateDirectory(_Folder);
    //            }
    //            _WebPath = System.Web.Mvc.UrlHelper.GenerateContentUrl("~/Reports/Downloads/" + value, new HttpContextWrapper(HttpContext.Current));//../Reports/Downloads/" +
    //        }
    //    }
        
    //    public string WebPath
    //    {
    //        get { return _WebPath; }
    //        set
    //        {
    //            _WebPath = value;
    //        }
    //    }
    //}

    public class ReportDownloadModel
    {
        public HttpContent Content { get; private set; }
        public string FileName { get; private set; }
        public MediaTypeHeaderValue ContentType { get; private set; }

        public ReportDownloadModel(byte[] content, string fileName)
        {
            this.Content = new ByteArrayContent(content);
            this.FileName = fileName;

            string ext = System.IO.Path.GetExtension(FileName).ToLower();

            var dict = new Dictionary<string,string>{
                {".xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"},
                {".zip", "application/zip"},
            };

            string mediaType = dict.ContainsKey(ext) ? dict[ext] : null;

            if (mediaType == null)
            {
                throw new Exception(string.Format("尚未建立副檔名與ContentType對應關係(目前只處理Excel)，請參考 http://www.freeformatter.com/mime-types-list.html 修改 ReportDownloadModel 相關程式碼"));
            }

            // http://www.freeformatter.com/mime-types-list.html

            this.ContentType = new MediaTypeHeaderValue(mediaType);
        }
    }
}