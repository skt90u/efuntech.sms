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
    public class SampleData_WebAuthorization : SampleData 
    {
        static SampleData_WebAuthorization()
        {
        }

        protected override void SeedEntity(ApplicationDbContext context, ApplicationUser user)
        {
            if (context.WebAuthorizations.Count() != 0) return;

            string ControllerName = "Home";
            
            var data = new List<WebAuthorization> 
            { 
                new WebAuthorization
                {
                    ControllerName = ControllerName,
                    ActionName = "SendMessage",
                    Roles = string.Join(",", new Role[]{Role.Administrator, Role.Supervisor, Role.DepartmentHead, Role.Employee}),
                    Users = string.Empty,
                    Remark = "簡訊發送",
                },
                new WebAuthorization
                {
                    ControllerName = ControllerName,
                    ActionName = "SendParamMessage",
                    Roles = string.Join(",", new Role[]{Role.Administrator, Role.Supervisor, Role.DepartmentHead, Role.Employee}),
                    Users = string.Empty,
                    Remark = "參數簡訊發送",
                },
                new WebAuthorization
                {
                    ControllerName = ControllerName,
                    ActionName = "SearchMemberSend",
                    Roles = string.Join(",", new Role[]{Role.Administrator, Role.Supervisor, Role.DepartmentHead, Role.Employee}),
                    Users = string.Empty,
                    Remark = "發送查詢",
                },
                new WebAuthorization
                {
                    ControllerName = ControllerName,
                    ActionName = "ContactManager",
                    Roles = string.Join(",", new Role[]{Role.Administrator, Role.Supervisor, Role.DepartmentHead, Role.Employee}),
                    Users = string.Empty,
                    Remark = "聯絡人管理",
                },
                new WebAuthorization
                {
                    ControllerName = ControllerName,
                    ActionName = "SMS_Setting",
                    Roles = string.Join(",", new Role[]{Role.Administrator, Role.Supervisor, Role.DepartmentHead, Role.Employee}),
                    Users = string.Empty,
                    Remark = "系統設定",
                },

                new WebAuthorization
                {
                    ControllerName = ControllerName,
                    ActionName = "RecurringSMS",
                    Roles = string.Join(",", new Role[]{Role.Administrator, Role.Supervisor, Role.DepartmentHead}),
                    Users = string.Empty,
                    Remark = "預約/週期簡訊維護",
                },
                new WebAuthorization
                {
                    ControllerName = ControllerName,
                    ActionName = "DepartmentManager",
                    Roles = string.Join(",", new Role[]{Role.Administrator, Role.Supervisor, Role.DepartmentHead}),
                    Users = string.Empty,
                    Remark = "子帳號管理",
                },
                new WebAuthorization
                {
                    ControllerName = ControllerName,
                    ActionName = "DepartmentPointManager",
                    Roles = string.Join(",", new Role[]{Role.Administrator, Role.Supervisor, Role.DepartmentHead}),
                    Users = string.Empty,
                    Remark = "子帳號點數管理",
                },
                new WebAuthorization
                {
                    ControllerName = ControllerName,
                    ActionName = "SectorStatistics",
                    Roles = string.Join(",", new Role[]{Role.Administrator, Role.Supervisor, Role.DepartmentHead}),
                    Users = string.Empty,
                    Remark = "報表管理",
                },
                new WebAuthorization
                {
                    ControllerName = ControllerName,
                    ActionName = "Others",
                    Roles = string.Join(",", new Role[]{Role.Administrator}),
                    Users = string.Empty,
                    Remark = "其他功能",
                },
            };

            context.WebAuthorizations.AddRange(data);

            context.SaveChanges();
        }
    }
}
