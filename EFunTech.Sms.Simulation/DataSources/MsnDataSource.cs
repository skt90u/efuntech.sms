using AutoPoco.DataSources;
using AutoPoco.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFunTech.Sms.Simulation.DataSources
{
    public class MsnDataSource : FirstNameSource
    {
        private readonly string[] data = { 
            "gmail.com",
            "hotmail.com"
        };

        public override string Next(IGenerationContext context)
        {
            string name = base.Next(context);

            var builder = new StringBuilder();
            int index = RandomNumberGenerator.Current.Next(data.Length);
            return string.Format("{0}@{1}", name, data[index]);
        }
    }
}
