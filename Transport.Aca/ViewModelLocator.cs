/*
  In App.xaml:
  <Application.Resources>
      <vm:ViewModelLocator xmlns:vm="clr-namespace:Transport.Aca"
                           x:Key="Locator" />
  </Application.Resources>
  
  In the View:
  DataContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"

  You can also use Blend to do all this with the tool's support.
  See http://www.galasoft.ch/mvvm
*/

using Microsoft.Practices.ServiceLocation;
using Transport.Aca.Dialogs.InitDialog;

namespace Transport.Aca
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
        public ViewModelLocator()
        {
        }

        public MainViewModel Main => ServiceLocator.Current.GetInstance<MainViewModel>();
        //public GraphViewModel Graph => ServiceLocator.Current.GetInstance<GraphViewModel>();
        public NodesListViewModel NodesList => ServiceLocator.Current.GetInstance<NodesListViewModel>();

        //public InitDialogViewModel InitDialog => ServiceLocator.Current.GetInstance<InitDialogViewModel>();

        //public AdjacencyMatrixViewModel AdjacencyMatrix => ServiceLocator.Current.GetInstance<AdjacencyMatrixViewModel>();

        //public DirectTravelersMatrixViewModel DirectTravelersMatrix => ServiceLocator.Current.GetInstance<DirectTravelersMatrixViewModel>();

        public static void Cleanup()
        {
            // TODO Clear the ViewModels
        }
    }
}