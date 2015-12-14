using AutoPoco.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFunTech.Sms.Simulation.DataSources
{
    public class BirthdaySource : DatasourceBase<string>
    {
        private int yearsMin;
        private int yearsMax;

        public BirthdaySource(int yearsMin, int yearsMax)
        {
            this.yearsMin = yearsMin;
            this.yearsMax = yearsMax;
        }

        public override string Next(IGenerationContext context)
        {
            int year = DateTime.UtcNow.Year - RandomNumberGenerator.Current.Next(this.yearsMin, this.yearsMax);
            int day = RandomNumberGenerator.Current.Next(1, 28);
            int month = RandomNumberGenerator.Current.Next(1, 12);

            DateTime dateTime = new DateTime(year, month, day);

            return dateTime.ToString(@"MM/dd");
        }
    }
}
