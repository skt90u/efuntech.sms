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
    public class SampleData_SharedGroupContact : SampleData 
    {
        private static IGenerationSessionFactory factory;
        private static IRandomNumberGenerator randomNumberGenerator;
        private static IGenerationSession session;

        static SampleData_SharedGroupContact()
        {
            factory = AutoPocoContainer.Configure(x =>
            {
                x.Conventions(c =>
                {
                    c.UseDefaultConventions();
                });

                x.AddFromAssemblyContainingType<SharedGroupContact>();

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
            // 目前使用者所有的聯絡人群組(排除常用聯絡人)
            var groupIds = user.Groups.Where(x => x.Name != Group.CommonContactGroupName).Select(x => x.Id).ToList();
            // 已經有測試資料
            if (context.SharedGroupContacts.Any(x => groupIds.Contains(x.GroupId))) return;

            var random_groupCount = randomNumberGenerator.Next(groupIds.Count());
            var random_groupIds = groupIds.PickRandom(random_groupCount).ToList();

            var userIds = context.Users.Where(x => x.Id != user.Id).Select(x => x.Id).ToList();

            var SharedGroupContacts = new List<SharedGroupContact>();
            foreach (var groupId in random_groupIds)
            {
                var random_userCount = randomNumberGenerator.Next(userIds.Count());
                var random_userIds = userIds.PickRandom(random_userCount).ToList();

                foreach (var userId in random_userIds)
                {
                    SharedGroupContacts.Add(new SharedGroupContact
                    {
                        GroupId = groupId,
                        ShareToUserId = userId
                    });
                }
            }

            // 不需要額外Distinct
            //SharedGroupContacts = SharedGroupContacts.Distinct(new SharedGroupContactCompare()).ToList();

            context.SharedGroupContacts.AddRange(SharedGroupContacts);

            context.SaveChanges();
        }
    }
}
