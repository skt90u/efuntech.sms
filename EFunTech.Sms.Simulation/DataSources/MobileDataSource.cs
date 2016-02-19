using AutoPoco.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFunTech.Sms.Simulation.DataSources
{
    public class MobileDataSource : DatasourceBase<string>
    {
        private readonly string[] data = { 
            "886921859601",
            "886921859602",
            "886921859603",
            "886921859604",
            "886921859605",
            "886921859606",
            "886921859607",
            "886921859608",
            "886921859609",
            "886921859610"
        };

        public override string Next(IGenerationContext context)
        {
            var builder = new StringBuilder();
            int index = RandomNumberGenerator.Current.Next(data.Length);
            return data[index];
        }
    }
}
