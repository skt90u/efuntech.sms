using EFunTech.Sms.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFunTech.Sms.CodeGenerator
{
    public class Job_20150903 : IJob
    {
        public string PortalDir { get; set; }
        public bool Overwrite { get; set; }

        public void Execute()
        {
            List<IJob> jobs = new List<IJob>();

            
            jobs.Add(new SingleGridPageGenerator {
                SchemaType = typeof(CommonMessage),
                PortalDir = PortalDir,
                Overwrite = Overwrite,
                JsControllerName = "CommonMessage",
                ColumnWidth = 4,
            });

            jobs.Add(new SingleGridPageGenerator
            {
                SchemaType = typeof(Signature),
                PortalDir = PortalDir,
                Overwrite = Overwrite,
                JsControllerName = "Signature",
                ColumnWidth = 4,
            });

            jobs.ForEach(job => job.Execute());
        }
    }
}
