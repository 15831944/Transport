using System.Collections.ObjectModel;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Transport.Aca3.Services;

namespace Transport.Aca3.ViewModels
{
    public class MapViewModel : ViewModelBase
    {
        private readonly IDataAccessService _dataAccessService;

        public MapViewModel(IDataAccessService dataAccessService)
        {
            _dataAccessService = dataAccessService;
            _buildMap = new Lazy<RelayCommand>(() => new RelayCommand(InvokeBuildMap));
        }


        #region Properties

        public ObservableCollection<VisualItemViewModel> VisualItems { get; } =
            new ObservableCollection<VisualItemViewModel>();

        private bool _isItemsLoaded;

        public bool IsItemsLoaded
        {
            get { return _isItemsLoaded; }
            set { Set(ref _isItemsLoaded, value); }
        }

        #endregion

        #region Commands

        private readonly Lazy<RelayCommand> _buildMap;
        public ICommand BuildMapCommand => _buildMap.Value;

        private void InvokeBuildMap()
        {
            var nodes = _dataAccessService.GetGraphNodes();
            GeoPositionToScreenPosition(nodes);
            var edges = nodes.SelectMany(node => node.Edges).ToList();

            nodes.ForEach(n => VisualItems.Add(n));
            edges.ForEach(e => VisualItems.Add(e));

            IsItemsLoaded = true;
        }

        #endregion

        #region Helpers

        private static void GeoPositionToScreenPosition(List<NodeViewModel> nodes)
        {
            var maxLat = 0.0;
            var minLon = double.MaxValue;
            var minDist = double.MaxValue;

            foreach (var node in nodes)
            {
                if (node.Lat > Math.Abs(maxLat)) maxLat = Math.Abs(node.Lat);
                if (node.Lon < Math.Abs(minLon)) minLon = Math.Abs(node.Lon);

                foreach (var edge in node.Edges)
                {
                    var dist = LineLenght(edge.Source.Lon, edge.Dest.Lon, edge.Source.Lat, edge.Dest.Lat);
                    if (dist < minDist) minDist = dist;
                }
            }

            const double baseMinDest = 15.0;
            var zoom = baseMinDest / minDist;

            foreach (var node in nodes)
            {
                node.Center = new Point((node.Lon - minLon) * zoom, (-node.Lat + maxLat) * zoom);
            }
        }

        private static double LineLenght(double x1, double x2, double y1, double y2)
        {
            return Math.Sqrt(Math.Pow(x2 - x1, 2) + Math.Pow(y2 - y1, 2));
        }

        #endregion

    }
}
