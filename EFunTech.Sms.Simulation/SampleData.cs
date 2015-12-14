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
    /// <summary>
    /// 參考資料
    /// https://autopoco.codeplex.com/
    /// http://autopocodatasources.skaele.nl/
    /// </summary>
    public class SampleData
    {
        public void Seed(ApplicationDbContext context, ApplicationUser user)
        {
            //new SampleData_WebAuthorization().SeedEntity(context, user);
            //new SampleData_MenuItem().SeedEntity(context, user);
            
            //new SampleData_LogItem().SeedEntity(context, user);

            /*
            new SampleData_ApplicationUser().SeedEntity(context, user);
            new SampleData_CreditWarning().SeedEntity(context, user);
            new SampleData_ReplyCc().SeedEntity(context, user);
            new SampleData_CommonMessage().SeedEntity(context, user);
            new SampleData_Blacklist().SeedEntity(context, user);
            new SampleData_Contact().SeedEntity(context, user);
            new SampleData_Group().SeedEntity(context, user);
            new SampleData_GroupContact().SeedEntity(context, user);
            new SampleData_Department().SeedEntity(context, user);
            new SampleData_Signature().SeedEntity(context, user);
            new SampleData_UploadedFile().SeedEntity(context, user);
            new SampleData_SendMessageRule().SeedEntity(context, user);
            new SampleData_SendMessageHistories().SeedEntity(context, user);
            */

            //new SampleData_TradeDetail().SeedEntity(context, user);
            // 這個函式不適合在這邊執行
            //new SampleData_SharedGroupContact().SeedEntity(context, user);
        }

        protected virtual void SeedEntity(ApplicationDbContext context, ApplicationUser user)
        {
            throw new NotImplementedException();
        }
    }
}
