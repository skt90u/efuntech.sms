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
    public class SampleData_MenuItem : SampleData 
    {
        static SampleData_MenuItem()
        {
        }

        protected override void SeedEntity(ApplicationDbContext context, ApplicationUser user)
        {
            //foreach (var entity in context.MenuItems.ToList())
            //{
            //    context.MenuItems.Remove(entity);
            //}
            //context.SaveChanges();

            if (context.MenuItems.Count() != 0) return;

            var data = new List<MenuItem> { 
                new MenuItem{
                    Level = 1,
                    ParentId = null,
                    Name = "簡訊發送",
                    MapRouteUrl = "SendMessage",
                    Order = 1,
                },
                new MenuItem{
                    Level = 1,
                    ParentId = null,
                    Name = "參數簡訊發送",
                    MapRouteUrl = "SendParamMessage",
                    Order = 2,
                },
                new MenuItem{
                    Level = 1,
                    ParentId = null,
                    Name = "發送查詢",
                    MapRouteUrl = "SearchMemberSend",
                    Order = 3,
                },
                new MenuItem{
                    Level = 1,
                    ParentId = null,
                    Name = "聯絡人管理",
                    MapRouteUrl = "ContactManager",
                    Order = 4,
                },
                new MenuItem{
                    Level = 1,
                    ParentId = null,
                    Name = "系統設定",
                    MapRouteUrl = "SMS_Setting",
                    Order = 5,
                },

                new MenuItem{
                    Level = 1,
                    ParentId = null,
                    Name = "預約/週期簡訊維護",
                    MapRouteUrl = "RecurringSMS",
                    Order = 6,
                },
                new MenuItem{
                    Level = 1,
                    ParentId = null,
                    Name = "子帳號管理",
                    MapRouteUrl = "DepartmentManager",
                    Order = 7,
                },
                new MenuItem{
                    Level = 1,
                    ParentId = null,
                    Name = "子帳號點數管理",
                    MapRouteUrl = "DepartmentPointManager",
                    Order = 8,
                },
                new MenuItem{
                    Level = 1,
                    ParentId = null,
                    Name = "報表管理",
                    MapRouteUrl = "SectorStatistics",
                    Order = 9,
                },
                new MenuItem{
                    Level = 1,
                    ParentId = null,
                    Name = "其他功能",
                    MapRouteUrl = "Others",
                    Order = 10,
                },
            };

            foreach (var menuItem in data)
            {
                WebAuthorization webAuthorization = context.WebAuthorizations.Where(p => p.Remark == menuItem.Name).FirstOrDefault();
                menuItem.WebAuthorization = webAuthorization;
                context.MenuItems.Add(menuItem);
                context.SaveChanges();
            }
        }
    }
}
