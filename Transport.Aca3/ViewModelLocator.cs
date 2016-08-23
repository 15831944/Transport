/*
  In App.xaml:
  <Application.Resources>
      <vm:ViewModelLocator xmlns:vm="clr-namespace:Transport.Aca3"
                           x:Key="Locator" />
  </Application.Resources>
  
  In the View:
  DataContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"

  You can also use Blend to do all this with the tool's support.
  See http://www.galasoft.ch/mvvm
*/

using Microsoft.Practices.ServiceLocation;
using Transport.Aca3.ViewModels;

namespace Transport.Aca3
{
    /// <summary>
    /// This class contains static references to all the view models in the
    /// application and provides an entry point for the bindings.
    /// </summary>
    public class ViewModelLocator
    {
        /// <summary>
        /// Initializes a new instance of the ViewModelLocator class.
        /// </summary>
        public MainViewModel Main => ServiceLocator.Current.GetInstance<MainViewModel>();

        public AcaConfigViewModel AcaConfig => ServiceLocator.Current.GetInstance<AcaConfigViewModel>();

        public DataSourceViewModel DataSource => ServiceLocator.Current.GetInstance<DataSourceViewModel>();

        public AcaConstrainsViewModel AcaConstrains => ServiceLocator.Current.GetInstance<AcaConstrainsViewModel>();

        public MapViewModel Map => ServiceLocator.Current.GetInstance<MapViewModel>();

        public static void Cleanup()
        {
            // TODO Clear the ViewModels
        }
    }
}