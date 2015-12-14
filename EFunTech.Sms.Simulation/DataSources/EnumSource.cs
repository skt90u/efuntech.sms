using AutoPoco.Engine;
using EFunTech.Sms.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace EFunTech.Sms.Simulation.DataSources
{
    public class EnumSource<T> : DatasourceBase<T> where T : struct, IConvertible
    {
        public static readonly Func<int, T> Convert = GenerateConverter();

        static Func<int, T> GenerateConverter()
        {
            var parameter = Expression.Parameter(typeof(int));
            var dynamicMethod = Expression.Lambda<Func<int, T>>(
                Expression.Convert(parameter, typeof(T)),
                parameter);
            return dynamicMethod.Compile();
        }

        public override T Next(IGenerationContext context)
        {
            int max = System.Convert.ToInt32(Enum.GetValues(typeof(T)).Cast<T>().Max());
            int index = RandomNumberGenerator.Current.Next(max);
            return Convert(index);
        }
    }
}
