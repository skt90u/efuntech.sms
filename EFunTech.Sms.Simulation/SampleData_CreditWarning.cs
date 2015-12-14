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
    public class SampleData_CreditWarning : SampleData 
    {
        private static IGenerationSessionFactory factory;
        private static IRandomNumberGenerator randomNumberGenerator;
        private static IGenerationSession session;

        static SampleData_CreditWarning()
        {
            factory = AutoPocoContainer.Configure(x =>
            {
                x.Conventions(c =>
                {
                    c.UseDefaultConventions();
                });

                x.AddFromAssemblyContainingType<CreditWarning>();

                x.Include<CreditWarning>()
                    .Setup(p => p.Enabled).Use<BooleanSource>()
                    .Setup(p => p.BySmsMessage).Use<BooleanSource>()
                    .Setup(p => p.ByEmail).Use<BooleanSource>()
                    ;
            });

            session = factory.CreateSession();

            randomNumberGenerator = new RandomNumberGenerator();
        }

        protected override void SeedEntity(ApplicationDbContext context, ApplicationUser user)
        {
            if (user.CreditWarning != null) return;

            var creditWarning = session.Single<CreditWarning>()
                .Impose(x => x.SmsBalance, Convert.ToDecimal(randomNumberGenerator.Next(100)))
                .Impose(x => x.LastNotifiedTime, DateTime.UtcNow)
                .Impose(x => x.NotifiedInterval, TimeSpan.FromHours(1).TotalSeconds)
                .Impose(x => x.Owner, user)
                .Get();

            context.CreditWarnings.Add(creditWarning);
            
            context.SaveChanges();
        }
    }
}
