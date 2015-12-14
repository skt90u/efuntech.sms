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
    public class SampleData_Department : SampleData 
    {
        private static IGenerationSessionFactory factory;
        private static IRandomNumberGenerator randomNumberGenerator;
        private static IGenerationSession session;

        static SampleData_Department()
        {
            factory = AutoPocoContainer.Configure(x =>
            {
                x.Conventions(c =>
                {
                    c.UseDefaultConventions();
                });

                x.AddFromAssemblyContainingType<Department>();

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
            const string departmentName = "一帆";

            // 尋找目前使用者的Role資訊
            var userRoles = context.Roles.Where(x => x.Users.Any(y => y.UserId == user.Id));

            // 建立部門
            if (userRoles.Any(x => x.Name == Role.Supervisor.ToString()))
            {
                // 如果部門已經建立，就忽略
                if (context.Departments.Any(x => x.Name == departmentName)) return;

                context.Departments.Add(new Department
                {
                    Name = departmentName,
                    Description = departmentName,
                    CreatedUser = user
                });
            }

            if (userRoles.Any(x => x.Name == Role.DepartmentHead.ToString() ||
                                   x.Name == Role.Employee.ToString()))
            {
                // 如果使用者已經指定部門，就忽略

                if (user.Department != null) return;

                // 如果部門還沒有建立，就忽略
                var department = context.Departments.Where(x => x.Name == departmentName).FirstOrDefault();
                if (department == null) return;

                user.Department = department;

                // 異動資料
                var entry = context.Entry(user);
                context.Set<ApplicationUser>().Attach(user);
                entry.State = EntityState.Modified;
            }

            context.SaveChanges();
        }
    }
}
