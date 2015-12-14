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
    public class SampleData_Contact : SampleData 
    {
        private static IGenerationSessionFactory factory;
        private static IRandomNumberGenerator randomNumberGenerator;
        private static IGenerationSession session;

        static SampleData_Contact()
        {
            factory = AutoPocoContainer.Configure(x =>
            {
                x.Conventions(c =>
                {
                    c.UseDefaultConventions();
                });

                x.AddFromAssemblyContainingType<Contact>();

                x.Include<Contact>()
                    .Setup(p => p.Name).Use<FirstNameSource>()
                    .Setup(p => p.Mobile).Use<MobileDataSource>()
                    .Setup(p => p.HomePhoneArea).Use<PhoneAreaDataSource>()
                    .Setup(p => p.HomePhone).Use<PhoneDataSource>()
                    .Setup(p => p.CompanyPhoneArea).Use<PhoneAreaDataSource>()
                    .Setup(p => p.CompanyPhone).Use<PhoneDataSource>()
                    .Setup(p => p.CompanyPhoneExt).Use<PhoneExtDataSource>()
                    .Setup(p => p.Email).Use<EmailAddressSource>()
                    .Setup(p => p.Msn).Use<MsnDataSource>()
                    .Setup(p => p.Description).Use<DescriptionDataSource>()
                    .Setup(p => p.Birthday).Use<BirthdaySource>(2005, 2010)
                    .Setup(p => p.ImportantDay).Use<BirthdaySource>(2005, 2010)
                    .Setup(p => p.Gender).Use<GenderSource>()
                    ;
            });

            session = factory.CreateSession();

            randomNumberGenerator = new RandomNumberGenerator();
        }

        protected override void SeedEntity(ApplicationDbContext context, ApplicationUser user)
        {
            // 已經有測試資料
            if (context.Contacts.Any(x => x.CreatedUser.Id == user.Id)) return;

            var Contacts = session.List<Contact>(randomNumberGenerator.Next(50, 200))
                .Impose(p => p.CreatedUser, user)
                .Impose(p => p.Groups, string.Empty)
                .Get();

            context.Contacts.AddRange(Contacts);

            context.SaveChanges();
        }
    }
}
