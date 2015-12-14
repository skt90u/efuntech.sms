using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Linq;

using System;
using System.Text;
using System.Collections.Generic;

namespace EFunTech.Sms.Portal
{
    public class CommonMailService
    {
        private IMailProvider mailProvider;

        public CommonMailService(ISystemParameters systemParameters, ILogService logService)
        {
            //this.mailProvider = new GMailProvider(systemParameters, logService);
            this.mailProvider = new AliyunMailProvider(systemParameters, logService);
        }

        public void Send(string subject, string body, string[] destinations)
        {
            this.mailProvider.Send(subject, body, destinations);
        }
    }
}
