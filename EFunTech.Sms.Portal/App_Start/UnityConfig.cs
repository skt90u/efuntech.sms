using System;
using Microsoft.Practices.Unity;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using EFunTech.Sms.Schema;
using System.Data.Entity;
using EFunTech.Sms.Portal.Controllers;

using JUtilSharp.Database;
using System.Web;
using System.Security.Principal;

namespace EFunTech.Sms.Portal.App_Start
{
    /// <summary>
    /// Specifies the Unity configuration for the main container.
    /// </summary>
    public class UnityConfig
    {
        #region Unity Container
        private static Lazy<IUnityContainer> container = new Lazy<IUnityContainer>(() =>
        {
            var container = new UnityContainer();
            // PerRequestLifetimeManager
            RegisterTypes<HierarchicalLifetimeManager>(container, isBackground: false);
            return container;
        });

        private static Lazy<IUnityContainer> backgroundContainer = new Lazy<IUnityContainer>(() =>
        {
            var container = new UnityContainer();
            RegisterTypes<HierarchicalLifetimeManager>(container, isBackground: true);
            return container;
        });

        /// <summary>
        /// Gets the configured Unity container.
        /// </summary>
        public static IUnityContainer GetConfiguredContainer()
        {
            return container.Value;
        }

        public static IUnityContainer GetConfiguredBackgroundContainer()
        {
            return backgroundContainer.Value;
        }
        #endregion

        public static void RegisterTypes<TLifetimeManager>(IUnityContainer container, bool isBackground) 
            where TLifetimeManager : LifetimeManager, new()
        {
            container.RegisterType<ISystemParameters, SystemParameters>(new TLifetimeManager());

            if (!isBackground)
            {
                container.RegisterType<ApplicationDbContext>(new InjectionFactory(c =>
                {
                    var context = (ApplicationDbContext)HttpContext.Current.Items["__dbcontext"];
                    if (context == null)
                    {
                        context = new ApplicationDbContext();
                        HttpContext.Current.Items["__dbcontext"] = context;
                    }
                    return context;
                })); 
            }
            

            container.RegisterType<DbContext, ApplicationDbContext>(new TLifetimeManager());
            container.RegisterType<IUnitOfWork, UnitOfWork>(new TLifetimeManager(), new InjectionConstructor(typeof(DbContext)));

            container.RegisterType<UserManager<ApplicationUser>>(new TLifetimeManager());
            container.RegisterType<IUserStore<ApplicationUser>, UserStore<ApplicationUser>>(new TLifetimeManager());

            /*
            container.RegisterType<IAuthenticationManager>(
                new InjectionFactory(c => HttpContext.Current.GetOwinContext().Authentication));
             * public ApplicationSignInManager(ApplicationUserManager userManager, IAuthenticationManager authenticationManager)
             * 
             * And have only one constructor for SignInManager:
             */
            

            //container.RegisterType<IContactManagerService, ContactManagerService>(
            //    new TLifetimeManager());
            //container.RegisterType<IDepartmentManager, DepartmentManager>(
            //    new TLifetimeManager());
            //container.RegisterType<IDepartmentPointManager, DepartmentPointManager>(
            //    new TLifetimeManager());
            //container.RegisterType<IRecurringSMS, RecurringSMS>(
            //    new TLifetimeManager());
            //container.RegisterType<ISearchMemberSend, SearchMemberSend>(
            //    new TLifetimeManager());
            //container.RegisterType<ISectorStatistics, SectorStatistics>(
            //    new TLifetimeManager());
            //container.RegisterType<ISendMessage, SendMessage>(
            //    new TLifetimeManager());
            //container.RegisterType<ISendParamMessage, SendParamMessage>(
            //    new TLifetimeManager());
            //container.RegisterType<ISMS_Setting, SMS_Setting>(
            //    new TLifetimeManager());

            container.RegisterType<AccountController>(new InjectionConstructor());

            //-----------------------------------------------------------------------------
            #region LogServices
           
            // 20151013 Norman, 以下方式已不合適，會造成跨Thread存取同一個DbContext
            //container.RegisterType<DbLogService>(
            //    new TLifetimeManager(),
            //    new InjectionFactory(c =>
            //    {
            //        IUnitOfWork unitOfWork = container.Resolve<IUnitOfWork>();
            //        return new DbLogService(GetUserName(isBackground), unitOfWork);
            //    }));

            container.RegisterType<DbLogService>(new TLifetimeManager(), new InjectionConstructor(typeof(IUnitOfWork)));
#if DEBUG
            // 測試環境
            //container.RegisterType<ILogService, SmartInspectLogService>(
            //    new TLifetimeManager());

            container.RegisterType<ILogService, DbLogService>(
                new TLifetimeManager());
#else
            // 正式環境
            container.RegisterType<ILogService, DbLogService>(
                new TLifetimeManager());
#endif

            #endregion

            /*
            IUnityContainer myContainer = new UnityContainer();
            myContainer.Configure<InjectedMembers>()
              .ConfigureInjectionFor<MyObject>(
                new InjectionConstructor(12, "Hello Unity!"),
                new InjectionProperty("MyProperty"),
                new InjectionProperty("MyStringProperty", "SomeText"),
                new InjectionMethod("InitializeMe", 42.0, 
                        new ResolvedParameter(typeof(ILogger), "SpecialLogger"))
              );
             * 
             var repository = IOC.Container.Resolve<IRepository>("Client");
             var clientModel = IOC.Container.Resolve<ClientModel>(new ParameterOverrides<ClientModel> { {"dataAccess", repository } } );
             * 
             IOC.Container.RegisterType<IRepository, GenericRepository>("Client");
             IOC.Container.Resolve<IRepository>("Client");
             */
        }

        

        
    }
}
