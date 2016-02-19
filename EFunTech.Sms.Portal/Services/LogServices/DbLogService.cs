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
using System.Web.Http.Filters;

namespace EFunTech.Sms.Portal
{
    public class DbLogService : ILogService
    {
        private IUnitOfWork unitOfWork;
        private IRepository<LogItem> repository;
        private string entryAssembly;
        private string host;

        private string _userName;
        private string userName 
        {
            get 
            {
                if (string.IsNullOrEmpty(_userName)) // BasicAuth 在初始化會取不到使用者名稱
                {
                    _userName = GetUserName();
                }
                return _userName;
            }
            set
            {
                _userName = value;
            }
        }

        public DbLogService(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
            this.repository = unitOfWork.Repository<LogItem>();
            this.entryAssembly = GetEntryAssemblyName();
            this.host = GetHost();
            this.userName = GetUserName(); // BasicAuth 在此會取不到使用者名稱
        }

        public DbLogService(IUnitOfWork unitOfWork, string userName)
        {
            this.unitOfWork = unitOfWork;
            this.repository = unitOfWork.Repository<LogItem>();
            this.entryAssembly = GetEntryAssemblyName();
            this.host = GetHost();
            this.userName = userName; 
        }

        private void Write(LogLevel logLevel, string format, params object[] arg)
        {
            try
            {
                var frame = new StackFrame(2);
                var method = frame.GetMethod();

                string message = arg.Length != 0 ? string.Format(format, arg) : format;

                var logItem = new LogItem();
                logItem.EntryAssembly = this.entryAssembly;
                logItem.Class = method.ReflectedType.Name;
                logItem.Method = method.Name;
                logItem.LogLevel = logLevel;
                logItem.Message = message;
                logItem.StackTrace = null;
                logItem.CreatedTime = DateTime.UtcNow;
                logItem.UserName = userName;
                logItem.Host = this.host;
                logItem.FileName = frame.GetFileName();
                logItem.FileLineNumber = frame.GetFileLineNumber();

                this.repository.Insert(logItem);
            }
            catch (Exception exception)
            {
                (new DebugLogService()).Error(exception);
                throw;
            }
        }

        private void Write(Exception ex)
        {
            try
            {
                var frame = new StackFrame(2);
                var method = frame.GetMethod();

                string message = GetDetailExceptionMessage(ex);

                var logItem = new LogItem();
                logItem.EntryAssembly = this.entryAssembly;
                logItem.Class = method.ReflectedType.Name;
                logItem.Method = method.Name;
                logItem.LogLevel = LogLevel.Error;
                logItem.Message = message;
                logItem.StackTrace = GetStackTrace(ex);
                logItem.CreatedTime = DateTime.UtcNow;
                logItem.UserName = userName;
                logItem.Host = this.host;
                logItem.FileName = frame.GetFileName();
                logItem.FileLineNumber = frame.GetFileLineNumber();

                this.repository.Insert(logItem);
            }
            catch (Exception exception)
            {
                (new DebugLogService()).Error(exception);
                throw;
            }
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
            var es = new List<Exception>();

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
                try
                {
                    return HttpContext.Current.Request.UserHostAddress;
                }
                catch
                {
                    return string.Empty;
                }
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
                try
                {
                    IPrincipal user = HttpContext.Current.User;

                    return user.Identity.GetUserName();
                }
                catch(Exception ex)
                {
                    (new DebugLogService()).Error(ex);
                    return null;
                }
       
                //IOwinContext owinContext = HttpContext.Current.GetOwinContext();
                //if (owinContext == null) return "Guest";

                //IAuthenticationManager authenticationManager = owinContext.Authentication;
                //if (authenticationManager == null) return "Guest";

                //ClaimsPrincipal claimsPrincipal = authenticationManager.User;
                //if (claimsPrincipal == null) return "Guest";

                //IIdentity identity = claimsPrincipal.Identity;
                //if (identity == null) return "Guest";

                //return identity.GetUserName();
            }
        }
    }
}
