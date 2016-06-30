using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Linq;

using System;
using System.Text;
using System.Collections.Generic;
using Hangfire;

namespace EFunTech.Sms.Portal
{
    public class CommonMailService
    {
        private ISystemParameters systemParameters;
        private ILogService logService;

        private IMailProvider mailProvider;
        
        public CommonMailService(ISystemParameters systemParameters, ILogService logService)
        {
            this.systemParameters = systemParameters;
            this.logService = logService;

            this.mailProvider = new GMailProvider(systemParameters, logService);
            //this.mailProvider = new AliyunMailProvider(systemParameters, logService);
        }

        [AutomaticRetry(Attempts = 0)]
        public void Send(string subject, string body, string[] destinations)
        {
            //this.mailProvider.Send(subject, body, destinations);

            // 20160330 Norman, 阿里雲 Mail Server 錯誤導致
            try
            {
                this.mailProvider.Send(subject, body, destinations);
            }
            catch(Exception ex)
            {
                logService.Error(ex);
                // 20160330 Norman, 不再往外部傳遞 避免 阿里雲 Mail Server 錯誤導致 傳送簡訊失敗
            }
            
        }
    }
}
