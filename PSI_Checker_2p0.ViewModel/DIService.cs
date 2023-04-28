using Microsoft.Extensions.DependencyInjection;
using PSI_Checker_2p0.Acquistion;
using PSI_Checker_2p0.FileHandler.FileManagers;
using PSI_Checker_2p0.Logger;
using PSI_Checker_2p0.Utils;
using PSI_Checker_2p0.ViewModel.ViewModels;
using System;
using System.Linq;

namespace PSI_Checker_2p0.ViewModel
{
    /// <summary>
    /// Singleton class which collects all Dependency Injection related class.
    /// </summary>
    public class DIService
    {
        private static DIService instance;
        public static DIService Instance
        {
            get
            {
                if (instance is null)
                    instance = new DIService();
                return instance;
            }
        }

        #region Shortcuts
        public static IFileManager File = DIService.Instance.Get<IFileManager>();

        public static ILogFactory Logger = DIService.Instance.Get<ILogFactory>();

        public static ITaskManager Task = DIService.Instance.Get<ITaskManager>();

        public static PsiConfigHandler PsiConfigs = DIService.Instance.Get<PsiConfigHandler>();
        #endregion

        private static IServiceProvider serviceProvider;

        public DIService()
        {
            SetupServices();
        }
        private void SetupServices()
        {
            ServiceCollection services = new ServiceCollection();
            BindShortcuts(services);
            BindValues(services);
            BindViewModels(services);
            serviceProvider = services.BuildServiceProvider();
        }

        private void BindShortcuts(ServiceCollection services)
        {
            services.AddSingleton<ILogFactory>(new BaseLogFactory(new ILogger[]
            {
                new ConsoleLogger(),
            }));
            services.AddSingleton<ITaskManager, TaskManager>();
            services.AddSingleton<IFileManager, FileManager>();
            services.AddSingleton<PsiConfigHandler, PsiConfigHandler>();
        }

        private void BindValues(ServiceCollection services)
        {
            services.AddSingleton<ScopeDeviceThread, ScopeDeviceThread>();
            services.AddSingleton<ControllerDeviceThread, ControllerDeviceThread>();
            services.AddSingleton<ImportantInfos, ImportantInfos>();
        }

        private void BindViewModels(ServiceCollection services)
        {
            services.AddSingleton(typeof(MenuVM), new MenuVM());
            services.AddSingleton(typeof(BiDirVM), new BiDirVM());
        }

        public T Get<T>() where T : class
        {
            try
            {
                return serviceProvider.GetServices<T>().Where((x) => x is T).First();
            }
            catch
            {
                return null;
            }
        }
    }
}
