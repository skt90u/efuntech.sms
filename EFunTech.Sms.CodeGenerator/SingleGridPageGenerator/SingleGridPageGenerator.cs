using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFunTech.Sms.CodeGenerator
{
    public class SingleGridPageGenerator : IJob
    {
        public Type SchemaType { get; set; }
        public string PortalDir { get; set; }
        public bool Overwrite { get; set; }
        public string JsControllerName { get; set; }
        public int ColumnWidth { get; set; }

        private SchemaInfo _SchemaInfo;
        private SchemaInfo SchemaInfo
        {
            get
            {
                if (_SchemaInfo == null)
                {
                    _SchemaInfo = new SchemaInfo { 
                        SchemaType = SchemaType,
                        JsControllerName = JsControllerName,
                        ColumnWidth = ColumnWidth,
                    };
                }
                return _SchemaInfo;
            }
        }

        public void Execute()
        {
            List<IJob> jobs = new List<IJob>();

            jobs.Add(new ModelGenerator
            {
                PortalDir = PortalDir,
                Overwrite = Overwrite,
                SchemaInfo = SchemaInfo,
            });

            jobs.Add(new AutoMapperProfileGenerator
            {
                PortalDir = PortalDir,
                Overwrite = Overwrite,
                SchemaInfo = SchemaInfo,
            });

            jobs.Add(new CrudApiControllerGenerator
            {
                PortalDir = PortalDir,
                Overwrite = Overwrite,
                SchemaInfo = SchemaInfo,
            });

            jobs.Add(new SingleUiGridControllerGenerator
            {
                PortalDir = PortalDir,
                Overwrite = Overwrite,
                SchemaInfo = SchemaInfo,
            });

            jobs.Add(new SchemaFormGenerator
            {
                PortalDir = PortalDir,
                Overwrite = Overwrite,
                SchemaInfo = SchemaInfo,
            });

            jobs.ForEach(job => job.Execute());
        }
    }



}
