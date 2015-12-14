using AutoPoco.Engine;
using PhoneNumbers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFunTech.Sms.Simulation.DataSources
{
    public class E164MobileDataSource : MobileDataSource
    {
        public override string Next(IGenerationContext context)
        {
            return GetE164PhoneNumber(base.Next(context));
        }

        private string GetE164PhoneNumber(string mobile)
        {
            PhoneNumber number = PhoneNumberUtil.GetInstance().Parse(mobile, "TW");

            return PhoneNumberUtil.GetInstance().Format(number, PhoneNumberFormat.E164);
        }
    }
}
