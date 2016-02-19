using AutoPoco.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFunTech.Sms.Simulation.DataSources
{
    public class PhoneAreaDataSource : DatasourceBase<string>
    {
        private readonly string[] data = { 
            "02",
            "03",
            "04",
            "05",
            "06",
            "07",
            "08",
            "037",
            "049",
            "082",
            "089",
            "0826",
            "0836",
        };

        public override string Next(IGenerationContext context)
        {
            var builder = new StringBuilder();
            int index = RandomNumberGenerator.Current.Next(data.Length);
            return data[index];
        }
    }
}
