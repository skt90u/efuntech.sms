
using System.Configuration;
using System;

namespace EFunTech.Sms.Portal
{
    public class SystemParameters : ISystemParameters
    {
        public SystemParameters()
        {
            this.EMailUserName = ConfigurationManager.AppSettings["EMailUserName"];
            this.EMailPassword = ConfigurationManager.AppSettings["EMailPassword"];
            this.InfobipNormalQualityUserName = ConfigurationManager.AppSettings["InfobipNormalQualityUserName"];
            this.InfobipNormalQualityPassword = ConfigurationManager.AppSettings["InfobipNormalQualityPassword"];
            this.InfobipHighQualityUserName = ConfigurationManager.AppSettings["InfobipHighQualityUserName"];
            this.InfobipHighQualityPassword = ConfigurationManager.AppSettings["InfobipHighQualityPassword"];
            this.Every8dUserName = ConfigurationManager.AppSettings["Every8dUserName"];
            this.Every8dPassword = ConfigurationManager.AppSettings["Every8dPassword"];

            this.RetryMaxTimes = Convert.ToInt32(ConfigurationManager.AppSettings["RetryMaxTimes"]);

            this.ExpireTimeSpan = Convert.ToInt32(ConfigurationManager.AppSettings["ExpireTimeSpan"]);
            this.ValidateInterval = Convert.ToInt32(ConfigurationManager.AppSettings["ValidateInterval"]);

            this.ContactAtMostOneGroup = true;
            this.AllowSendMessage = Convert.ToBoolean(ConfigurationManager.AppSettings["AllowSendMessage"]);
            this.InsufficientBalanceNotifiee = ConfigurationManager.AppSettings["InsufficientBalanceNotifiee"];

            this.MaxUploadedMessageReceiver = Convert.ToInt32(ConfigurationManager.AppSettings["MaxUploadedMessageReceiver"]);
        }

        public string EMailUserName { get; set; }
        public string EMailPassword { get; set; }

        public string InfobipNormalQualityUserName { get; set; }
        public string InfobipNormalQualityPassword { get; set; }

        public string InfobipHighQualityUserName { get; set; }
        public string InfobipHighQualityPassword { get; set; }

        public string Every8dUserName { get; set; }
        public string Every8dPassword { get; set; }

        public int RetryMaxTimes { get; set; }

        public int ExpireTimeSpan { get; set; }
        public int ValidateInterval { get; set; }
        public bool ContactAtMostOneGroup { get; set; }

        public bool AllowSendMessage { get; set; }

        public string InsufficientBalanceNotifiee  { get; set; }

        public int MaxUploadedMessageReceiver { get; set; }
    }
}
