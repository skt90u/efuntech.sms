using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Linq;

using System;
using System.Text;
using System.Collections.Generic;

namespace EFunTech.Sms.Portal
{
    // http://bbs.csdn.net/topics/390627183
    // http://jimmysu.logdown.com/posts/249495-gmail-smtp-authentication-required
    // http://mailhelp.mxhichina.com/smartmail/detail.vm?knoId=5871700

    public class AliyunMailProvider : IMailProvider
    {
        private string userName;
        private string password;
        private ILogService logService;
        private const int Timeout = 5000;

        public AliyunMailProvider(ISystemParameters systemParameters, ILogService logService)
        {
            this.userName = systemParameters.EMailUserName; 
            this.password = systemParameters.EMailPassword; 
            this.logService = logService;
        }

        /// <summary>
        /// http://webstackoflove.com/use-gmail-to-deliver-email-from-azure-website/
        /// </summary>
        public void Send(string subject, string body, string[] destinations)
        {
            string ticket = Guid.NewGuid().ToString();

            logService.Debug("發送MAIL(主旨: {0}, 內容: {1}, 接收者: [{2}], Ticket: {3})", subject, body, string.Join(", ", destinations.Select(p => string.Format("'{0}'", p))), ticket);

            try
            {
                var mailClient = new SmtpClient {
                    // 阿里雲不支援SSL
                    Host = "smtp.mxhichina.com",
                    Port = 25,
                    EnableSsl = false,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(userName, password),
                    Timeout = Timeout,
                };

                using (mailClient)
                {
                    using (MailMessage email = new MailMessage())
                    {
                        email.From = new MailAddress(userName);
                        foreach (string destination in destinations) email.To.Add(destination);
                        email.Subject = subject;
                        email.Body = body;
                        email.IsBodyHtml = true;
                        //email.Priority = MailPriority.High;

                        mailClient.Send(email);

                        logService.Debug("發送EMAIL成功(Ticket: {0})", ticket);
                    }
                }
            }
            catch (Exception ex)
            {
                this.logService.Error(ex);

                throw;
            }
        }
    }
}
