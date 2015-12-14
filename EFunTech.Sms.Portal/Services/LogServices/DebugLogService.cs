using System;
using System.Collections.Generic;
using System.Linq;
using JUtilSharp.Database;
using EFunTech.Sms.Schema;
using System.Reflection;
using System.Diagnostics;
using System.Data.Entity;
using Microsoft.AspNet.Identity;
using System.Web;
using System.Security.Principal;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using System.Security.Claims;

namespace EFunTech.Sms.Portal
{
    public class DebugLogService : ILogService
    {
        private string entryAssembly;
        private string host;
        private string userName;

        public DebugLogService()
        {
            this.entryAssembly = GetEntryAssemblyName();
            this.host = GetHost();
            this.userName = GetUserName();
        }

        private void Write(LogLevel logLevel, string format, params object[] arg)
        {
            StackFrame frame = new StackFrame(2);
            MethodBase method = frame.GetMethod();

            string message = arg.Length != 0 ? string.Format(format, arg) : format;

            LogItem logItem = new LogItem();
            logItem.EntryAssembly = this.entryAssembly;
            logItem.Class = method.ReflectedType.Name;
            logItem.Method = method.Name;
            logItem.LogLevel = logLevel;
            logItem.Message = message;
            logItem.StackTrace = null;
            logItem.CreatedTime = DateTime.UtcNow;
            logItem.UserName = userName;
            logItem.Host = this.host;

            // 如何解決執行程式碼 Debug.WriteLine("訊息") 時在【輸出】視窗沒有輸出資料
            // https://support.microsoft.com/zh-tw/kb/2706882
            WriteLine(logItem);
        }

        private void WriteLine(LogItem logItem)
        {
            System.Diagnostics.Debug.WriteLine("LogItem");

            foreach(var property in logItem.GetType().GetProperties())
            {
                System.Diagnostics.Debug.WriteLine("\t {0} = {1}", property.Name, property.GetValue(logItem));
            }

            System.Diagnostics.Debug.WriteLine(string.Empty);
        }

        private void Write(Exception ex)
        {
            StackFrame frame = new StackFrame(2);
            MethodBase method = frame.GetMethod();

            string message = GetDetailExceptionMessage(ex);

            LogItem logItem = new LogItem();
            logItem.EntryAssembly = this.entryAssembly;
            logItem.Class = method.ReflectedType.Name;
            logItem.Method = method.Name;
            logItem.LogLevel = LogLevel.Error;
            logItem.Message = message;
            logItem.StackTrace = GetStackTrace(ex);
            logItem.CreatedTime = DateTime.UtcNow;
            logItem.UserName = userName;
            logItem.Host = this.host;

            WriteLine(logItem);
        }

        private string GetStackTrace(Exception ex)
        {
            string stackTrace = null;

            Exception e = ex;

            while (e != null)
            {
                stackTrace = e.StackTrace;
                if (stackTrace != null)
                    break;

                e = e.InnerException;
            }

            return stackTrace ?? string.Empty;
        }

        private string GetDetailExceptionMessage(Exception ex)
        {
            List<Exception> es = new List<Exception>();

            Exception e = ex;

            while (e != null)
            {
                es.Add(e);

                e = e.InnerException;
            }

            es.Reverse();

            return string.Join("\r\n", es.Select((err, i) => string.Format("Exception{0} = {1}", i, err.Message)));
        }

        public void Debug(string format, params object[] arg)
        {
            Write(LogLevel.Debug, format, arg);
        }

        public void Info(string format, params object[] arg)
        {
            Write(LogLevel.Info, format, arg);
        }

        public void Warn(string format, params object[] arg)
        {
            Write(LogLevel.Warn, format, arg);
        }

        public void Error(string format, params object[] arg)
        {
            Write(LogLevel.Error, format, arg);
        }

        public void Error(Exception ex)
        {
            Write(ex);
        }

        private string GetEntryAssemblyName()
        {
            Assembly entryAssembly = null;

            if (HttpContext.Current == null ||
                HttpContext.Current.ApplicationInstance == null)
            {
                entryAssembly = Assembly.GetEntryAssembly();
            }
            else
            {
                var type = HttpContext.Current.ApplicationInstance.GetType();
                while (type != null && type.Namespace == "ASP")
                {
                    type = type.BaseType;
                }
                entryAssembly = type == null ? null : type.Assembly;
            }

            return entryAssembly == null ? string.Empty : entryAssembly.GetName().Name;
        }

        private string GetHost()
        {
            if (HttpContext.Current == null ||
                HttpContext.Current.ApplicationInstance == null)
            {
                return string.Empty;
            }
            else
            {
                return HttpContext.Current.Request.UserHostAddress;
            }
        }

        private string GetUserName()
        {
            if (HttpContext.Current == null ||
                HttpContext.Current.ApplicationInstance == null)
            {
                return "Hangfire";
            }
            else
            {
                IOwinContext owinContext = HttpContext.Current.GetOwinContext();
                if (owinContext == null) return "Guest";

                IAuthenticationManager authenticationManager = owinContext.Authentication;
                if (authenticationManager == null) return "Guest";

                ClaimsPrincipal claimsPrincipal = authenticationManager.User;
                if (claimsPrincipal == null) return "Guest";

                IIdentity identity = claimsPrincipal.Identity;
                if (identity == null) return "Guest";

                return identity.GetUserName();
            }
        }
    }
}
