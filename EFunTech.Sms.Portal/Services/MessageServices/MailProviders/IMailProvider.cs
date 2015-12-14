using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFunTech.Sms.Portal
{
    public interface IMailProvider
    {
        void Send(string subject, string body, string[] destinations);
    }
}
