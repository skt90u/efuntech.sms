using EFunTech.Sms.Portal;
using EFunTech.Sms.Schema;
using JUtilSharp.Database;
using LinqKit;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFunTech.Sms.ConsoleApplication
{
    class Program
    {
        static void Main(string[] args)
        {
            Program pg = new Program();

            pg.RetrySMS();
        }

        protected DbContext context;
        protected IUnitOfWork unitOfWork;
        protected ISystemParameters systemParameters;
        protected ILogService logService;

        public Program()
        {
            this.context = new ApplicationDbContext();
            this.unitOfWork = new UnitOfWork(context);
            this.systemParameters = new SystemParameters();
            this.logService = new SmartInspectLogService();
        }

        public void RetrySMS()
        {
            try
            {
                CommonSmsService css = new CommonSmsService(systemParameters, logService, unitOfWork);

                css.RetrySMS(3625);
            }
            catch(Exception ex)
            {
                var b = 0;
            }
        }
    }
}
