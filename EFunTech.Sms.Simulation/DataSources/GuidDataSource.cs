using AutoPoco.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFunTech.Sms.Simulation.DataSources
{
    public class GuidDataSource : DatasourceBase<string>
    {
        public override string Next(IGenerationContext context)
        {
            return Guid.NewGuid().ToString().Replace("-", string.Empty);
        }
    }
}
