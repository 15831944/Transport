﻿using System.Windows;

namespace Transport.Aca2
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            new UnityContainerBootstrapper().Run();
            Shutdown();
        }
    }
}
