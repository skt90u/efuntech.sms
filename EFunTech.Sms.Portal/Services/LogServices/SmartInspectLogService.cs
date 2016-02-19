using EFunTech.Sms.Schema;
using Gurock.SmartInspect;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EFunTech.Sms.Portal
{
    public class SmartInspectLogService : ILogService
    {
        public SmartInspectLogService()
        {
            //SiAuto.Si.Error += new Gurock.SmartInspect.ErrorEventHandler(HandleError);
            SiAuto.Si.Enabled = true;
        }
        
        private string GetMessage(string format, params object[] arg)
        {
            return arg.Length != 0 ? string.Format(format, arg) : format;
        }

        private string GetExceptionMessage(Exception ex)
        {
            string exceptions = string.Format("[{0}]", string.Join(", ", GetExceptions(ex).Select(e => string.Format("'{0}'", e.Message))));
            string stackTrace = ex.StackTrace.ToString();
            return string.Format("Exceptions: {0}, StackTrace: {1}", exceptions, stackTrace);
        }

        private List<Exception> GetExceptions(Exception ex)
        {
            var es = new List<Exception>();

            Exception e = ex;

            while (e != null)
            {
                es.Add(e);

                e = e.InnerException;
            }

            es.Reverse();

            return es;
        }

        public void Debug(string format, params object[] arg)
        {
            SiAuto.Main.LogDebug(GetMessage(format, arg));
        }

        public void Info(string format, params object[] arg)
        {
            SiAuto.Main.LogMessage(GetMessage(format, arg));
        }

        public void Warn(string format, params object[] arg)
        {
            SiAuto.Main.LogWarning(GetMessage(format, arg));
        }

        public void Error(string format, params object[] arg)
        {
            SiAuto.Main.LogError(GetMessage(format, arg));
        }

        public void Error(Exception ex)
        {
            SiAuto.Main.LogError(GetExceptionMessage(ex));
        }
    }
}