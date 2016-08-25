using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Media;
using GalaSoft.MvvmLight;

namespace Transport.Aca3.ViewModels
{
    public class PathViewModel : ViewModelBase
    {
        #region Ctors

        public PathViewModel()
        {
            _nodes = new ObservableCollection<NodeViewModel>();
            Nodes = new ReadOnlyObservableCollection<NodeViewModel>(_nodes);

            _edges = new ObservableCollection<EdgeViewModel>();
            Edges = new ReadOnlyObservableCollection<EdgeViewModel>(_edges);
        }

        public PathViewModel(params NodeViewModel[] nodes)
            :this()
        {
            Append(nodes);
        }

        public PathViewModel(params EdgeViewModel[] edges)
            :this()
        {
            Append(edges);
        }

        #endregion

        #region Public Methods

        public PathViewModel Append(params NodeViewModel[] nodes)
        {
            if (!nodes.Any()) return this;

            var cur = Nodes.LastOrDefault();
            
            foreach (var next in nodes)
            {
                // если список узлов не был пуст
                if (cur != null)
                {
                    // ищем ребро смежной вершины
                    var edge = cur.Edges.FirstOrDefault(e => e.Dest == next);
                    if (edge != null)
                    {
                        // добавляем во временные списки
                        _nodes.Add(next);
                        _edges.Add(edge);
                    }
                    else
                    {
                        // выход или Exception
                        return this;
                    }
                }
                else
                {
                    // добавляем первый узел без ребра
                    _nodes.Add(next);
                }
                
                cur = next;
            }

            return this;
        }

        public PathViewModel Append(params EdgeViewModel[] edges)
        {
            if (!edges.Any()) return this;

            var cur = Edges.LastOrDefault();

            foreach (var next in edges)
            {
                if (cur != null)
                {
                    // проверяем смежность вершин
                    if (cur.Dest == next.Source)
                    {
                        // добавляем во временные списки
                        _nodes.Add(next.Dest);
                        _edges.Add(next);
                    }
                    else
                    {
                        // выход или Exception
                        return this;
                    }
                }
                else
                {
                    // добавляем первый узел
                    _nodes.Add(next.Source);
                    _nodes.Add(next.Dest);
                    _edges.Add(next);
                }

                cur = next;
            }

            return this;
        }

        public PathViewModel RemoveLast()
        {
            _nodes.RemoveAt(_nodes.Count);
            _edges.RemoveAt(_edges.Count);

            return this;
        }

        #endregion

        #region Properties

        private Color _color;
        public Color Color
        {
            get { return _color; }
            set { Set(ref _color, value); }
        }

        private readonly ObservableCollection<NodeViewModel> _nodes;
        public ReadOnlyObservableCollection<NodeViewModel> Nodes { get; }

        private readonly ObservableCollection<EdgeViewModel> _edges;
        public ReadOnlyObservableCollection<EdgeViewModel> Edges { get; }
        #endregion

    }
}
