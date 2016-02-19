// http://hangfirechinese.readthedocs.org/en/latest/deployment-to-production/index.html
using System;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.Google;
using Owin;
using EFunTech.Sms.Portal.Models;
using EFunTech.Sms.Schema;
using System.Threading.Tasks;
using System.Configuration;
using Microsoft.AspNet.Identity.EntityFramework;
using EFunTech.Sms.Simulation;
using Hangfire;
using System.Web.Hosting;
using System.Collections.Generic;

using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;
using Hangfire.Unity;
using Hangfire.Common;
using Hangfire.Server;
using EFunTech.Sms.Portal.App_Start;
using Hangfire.SqlServer;
using JUtilSharp.Database;
using System.Data.Entity;
using Hangfire.Logging;
using System.Security.Claims;

namespace EFunTech.Sms.Portal
{
	/// <summary>
    /// 使用 Hangfire 來處理非同步的工作
    /// http://www.dotblogs.com.tw/rainmaker/archive/2015/08/19/153169.aspx
    /// 
    /// Hangfire
    /// http://hangfire.io/
	/// </summary>
    public partial class Startup
    {
        public void ConfigureHangFire(IAppBuilder app) 
        {
            if (HangfireBootstrapper.Instance.Started)
            {
                app.UseHangfireDashboard("/ds", new DashboardOptions
                {
                    AuthorizationFilters = new[] { new RolesAuthorizationFilter(new Role[]{
                        Role.Administrator
                    }) }
                });
            }
        }
	}

    public class RolesAuthorizationFilter : Hangfire.Dashboard.IAuthorizationFilter
    {
        private Role[] roles;

        public RolesAuthorizationFilter(Role[] roles)
        {
            this.roles = roles;
        }

        public bool Authorize(IDictionary<string, object> owinEnvironment)
        {
            var claimsPrincipal = owinEnvironment["server.User"] as ClaimsPrincipal;

            foreach(var role in roles)
                if (claimsPrincipal.IsInRole(role.ToString()))
                    return true;

            return false;
        }
    }

    public class HangfireBootstrapper : IRegisteredObject
    {
        public static readonly HangfireBootstrapper Instance = new HangfireBootstrapper();

        private readonly object _lockObject = new object();
        private bool _started;

        private BackgroundJobServer _backgroundJobServer;

        private HangfireBootstrapper()
        {
        }

        public bool Started
        {
            get { return _started; }
        }

        public void Start()
        {
            lock (_lockObject)
            {
				
                if (_started) return;
                _started = true;
                
                HostingEnvironment.RegisterObject(this);

                var options = new SqlServerStorageOptions
                {
                    PrepareSchemaIfNecessary = true,

                    // 每15秒，查詢是否有需要執行的Job
                    // You can adjust the polling interval, but, as always, lower intervals can harm your SQL Server, and higher interval produce too much latency, so be careful.
                    QueuePollInterval = TimeSpan.FromSeconds(15), // Default value

                    // 不知道怎麼使用
                    // InvisibilityTimeout = TimeSpan.FromMinutes(30) // default value
                };

                GlobalConfiguration.Configuration
                    // .UseSqlServerStorage("ApplicationDbContext") // 使用下面的方式，要不然使用別的config，這裡的連線資訊也要跟著調整
                    .UseSqlServerStorage(new ApplicationDbContext().Database.Connection.ConnectionString, options);

                //GlobalConfiguration.Configuration.UseLogProvider(new HangfireLogProvider()); // 暫時先關閉，除非要偵錯 Hangfire

                // http://docs.hangfire.io/en/latest/background-methods/using-ioc-containers.html
                var container = UnityConfig.GetConfiguredBackgroundContainer() as UnityContainer;
                var jobActivator = new MyUnityJobActivator(container);
                GlobalConfiguration.Configuration.UseActivator(jobActivator);

                // http://docs.hangfire.io/en/latest/background-processing/dealing-with-exceptions.html
                // 預設重試次數
                GlobalJobFilters.Filters.Add(new AutomaticRetryAttribute { Attempts = 5 });
                // 手動指定重試次數 
                // [AutomaticRetry(Attempts = 10000)]

                GlobalJobFilters.Filters.Add(new ChildContainerPerJobFilterAttribute(jobActivator));

                RecurringJob.AddOrUpdate<EfSmsBackgroundJob>("CheckMonthlyAllotPoint", x => x.CheckMonthlyAllotPoint(), Cron.Minutely);
                RecurringJob.AddOrUpdate<EfSmsBackgroundJob>("SendSMS", x => x.SendSMS(), Cron.Minutely);
                RecurringJob.AddOrUpdate<EfSmsBackgroundJob>("GetDeliveryReport", x => x.GetDeliveryReport(), Cron.Minutely);
                RecurringJob.AddOrUpdate<EfSmsBackgroundJob>("HandleDeliveryReportTimeout", x => x.HandleDeliveryReportTimeout(), Cron.Minutely);
                RecurringJob.AddOrUpdate<EfSmsBackgroundJob>("HouseKeeping", x => x.HouseKeeping(), Cron.Minutely);

                // 建立Background JobSserver 來處理 Job
                _backgroundJobServer = new BackgroundJobServer();
            }
        }

        public void Stop()
        {
            lock (_lockObject)
            {
                if (_backgroundJobServer != null)
                {
                    _backgroundJobServer.Dispose();
                }
                HostingEnvironment.UnregisterObject(this);
            }
        }

        void IRegisteredObject.Stop(bool immediate)
        {
            Stop();
        }
    }

    public class ApplicationPreload : System.Web.Hosting.IProcessHostPreloadClient
    {
        public void Preload(string[] parameters)
        {
            HangfireBootstrapper.Instance.Start();
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////

    public class HangfireLog : ILog
    {
        private DbContext context;
        private IUnitOfWork unitOfWork;
        private ILogService logService;

        public HangfireLog(string name)
        {
            this.context = new ApplicationDbContext();
            this.unitOfWork = new UnitOfWork(this.context);
            this.logService = new DbLogService(this.unitOfWork, "HangfireLog:" + name);
        }
        
        public bool Log(Hangfire.Logging.LogLevel logLevel, Func<string> messageFunc, Exception exception = null)
        {
            try
            {
                string message = messageFunc != null ? messageFunc() : string.Empty;

                if (!string.IsNullOrEmpty(message))
                {
                    switch (logLevel)
                    {
                        case Hangfire.Logging.LogLevel.Trace:
                        case Hangfire.Logging.LogLevel.Debug:
                        case Hangfire.Logging.LogLevel.Info:
                            {
                                this.logService.Debug(message);
                            } break;
                        case Hangfire.Logging.LogLevel.Warn:
                            {
                                this.logService.Debug(message);
                            } break;
                        case Hangfire.Logging.LogLevel.Error:
                        case Hangfire.Logging.LogLevel.Fatal:
                            {
                                this.logService.Error(message);
                            } break;
                    }
                }

                if (exception != null)
                {
                    this.logService.Error(exception);
                }

                return true;
            }
            catch
            {
                return false;
            }
        }
    }

    public class HangfireLogProvider : ILogProvider
    {
        private Dictionary<string, HangfireLog> dict = new Dictionary<string, HangfireLog>();

        public ILog GetLogger(string name)
        {
            if (!dict.ContainsKey(name))
            {
                dict.Add(name, new HangfireLog(name));
            }

            return dict[name];
        }
    }

   ////////////////////////////////////////////////////////////////////////////////////////

    public class ChildContainerPerJobFilterAttribute : JobFilterAttribute, IServerFilter
    {
        public ChildContainerPerJobFilterAttribute(MyUnityJobActivator unityJobActivator)
        {
            UnityJobActivator = unityJobActivator;
        }

        public MyUnityJobActivator UnityJobActivator { get; set; }

        public void OnPerformed(PerformedContext filterContext)
        {
            UnityJobActivator.DisposeChildContainer();
        }

        public void OnPerforming(PerformingContext filterContext)
        {
            UnityJobActivator.CreateChildContainer();
        }
    }

    public class MyUnityJobActivator : JobActivator
    {
        [ThreadStatic]
        private static IUnityContainer childContainer;

        public MyUnityJobActivator(IUnityContainer container)
        {
            // Register dependencies
            //container.RegisterType<MyService>(new HierarchicalLifetimeManager());
            //container.RegisterType<BackgroundService>(new HierarchicalLifetimeManager());
            
            Container = container;
        }

        public IUnityContainer Container { get; set; }

        public override object ActivateJob(Type jobType)
        {
            return childContainer.Resolve(jobType);
        }

        public void CreateChildContainer()
        {
            childContainer = Container.CreateChildContainer();
        }

        public void DisposeChildContainer()
        {
            childContainer.Dispose();
            childContainer = null;
        }
    }
}