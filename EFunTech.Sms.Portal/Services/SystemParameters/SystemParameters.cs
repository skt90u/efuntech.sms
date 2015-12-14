
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
            this.InfobipUserName = ConfigurationManager.AppSettings["InfobipUserName"];
            this.InfobipPassword = ConfigurationManager.AppSettings["InfobipPassword"];
            this.Every8dUserName = ConfigurationManager.AppSettings["Every8dUserName"];
            this.Every8dPassword = ConfigurationManager.AppSettings["Every8dPassword"];

            this.ExpireTimeSpan = Convert.ToInt32(ConfigurationManager.AppSettings["ExpireTimeSpan"]);
            this.ValidateInterval = Convert.ToInt32(ConfigurationManager.AppSettings["ValidateInterval"]);

            this.ContactAtMostOneGroup = true;
        }

        public string EMailUserName { get; set; }
        public string EMailPassword { get; set; }

        public string InfobipUserName { get; set; }
        public string InfobipPassword { get; set; }

        public string Every8dUserName { get; set; }
        public string Every8dPassword { get; set; }

        public int ExpireTimeSpan { get; set; }
        public int ValidateInterval { get; set; }
        public bool ContactAtMostOneGroup { get; set; }
    }
}
