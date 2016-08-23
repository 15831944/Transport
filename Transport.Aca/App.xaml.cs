using System.Windows;

namespace Transport.Aca
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Bootstrapper(object sender, StartupEventArgs e)
        {
            new UnityContainerBootstrapper().Run();
            Shutdown();
        }
    }
}
