using AutoMapper;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace JUtilSharp.BootStrapper
{
    public class BootStrapper
    {
        private static IUnityContainer container = null;

        public static void Start()
        {
            if (container != null)
            {
                throw new Exception("BootStrapper.Start() 只能被呼叫一次");
            }

            Assembly entryAssembly = Assembly.GetEntryAssembly();
            if(entryAssembly == null)
                entryAssembly = Assembly.GetCallingAssembly();

            List<AssemblyName> assemblyNames = GetAssemblyDependency(entryAssembly);

            // 註冊IOC
            container = new UnityContainer();
            foreach (var assemblyName in assemblyNames)
            {
                foreach (IConfigureContainer task in GetAllIConfigureContainer(assemblyName))
                    task.Execute(container);
            }
            ServiceLocator.SetLocatorProvider(() => new UnityServiceLocator(container));

            // 執行初始化
            foreach (var assemblyName in assemblyNames)
            {
                foreach (IBootstrapperTask task in GetAllIBootstrapperTask(assemblyName))
                    task.Execute();
            }

            // 註冊AutoMapper轉型設定
            foreach (var assemblyName in assemblyNames)
            {
                foreach (Profile profile in AutoMapperProfile(assemblyName))
                    Mapper.AddProfile(profile);
            }
        }

        public static void End()
        {
            if (container != null)
            {
                container.Dispose();
                container = null;
            }
        }

        #region Private Methods
        
        private static List<AssemblyName> GetAssemblyDependency(Assembly entryAssembly)
        {
            if (entryAssembly == null)
                throw new Exception("找不到程式進入點");

            List<AssemblyName> AssemblyNames = new List<AssemblyName>();

            CalculateAssemblyDependency(entryAssembly.GetName(), AssemblyNames);

            return AssemblyNames;
        }

        private static void CalculateAssemblyDependency(AssemblyName assemblyName, List<AssemblyName> AssemblyNames)
        {
            string[] excludeAssemblyNames = 
            {
                "System.",
                "Microsoft.",
                "mscorlib",
            };

            foreach (var excludeAssemblyName in excludeAssemblyNames)
            {
                if (assemblyName.Name.StartsWith(excludeAssemblyName)) return;
            }

            if (AssemblyNames.Contains(assemblyName))
                AssemblyNames.Remove(assemblyName);

            AssemblyNames.Insert(0, assemblyName);

            foreach (var referencedAssemblyName in Assembly.Load(assemblyName).GetReferencedAssemblies())
            {
                CalculateAssemblyDependency(referencedAssemblyName, AssemblyNames);
            }
        }

        private static IEnumerable<IConfigureContainer> GetAllIConfigureContainer(AssemblyName assemblyName)
        {
            Assembly assembly = Assembly.Load(assemblyName);

            Type tConfigureContainer = typeof(IConfigureContainer);

            IEnumerable<Type> types = assembly.GetTypes().Where(type => type.GetInterfaces().Contains(tConfigureContainer));

            // Order由小到大
            IEnumerable<IConfigureContainer> tasks = types.Select(type => (Activator.CreateInstance(type) as IConfigureContainer)).OrderBy(task => task.Order);

            return tasks;
        }

        private static IEnumerable<IBootstrapperTask> GetAllIBootstrapperTask(AssemblyName assemblyName)
        {
            Assembly assembly = Assembly.Load(assemblyName);

            Type tBootstrapperTask = typeof(IBootstrapperTask);

            IEnumerable<Type> types = assembly.GetTypes().Where(type => type.GetInterfaces().Contains(tBootstrapperTask));

            // Order由小到大
            IEnumerable<IBootstrapperTask> tasks = types.Select(type => (ServiceLocator.Current.GetInstance(type) as IBootstrapperTask)).OrderBy(task => task.Order);

            return tasks;
        }

        private static IEnumerable<Profile> AutoMapperProfile(AssemblyName assemblyName)
        {
            Assembly assembly = Assembly.Load(assemblyName);

            Type tProfile = typeof(Profile);

            IEnumerable<Type> types = assembly.GetTypes().Where(type => type != tProfile && tProfile.IsAssignableFrom(type));

            IEnumerable<Profile> profiles = types.Select(type => (ServiceLocator.Current.GetInstance(type) as Profile));

            return profiles;
        }

        #endregion
    }
}
