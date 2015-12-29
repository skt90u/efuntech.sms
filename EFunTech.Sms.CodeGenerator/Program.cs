using EFunTech.Sms.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using AutoMapper;
using AutoMapper.QueryableExtensions;

using JSBeautifyLib;
using System.Reflection;
using EFunTech.Sms.Portal;
using Every8dApi;
using System.Diagnostics;
using EFunTech.Sms.Portal.Models;

namespace EFunTech.Sms.CodeGenerator
{
    class Program
    {
        public static void ConfigureMapper()
        {
            var profileType = typeof(Profile);
            // Get an instance of each Profile in the executing assembly.
            var profiles = Assembly.GetExecutingAssembly().GetTypes()
                .Where(t => profileType.IsAssignableFrom(t)
                    && t.GetConstructor(Type.EmptyTypes) != null)
                .Select(Activator.CreateInstance)
                //.Select(ServiceLocator.Current.GetInstance) // not working
                .Cast<Profile>();

            Mapper.Initialize(cfg => profiles.ToList().ForEach(cfg.AddProfile));

            //Mapper.Initialize(cfg => {
            //    cfg.ConstructServicesUsing(type => Activator.CreateInstance(type));
            //    cfg.ConstructServicesUsing(type => ServiceLocator.Current.GetInstance(type));
            //});
        }

        static void EfTest()
        {
            ConfigureMapper();

            using (var context = new ApplicationDbContext())
            {
                var n = 0;

                //var a0 = context.Set<ApplicationUser>().Project().To<ApplicationUserModel>().ToList();
                var a1 = context.Set<ApplicationUser>().Project().To<ApplicationUserModel1>().ToList();

                foreach (var c in a1)
                {
                    Console.WriteLine(c.FullName);
                }
                //var a2 = context.Set<ApplicationUser>().Project().To<ApplicationUserModel2>().ToList();
                //var a3 = context.Set<ApplicationUser>().Project().To<ApplicationUserModel3>().ToList();

                var b = 0;
            }
        }

        static void Main(string[] args)
        {
            EfTest();
            return;

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
