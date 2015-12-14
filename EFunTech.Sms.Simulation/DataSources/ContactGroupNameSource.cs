using AutoPoco.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFunTech.Sms.Simulation.DataSources
{
    public class ContactGroupNameSource : DatasourceBase<string>
    {
        private readonly string[] data = new string[] { 
            "Group01",
            "Group02",
            "Group03",
            "Group04",
            "Group05",
            "Group06",
            "Group07",
            "Group08",
            "Group09",
            "Group10"
        };

        public override string Next(IGenerationContext context)
        {
            var builder = new StringBuilder();
            int index = RandomNumberGenerator.Current.Next(data.Length);
            return data[index];
        }
    }
}
