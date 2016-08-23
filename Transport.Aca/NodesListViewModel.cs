using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Transport.Aca.Model;

namespace Transport.Aca
{
    public class NodesListViewModel : ViewModelBase
    {
        private readonly IGraph _graph;

        private Node _selectedNode;

        #region private commands

        private readonly Lazy<RelayCommand> _addNodeCommand;
        private readonly Lazy<RelayCommand> _removeNodeCommand;

        #endregion

        public NodesListViewModel(IGraph graph)
        {
            _graph = graph;
            _addNodeCommand = new Lazy<RelayCommand>(() => new RelayCommand(AddNode));
            _removeNodeCommand = new Lazy<RelayCommand>(() => new RelayCommand(RemoveCommand));
        }

        public ICommand AddNodeCommand => _addNodeCommand.Value;

        private void AddNode()
        {
            _graph.AddNode(new Node());
        }

        public ICommand RemoveNodeCommand => _removeNodeCommand.Value;

        private void RemoveCommand()
        {
            _graph.RemoveNode(SelectedNode);
        }

        public List<Node> Nodes => _graph.Nodes.ToList();

        public Node SelectedNode
        {
            get { return _selectedNode; }
            set { Set(ref _selectedNode, value); }
        }
    }
}
