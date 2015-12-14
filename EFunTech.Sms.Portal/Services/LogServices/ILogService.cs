using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFunTech.Sms.Portal
{
    public interface ILogService
    {
        void Debug(string format, params object[] arg);
        void Info(string format, params object[] arg);
        void Warn(string format, params object[] arg);
        void Error(string format, params object[] arg);
        void Error(Exception ex);
    }
}
