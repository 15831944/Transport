using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;

namespace Transport.Aca3.ViewModels
{
    public class PathlistViewModel : ViewModelBase
    {
        private ObservableCollection<PathViewModel> _path;
        public ObservableCollection<PathViewModel> Paths => _path ?? (_path = new ObservableCollection<PathViewModel>());

        private PathViewModel _selectedPath;
        public PathViewModel SelectedPath
        {
            get { return _selectedPath; }
            set { Set(ref _selectedPath, value); }
        }
    }
}
