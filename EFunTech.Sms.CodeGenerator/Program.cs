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
                //var a0 = context.Set<ApplicationUser>().Project().To<ApplicationUserModel>().ToList();
                var a1 = context.Set<ApplicationUser>().Project().To<ApplicationUserModel1>().ToList();

                foreach (var c in a1)
                {
                    Console.WriteLine(c.FullName);
                }
                //var a2 = context.Set<ApplicationUser>().Project().To<ApplicationUserModel2>().ToList();
                //var a3 = context.Set<ApplicationUser>().Project().To<ApplicationUserModel3>().ToList();

            }
        }

        static void Main(string[] args)
        {
            //Diagnostics.Check();

            //return;

            string PortalDir = @"C:\cygwin64\home\Z215\Project\efuntech.sms\EFunTech.Sms.Portal";
            //string PortalDir = @"C:\Project\efuntech.sms\EFunTech.Sms.Portal";
            bool Overwrite = true;
        
            var jobs = new List<IJob>();

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
