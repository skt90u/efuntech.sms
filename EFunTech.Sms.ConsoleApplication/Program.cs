﻿using EFunTech.Sms.Portal;
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
using System.Data.Entity.Validation;

namespace EFunTech.Sms.ConsoleApplication
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Program pg = new Program();

                //pg.SendEmail();
                pg.RemoveUser();
            }
            catch (DbEntityValidationException ex)
            {
                throw Repository.GetRealException(ex);
            }
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

            controller.Delete("0389379c-6792-4a10-a543-1f6733525cee").Wait();
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
