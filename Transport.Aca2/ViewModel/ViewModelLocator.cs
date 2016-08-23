/*
  In App.xaml:
  <Application.Resources>
      <vm:ViewModelLocator xmlns:vm="clr-namespace:Transport.Aca2"
                           x:Key="Locator" />
  </Application.Resources>
  
  In the View:
  DataContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"

  You can also use Blend to do all this with the tool's support.
  See http://www.galasoft.ch/mvvm
*/

using Microsoft.Practices.ServiceLocation;

namespace Transport.Aca2.ViewModel
{
    /// <summary>
    /// This class contains static references to all the view models in the
    /// application and provides an entry point for the bindings.
    /// </summary>
    public class ViewModelLocator
    {
        //public MainViewModel Main => ServiceLocator.Current.GetInstance<MainViewModel>();
        public AdjacencyMatrixViewModel AdjacencyMatrix => ServiceLocator.Current.GetInstance<AdjacencyMatrixViewModel>();


        public static void Cleanup()
        {
            // TODO Clear the ViewModels
        }
    }
}