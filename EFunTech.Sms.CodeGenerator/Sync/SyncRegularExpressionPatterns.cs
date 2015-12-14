using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFunTech.Sms.CodeGenerator
{
    public class SyncRegularExpressionPatterns : IJob
    {
        public string PortalDir { get; set; }
        public bool Overwrite { get; set; }

        public void Execute()
        {
            
        }
    }
}
/*
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication2
{
    class Program
    {
        public const int aaa = 1;

        static void Main(string[] args)
        {
            var m = new object();
            foreach (var f in typeof(Program).GetFields())
                if (f.IsLiteral)
                {
                    // stuff
                    var a = 0;
                }
        }
    }
}

 */
