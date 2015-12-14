using EFunTech.Sms.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using JSBeautifyLib;
using System.Reflection;
using EFunTech.Sms.Portal;
using Every8dApi;
using System.Diagnostics;

namespace EFunTech.Sms.CodeGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
           


            //foreach (var mobile in mobiles)
            //{
            //    var result = MobileUtil.GetRegionInfo(mobile);
            //    Console.WriteLine(result.DisplayName);
            //}
            
            /*
            TimeSpan ts = new TimeSpan(8, 0, 0);

            TimeZoneInfo match = null;

            foreach(var n in TimeZoneInfo.GetSystemTimeZones())
            {
                if (n.BaseUtcOffset != ts) continue;

                match = n;
                break;

                Console.WriteLine(n);
            }

            var a = TimeZoneInfo.Local;
            DateTime now = DateTime.Now;

            Console.WriteLine("now：" + now.ToString("yyyyMMddHHmm"));
            Console.WriteLine("utc：" + now.ToString("yyyyMMddHHmm"));
            //Console.WriteLine(DateTime.UtcNow.ToString("yyyyMMddHHmm"));
            //Console.WriteLine(DateTime.UtcNow.ToUniversalTime().ToString("yyyyMMddHHmm"));
            //Console.WriteLine(now.ToUniversalTime().ToString("yyyyMMddHHmm"));
            //Console.WriteLine(TimeZone.CurrentTimeZone.ToUniversalTime(DateTime.UtcNow).ToString("yyyyMMddHHmm"));
            //Console.WriteLine(TimeZoneInfo.ConvertTimeToUtc(now, match).ToString("yyyyMMddHHmm"));

            var b = TimeZoneInfo.ConvertTime(now, match).ToUniversalTime();
            var aa = TimeZone.CurrentTimeZone.ToUniversalTime(DateTime.UtcNow);
            //var bb = TimeZone.CurrentTimeZone.ToUniversalTime(DateTime.Now);
            return;
            */
            //TestOneApi a = new TestOneApi();
            //a.Case3();
            //return;

            //Utils.HtmlToTemplate("SendRuleModal",
            //    @"C:\Project\efuntech.sms\EFunTech.Sms.CodeGenerator\INPUT\MemberSelector.html",
            //    @"C:\Project\efuntech.sms\EFunTech.Sms.CodeGenerator\INPUT\MemberSelector.js");
            //return;

            //SendMessageRuleExamples examples = new SendMessageRuleExamples();
            //examples.Example01();
            //return;

            Diagnostics.Check();

            //string inputDir = @"C:\Project\efuntech.sms\EFunTech.Sms.CodeGenerator\INPUT";
            //string outputDir = @"C:\Project\efuntech.sms\EFunTech.Sms.CodeGenerator\OUTPUT";
            //string[] extensions = new string[] { "*.js", "*.cs" };
            //foreach (string extension in extensions)
            //{
            //    foreach (string filePath in Directory.GetFiles(inputDir, extension))
            //    {
            //        Utils.AppenAddLineToFile(filePath, Path.Combine(outputDir, Path.GetFileName(filePath) + ".out"));
            //    }
            //}
            
            
            string PortalDir = @"C:\Project\efuntech.sms\EFunTech.Sms.Portal";
            bool Overwrite = true;
        
            List<IJob> jobs = new List<IJob>();

            //jobs.Add(new SyncLookupApi
            //{
            //    PortalDir = PortalDir,
            //    Overwrite = Overwrite,
            //});
            //jobs.Add(new SyncValidationApi
            //{
            //    PortalDir = PortalDir,
            //    Overwrite = Overwrite,
            //});
            //jobs.Add(new SyncRegularExpressionPatterns
            //{
            //    PortalDir = PortalDir,
            //    Overwrite = Overwrite,
            //});
            jobs.Add(new SyncEnum
            {
                PortalDir = PortalDir,
                Overwrite = Overwrite,
            });

            //jobs.Add(new Job_20151120
            //{
            //    PortalDir = PortalDir,
            //    Overwrite = Overwrite,
            //});
            
            try {
                jobs.ForEach(job => job.Execute());
            }
            catch (Exception ex)
            {
                var logService = new SmartInspectLogService();
                logService.Error(ex);
            }


            Utils.OpenFolder(Path.Combine(PortalDir, "GenerateResult"));
        }
    }
}
