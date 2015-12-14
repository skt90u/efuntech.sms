using AutoPoco;
using AutoPoco.DataSources;
using AutoPoco.Engine;
using EFunTech.Sms.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity.Validation;
using EFunTech.Sms.Simulation.DataSources;
using EFunTech.Sms.Simulation.Comparision;
using System.Linq.Expressions;
using System.Data.Entity;

namespace EFunTech.Sms.Simulation
{
    public class SampleData_Blacklist : SampleData 
    {
        private static IGenerationSessionFactory factory;
        private static IRandomNumberGenerator randomNumberGenerator;
        private static IGenerationSession session;

        static SampleData_Blacklist()
        {
            factory = AutoPocoContainer.Configure(x =>
            {
                x.Conventions(c =>
                {
                    c.UseDefaultConventions();
                });

                x.AddFromAssemblyContainingType<Blacklist>();

                x.Include<Blacklist>()
                    .Setup(p => p.Name).Use<FirstNameSource>()
                    .Setup(p => p.Mobile).Use<MobileDataSource>()
                    .Setup(p => p.Enabled).Use<BooleanSource>()
                    .Setup(p => p.Remark).Use<RemarkDataSource>()
                    .Setup(p => p.UpdatedTime).Use<DateTimeSource>(DateTime.UtcNow.AddYears(-10), DateTime.UtcNow.AddYears(10))
                    ;
            });

            session = factory.CreateSession();

            randomNumberGenerator = new RandomNumberGenerator();
        }

        protected override void SeedEntity(ApplicationDbContext context, ApplicationUser user)
        {
            // 已經有測試資料
            if (context.Blacklists.Any(x => x.CreatedUser.Id == user.Id)) return;

            var Blacklists = session.List<Blacklist>(randomNumberGenerator.Next(50, 200))
                .Impose(p => p.CreatedUser, user)
                .Impose(p => p.UpdatedUserName, user.UserName)
                .Impose(p => p.UploadedFile, null)
                .Get();

            context.Blacklists.AddRange(Blacklists);

            context.SaveChanges();
        }
    }
}
