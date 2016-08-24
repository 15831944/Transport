using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;
using MvvmDialogs;
using Transport.Aca3.Models;
using Transport.Aca3.Services;
using Transport.Aca3.ViewModels;

namespace Transport.Aca3
{
    internal class UnityContainerBootstrapper
    {
        public UnityContainerBootstrapper()
        {
            var unityContainer = new UnityContainer();
            RegisterTypes(unityContainer);
            ServiceLocator.SetLocatorProvider(() => new UnityServiceLocator(unityContainer));
        }

        private static void RegisterTypes(IUnityContainer unityContainer)
        {
            // ViewModels
            unityContainer.RegisterType<MainViewModel>();
            unityContainer.RegisterType<AcaConfigViewModel>();
            unityContainer.RegisterType<DataSourceViewModel>();
            unityContainer.RegisterType<MapViewModel>();

            // Models
            unityContainer.RegisterType<DataSource>(new ContainerControlledLifetimeManager());
            unityContainer.RegisterType<AcaAlgorithm>(new TransientLifetimeManager());
            unityContainer.RegisterType<AcaConfiguration>(new ContainerControlledLifetimeManager());

            // Services
            unityContainer.RegisterType<IDialogService>(new InjectionFactory(c => new DialogService()));
            unityContainer.RegisterType(typeof(IDataAccessService), typeof(DataAccessService), new ContainerControlledLifetimeManager());

        }

        public void Run()
        {
            new Views.MainWindow().ShowDialog();
        }
    }
}
