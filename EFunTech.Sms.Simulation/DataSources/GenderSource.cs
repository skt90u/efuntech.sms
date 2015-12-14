using AutoPoco.Engine;
using EFunTech.Sms.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFunTech.Sms.Simulation.DataSources
{
    public class GenderSource : DatasourceBase<Gender>
    {
        public override Gender Next(IGenerationContext context)
        {
            var builder = new StringBuilder();
            int index = RandomNumberGenerator.Current.Next(2);
            return (Gender)index;
        }
    }
}
