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
    public class SampleData_SendMessageRuleRecipient : SampleData 
    {
        private static IGenerationSessionFactory factory;
        private static IRandomNumberGenerator randomNumberGenerator;
        private static IGenerationSession session;

        static SampleData_SendMessageRuleRecipient()
        {
            factory = AutoPocoContainer.Configure(x =>
            {
                x.Conventions(c =>
                {
                    c.UseDefaultConventions();
                });

                x.AddFromAssemblyContainingType<SendMessageRuleRecipient>();

                //x.Include<SendMessageRuleRecipient>()
                //    .Setup(p => p.Subject).Use<SubjectDataSource>()
                //    .Setup(p => p.Content).Use<ContentDataSource>()
                //    .Setup(p => p.UpdatedTime).Use<DateTimeSource>(DateTime.Now.AddYears(-10), DateTime.Now.AddYears(10))
                //    ;
            });

            session = factory.CreateSession();

            randomNumberGenerator = new RandomNumberGenerator();
        }

        protected override void SeedEntity(ApplicationDbContext context, ApplicationUser user)
        {
            // TODO: SampleData_SendMessageRuleRecipient.SeedEntity(context, user)
            context.SaveChanges();
        }
    }
}
