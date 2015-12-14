using AutoPoco.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFunTech.Sms.Simulation.DataSources
{
    public class PhoneDataSource : DatasourceBase<string>
    {
        private readonly string[] data = new string[] { 
            "27208881",
            "27208882",
            "27208883",
            "27208884",
            "27208885",
            "27208886",
            "27208887",
            "27208888",
            "27208889",
            "27208890"
        };

        public override string Next(IGenerationContext context)
        {
            var builder = new StringBuilder();
            int index = RandomNumberGenerator.Current.Next(data.Length);
            return data[index];
        }
    }
}
