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
    public class SampleData_ApplicationUser : SampleData 
    {
        private static IGenerationSessionFactory factory;
        private static IRandomNumberGenerator randomNumberGenerator;
        private static IGenerationSession session;

        static SampleData_ApplicationUser()
        {
            factory = AutoPocoContainer.Configure(x =>
            {
                x.Conventions(c =>
                {
                    c.UseDefaultConventions();
                });

                x.AddFromAssemblyContainingType<ApplicationUser>();

                x.Include<ApplicationUser>()
                    .Setup(p => p.Gender).Use<GenderSource>()
                    .Setup(p => p.ContactPhone).Use<PhoneDataSource>()
                    .Setup(p => p.ContactPhoneExt).Use<PhoneExtDataSource>()
                    .Setup(p => p.ForeignSmsEnabled).Use<BooleanSource>()
                    .Setup(p => p.Email).Use<EmailAddressSource>()
                    .Setup(p => p.EmailConfirmed).Use<BooleanSource>()
                    .Setup(p => p.PhoneNumber).Use<MobileDataSource>()
                    .Setup(p => p.PhoneNumberConfirmed).Use<BooleanSource>()
                    ;
            });

            session = factory.CreateSession();

            randomNumberGenerator = new RandomNumberGenerator();
        }

        protected override void SeedEntity(ApplicationDbContext context, ApplicationUser user)
        {
            if (!string.IsNullOrEmpty(user.ContactPhone)) return;

            var referenceUser = session.Single<ApplicationUser>().Get();

            user.Gender = referenceUser.Gender;
            user.ContactPhone = referenceUser.ContactPhone;
            user.ContactPhoneExt = referenceUser.ContactPhoneExt;
            user.ForeignSmsEnabled = referenceUser.ForeignSmsEnabled;
            user.Email = referenceUser.Email;
            user.EmailConfirmed = referenceUser.EmailConfirmed;
            user.PhoneNumber = referenceUser.PhoneNumber;
            user.PhoneNumberConfirmed = referenceUser.PhoneNumberConfirmed;

            user.PhoneNumber = "886921859698";
            user.Email = "skt90u@gmail.com";

            // 異動資料
            var entry = context.Entry(user);
            context.Set<ApplicationUser>().Attach(user);
            entry.State = EntityState.Modified;

            context.SaveChanges();
        }
    }
}
