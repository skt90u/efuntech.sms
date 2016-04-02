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
using EFunTech.Sms.Portal.Controllers;

namespace EFunTech.Sms.ConsoleApplication
{
    class Program
    {
        static void Main(string[] args)
        {
            Program pg = new Program();

            pg.SendEmail();
            //pg.RemoveUser();
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

        private void RemoveUser()
        {
            var controller = new DepartmentManagerController(systemParameters, context, logService);

            controller.Delete("da5227aa-290f-4a8d-8279-b158f6588db0").Wait();
        }

        public void RetrySMS()
        {
            try
            {

                CommonSmsService css = new CommonSmsService(systemParameters, logService, unitOfWork);
                css.RetrySMS(3914);
            }
            catch(Exception ex)
            {
                var b = 0;
            }
        }

        public void SendEmail()
        {
            try
            {
                CommonMailService cms = new CommonMailService(systemParameters, logService);

                cms.Send("EE", "DD", new string[]{"skt90u@gmail.com"});

                var a = 0;
            }
            catch(Exception ex)
            {
                var b = 0;
            }
        }
    }
}
