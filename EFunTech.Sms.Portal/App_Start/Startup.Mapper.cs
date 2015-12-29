using System;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.Google;
using Owin;
using EFunTech.Sms.Portal.Models;
using EFunTech.Sms.Schema;
using AutoMapper;
using System.Reflection;
using System.Linq;
using Microsoft.Practices.ServiceLocation;

namespace EFunTech.Sms.Portal
{
    public partial class Startup
    {
        protected static bool MapperConfigureed = false;

        // 如需設定驗證的詳細資訊，請瀏覽 http://go.microsoft.com/fwlink/?LinkId=301864
        public void ConfigureMapper(IAppBuilder app)
        {
            if (MapperConfigureed == true) return;
            MapperConfigureed = true;

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
    }
}