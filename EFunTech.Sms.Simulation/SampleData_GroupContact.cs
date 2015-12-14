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
    public class SampleData_GroupContact : SampleData 
    {
        private static IGenerationSessionFactory factory;
        private static IRandomNumberGenerator randomNumberGenerator;
        private static IGenerationSession session;

        static SampleData_GroupContact()
        {
            factory = AutoPocoContainer.Configure(x =>
            {
                x.Conventions(c =>
                {
                    c.UseDefaultConventions();
                });

                x.AddFromAssemblyContainingType<GroupContact>();

                //x.Include<Signature>()
                //    .Setup(p => p.Subject).Use<SubjectDataSource>()
                //    .Setup(p => p.Content).Use<ContentDataSource>()
                //    .Setup(p => p.UpdatedTime).Use<DateTimeSource>(DateTime.UtcNow.AddYears(-10), DateTime.UtcNow.AddYears(10))
                //    ;
            });

            session = factory.CreateSession();

            randomNumberGenerator = new RandomNumberGenerator();
        }

        protected override void SeedEntity(ApplicationDbContext context, ApplicationUser user)
        {
            var ContactIds = context.Contacts.Where(x => x.CreatedUser.Id == user.Id).Select(x => x.Id);

            // 已經有測試資料
            if (context.GroupContacts.Any(x => ContactIds.Contains(x.ContactId))) return;

            var GroupContacts = new List<GroupContact>();
            var GroupIds = context.Groups.Where(x => x.CreatedUserId == user.Id).Select(x => x.Id).ToArray();
            var GroupCount = GroupIds.Count();
            foreach (var ContactId in ContactIds)
            {
                int index = randomNumberGenerator.Next(GroupCount + 1);
                if (index < GroupCount - 1)
                {
                    var GroupId = GroupIds[index];

                    GroupContacts.Add(new GroupContact
                    {
                        GroupId = GroupId,
                        ContactId = ContactId,
                    });
                }
            }
            context.GroupContacts.AddRange(GroupContacts);

            context.SaveChanges();
        }
    }
}
