using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;
using Transport.Aca.Dialogs;
using Transport.Aca.Dialogs.InitDialog;
using Transport.Aca.Model;
using Network = Transport.Aca.Algorithm.Network;

namespace Transport.Aca
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
            unityContainer.RegisterType<MainWindow>();
            unityContainer.RegisterType<GraphViewModel>();
            unityContainer.RegisterType<NodesListViewModel>();
            unityContainer.RegisterType<InitDialogViewModel>();
            unityContainer.RegisterType<AdjacencyMatrixViewModel>();
            unityContainer.RegisterType<DirectTravelersMatrixViewModel>();

            // Services
            unityContainer.RegisterType(typeof(IGraph), typeof(Graph), new ContainerControlledLifetimeManager());
            unityContainer.RegisterType(typeof (IWindowDialogService), typeof (WindowDialogService), new TransientLifetimeManager());
            unityContainer.RegisterType(typeof (IFileDialogService), typeof (FileDialogService), new TransientLifetimeManager());
        }

        public void Run()
        {
            new MainWindow().ShowDialog();
        }
    }
}
