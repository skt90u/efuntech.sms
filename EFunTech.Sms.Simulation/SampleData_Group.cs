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
    public class SampleData_Group : SampleData 
    {
        private static IGenerationSessionFactory factory;
        private static IRandomNumberGenerator randomNumberGenerator;
        private static IGenerationSession session;

        static SampleData_Group()
        {
            factory = AutoPocoContainer.Configure(x =>
            {
                x.Conventions(c =>
                {
                    c.UseDefaultConventions();
                });

                x.AddFromAssemblyContainingType<Group>();

                x.Include<Group>()
                    .Setup(p => p.Name).Use<ContactGroupNameSource>()
                    .Setup(p => p.Deletable).Use<BooleanSource>()
                    ;
            });

            session = factory.CreateSession();

            randomNumberGenerator = new RandomNumberGenerator();
        }

        protected override void SeedEntity(ApplicationDbContext context, ApplicationUser user)
        {
            // 已經有測試資料
            if (context.Groups.Any(x => x.CreatedUserId == user.Id)) return;

            var Groups = new List<Group>();

            Groups.Add(new Group
            {
                CreatedUserId = user.Id,
                CreatedUser = user,
                Name = Group.CommonContactGroupName,
                Description = Group.CommonContactGroupName,
                Deletable = false,
            });

            Groups.AddRange(session.List<Group>(randomNumberGenerator.Next(3, 20))
                .Impose(p => p.CreatedUserId, user.Id)
                .Impose(p => p.CreatedUser, user)
                .Impose(p => p.Deletable, true)
                .Get());

            Groups = Groups.Distinct(new ContactGroupCompare()).ToList();

            Groups.ForEach((p) => {
                if (p.Name != Group.CommonContactGroupName)
                {
                    p.Description = p.Name;
                }
            });

            context.Groups.AddRange(Groups);

            context.SaveChanges();
        }
    }
}
