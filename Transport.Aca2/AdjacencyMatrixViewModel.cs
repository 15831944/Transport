using System;
using System.Collections.Generic;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Gu.Wpf.DataGrid2D;

namespace Transport.Aca2
{
    public class AdjacencyMatrixViewModel : ViewModelBase
    {
        private readonly Graph<double> _graph;
        private readonly Lazy<ICommand> _addNodeCommand;
        private readonly Lazy<ICommand> _removeSelectedNodeCommand;

        public AdjacencyMatrixViewModel()
        {
            _graph = new Graph<double>();
            _addNodeCommand = new Lazy<ICommand>(() => new RelayCommand(AddNode));
            _removeSelectedNodeCommand = new Lazy<ICommand>(() => new RelayCommand<int>(RemoveSelectedNode, CanRemoveNode));
        }

        public IEnumerable<IEnumerable<double>> Table => _graph.Rows;

        public ICommand AddNodeCommand => _addNodeCommand.Value;

        private void AddNode()
        {
            _graph.AddNode();
        }

        public ICommand RemoveSelectedNodeCommand => _removeSelectedNodeCommand.Value;

        private void RemoveNode(int indx)
        {
            _graph.RemoveNode(indx);
        }

        private void RemoveSelectedNode(int a)
        {
            if (SelectedIndex != null)
            {
                RemoveNode(SelectedIndex.Value.Row);
            }
        }
        private bool CanRemoveNode(int arg)
        {
            return SelectedIndex.HasValue && _graph.NodesCount > 1 && SelectedIndex.Value.Row < _graph.NodesCount;
        }
         
        private RowColumnIndex? _selectedIndex;
        public RowColumnIndex? SelectedIndex
        {
            get { return _selectedIndex; }
            set { Set(ref _selectedIndex, value); }
        }
    }
}
