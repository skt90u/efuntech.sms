using AutoPoco.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFunTech.Sms.Simulation.DataSources
{
    public class PhoneExtDataSource : DatasourceBase<string>
    {
        #region Fields

        /// <summary>
        /// The max.
        /// </summary>
        private readonly int max;

        /// <summary>
        /// The min.
        /// </summary>
        private readonly int min;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="RandomStringSource"/> class.
        /// </summary>
        /// <param name="min">
        /// The min.
        /// </param>
        /// <param name="max">
        /// The max.
        /// </param>
        public PhoneExtDataSource(int min, int max)
        {
            this.min = min;
            this.max = max;
        }

        public PhoneExtDataSource()
            : this(0, 10000)
        {
        }

        #endregion

        public override string Next(IGenerationContext context)
        {
            return RandomNumberGenerator.Current.Next(this.min, this.max + 1).ToString();
        }
    }
}
