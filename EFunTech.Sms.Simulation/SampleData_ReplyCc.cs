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
    public class SampleData_ReplyCc : SampleData 
    {
        private static IGenerationSessionFactory factory;
        private static IRandomNumberGenerator randomNumberGenerator;
        private static IGenerationSession session;

        static SampleData_ReplyCc()
        {
            factory = AutoPocoContainer.Configure(x =>
            {
                x.Conventions(c =>
                {
                    c.UseDefaultConventions();
                });

                x.AddFromAssemblyContainingType<ReplyCc>();

                x.Include<ReplyCc>()
                    .Setup(p => p.BySmsMessage).Use<BooleanSource>()
                    .Setup(p => p.ByEmail).Use<BooleanSource>()
                    ;
            });

            session = factory.CreateSession();

            randomNumberGenerator = new RandomNumberGenerator();
        }

        protected override void SeedEntity(ApplicationDbContext context, ApplicationUser user)
        {
            if (user.ReplyCc != null) return;

            var replyCc = session.Single<ReplyCc>()
                .Impose(x => x.OwnerId, user.Id)
                .Impose(x => x.Owner, user)
                .Get();

            context.ReplyCcs.Add(replyCc);

            context.SaveChanges();
        }
    }
}
