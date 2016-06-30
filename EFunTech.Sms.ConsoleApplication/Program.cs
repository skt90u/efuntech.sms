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
using System.Data.Entity.Validation;
using OneApi.Model;
using OneApi.Config;
using OneApi.Client.Impl;

namespace EFunTech.Sms.ConsoleApplication
{
    class Program
    {
        private static void test_notify_url()
        {
            var configuration = new Configuration("ENVOTIONS", "Envo6183");
            
            var smsClient = new SMSClient(configuration);

            var smsRequest = new SMSRequest("ABC", DateTime.Now.ToShortDateString(), new string[] { "+886921859698", "+886928873075", "+886932273210" });
            smsRequest.NotifyURL = "http://zutech-sms.azurewebsites.net/api/InfobipDeliveryReport";
            smsRequest.CallbackData = "I_AM_CallbackData";
            SendMessageResult sendMessageResult = smsClient.SmsMessagingClient.SendSMS(smsRequest);
            
            // requestId = messageId;

            string requestId = sendMessageResult.ClientCorrelator; // you can use this to get deliveryReportList later.

            Console.WriteLine(requestId);
        }

        static void Main(string[] args)
        {
            try
            {
                test_notify_url();

                return;

                Program pg = new Program();

                //pg.SendEmail();
                //pg.RemoveUser();

                pg.GetProvider();

            }
            catch (DbEntityValidationException ex)
            {
                throw Repository.GetRealException(ex);
            }
        }

        private void GetProvider()
        {
            //CommonSmsService svc = new CommonSmsService(systemParameters, logService, unitOfWork);

            //try { svc.GetProvider(SmsProviderType.InfobipNormalQuality, 10000); }
            //catch { }

            //try { svc.GetProvider(SmsProviderType.InfobipHighQuality, 10000); }
            //catch { }

            //try { svc.GetProvider(SmsProviderType.Every8d, 10000); }
            //catch { }
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
