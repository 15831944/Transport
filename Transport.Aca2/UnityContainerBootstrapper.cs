using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;
using Transport.Aca2;

namespace Transport.Aca2
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
            unityContainer.RegisterType<AdjacencyMatrixViewModel>();

            // Services
            //unityContainer.RegisterType(typeof(IGraphService), typeof(Matrix), new ContainerControlledLifetimeManager());
        }

        public void Run()
        {
            new MainWindow().ShowDialog();
        }
    }
}
