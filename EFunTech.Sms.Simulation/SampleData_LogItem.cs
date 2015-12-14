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
    public class SampleData_LogItem : SampleData 
    {
        private static IGenerationSessionFactory factory;
        private static IRandomNumberGenerator randomNumberGenerator;
        private static IGenerationSession session;

        static SampleData_LogItem()
        {
            factory = AutoPocoContainer.Configure(x =>
            {
                x.Conventions(c =>
                {
                    c.UseDefaultConventions();
                });

                x.AddFromAssemblyContainingType<LogItem>();

                x.Include<LogItem>()
                    .Setup(p => p.EntryAssembly).Use<SubjectDataSource>()
                    .Setup(p => p.Class).Use<SubjectDataSource>()
                    .Setup(p => p.Method).Use<SubjectDataSource>()
                    .Setup(p => p.Message).Use<ContentDataSource>()
                    .Setup(p => p.StackTrace).Use<ContentDataSource>()
                    .Setup(p => p.CreatedTime).Use<DateTimeSource>(DateTime.UtcNow.AddYears(-10), DateTime.UtcNow.AddYears(10))
                    .Setup(p => p.Host).Use<SubjectDataSource>()
                    ;
            });

            session = factory.CreateSession();

            randomNumberGenerator = new RandomNumberGenerator();
        }

        protected override void SeedEntity(ApplicationDbContext context, ApplicationUser user)
        {
            // LogLevel
            var LogItems = session.List<LogItem>(randomNumberGenerator.Next(50, 200))
                .Impose(p => p.UserName, user.UserName)
                .Impose(p => p.LogLevel, (LogLevel)randomNumberGenerator.Next(Enum.GetValues(typeof(LogLevel)).Length))
                .Get();

            context.LogItems.AddRange(LogItems);

            context.SaveChanges();
        }
    }
}
