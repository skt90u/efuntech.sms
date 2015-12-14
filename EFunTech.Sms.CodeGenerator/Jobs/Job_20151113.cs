using EFunTech.Sms.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFunTech.Sms.CodeGenerator
{
    public class Job_20151113 : IJob
    {
        public string PortalDir { get; set; }
        public bool Overwrite { get; set; }

        public void Execute()
        {
            List<IJob> jobs = new List<IJob>();

            
            //jobs.Add(new SingleGridPageGenerator
            //{
            //    SchemaType = typeof(ApplicationUser),
            //    PortalDir = PortalDir,
            //    Overwrite = Overwrite,
            //    JsControllerName = "DepartmentUsers",
            //    ColumnWidth = 4,
            //});

            jobs.Add(new SingleGridPageGenerator
            {
                SchemaType = typeof(DeliveryReportQueue),
                PortalDir = PortalDir,
                Overwrite = Overwrite,
                JsControllerName = "DeliveryReportQueue",
                ColumnWidth = 4,
            });

            jobs.ForEach(job => job.Execute());
        }
    }
}
