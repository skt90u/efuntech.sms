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
    public class SampleData_TradeDetail : SampleData 
    {
        private static IGenerationSessionFactory factory;
        private static IRandomNumberGenerator randomNumberGenerator;
        private static IGenerationSession session;

        static SampleData_TradeDetail()
        {
            factory = AutoPocoContainer.Configure(x =>
            {
                x.Conventions(c =>
                {
                    c.UseDefaultConventions();
                });

                x.AddFromAssemblyContainingType<TradeDetail>();

                x.Include<TradeDetail>()
                    .Setup(p => p.TradeTime).Use<DateTimeSource>(DateTime.UtcNow.AddYears(-10), DateTime.UtcNow.AddYears(10))
                    .Setup(p => p.Remark).Use<ContentDataSource>()
                    ;
            });

            session = factory.CreateSession();

            randomNumberGenerator = new RandomNumberGenerator();
        }

        protected override void SeedEntity(ApplicationDbContext context, ApplicationUser user)
        {
            var TradeDetails = session.List<TradeDetail>(randomNumberGenerator.Next(50, 200))
                .Impose(p => p.OwnerId, user.Id)
                .Impose(p => p.TargetId, user.Id)
                .Impose(p => p.TradeType, (TradeType)randomNumberGenerator.Next(Enum.GetValues(typeof(TradeType)).Length))
                .Impose(x => x.Point, Convert.ToDecimal(randomNumberGenerator.Next(100)))
                .Get();

            context.TradeDetails.AddRange(TradeDetails);
            
            context.SaveChanges();
        }
    }
}
