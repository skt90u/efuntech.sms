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
    public class SampleData_Signature : SampleData 
    {
        private static IGenerationSessionFactory factory;
        private static IRandomNumberGenerator randomNumberGenerator;
        private static IGenerationSession session;

        static SampleData_Signature()
        {
            factory = AutoPocoContainer.Configure(x =>
            {
                x.Conventions(c =>
                {
                    c.UseDefaultConventions();
                });

                x.AddFromAssemblyContainingType<Signature>();

                x.Include<Signature>()
                    .Setup(p => p.Subject).Use<SubjectDataSource>()
                    .Setup(p => p.Content).Use<ContentDataSource>()
                    .Setup(p => p.UpdatedTime).Use<DateTimeSource>(DateTime.UtcNow.AddYears(-10), DateTime.UtcNow.AddYears(10))
                    ;
            });

            session = factory.CreateSession();

            randomNumberGenerator = new RandomNumberGenerator();
        }

        protected override void SeedEntity(ApplicationDbContext context, ApplicationUser user)
        {
            // 已經有測試資料
            if (context.Signatures.Any(x => x.CreatedUser.Id == user.Id)) return;

            var Signatures = session.List<Signature>(randomNumberGenerator.Next(50, 200))
                .Impose(p => p.CreatedUser, user)
                .Get();

            context.Signatures.AddRange(Signatures);

            context.SaveChanges();
        }
    }
}
