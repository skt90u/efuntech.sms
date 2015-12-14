using EFunTech.Sms.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFunTech.Sms.CodeGenerator
{
    public class Job_20150814 : IJob
    {
        private string outputRootDir;
        private bool overwrite;
        private int columnWidth;

        public Job_20150814(string outputRootDir, bool overwrite)
        {
            this.outputRootDir = outputRootDir;
            this.overwrite = overwrite;
            this.columnWidth = 4;
        }

        public void Execute()
        {
            List<IJob> jobs = new List<IJob>();

            jobs.Add(new SingleGridPageGenerator(typeof(Contact), outputRootDir, overwrite, columnWidth, "AllContactsController", "ContactManager"));
            
            jobs.ForEach(job => job.Execute());
        }
    }
}
