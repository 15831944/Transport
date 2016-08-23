using System;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Microsoft.Practices.ServiceLocation;
using Transport.Aca.Algorithm;
using Transport.Aca.Dialogs;
using Transport.Aca.Dialogs.InitDialog;
using Transport.Aca.Services;

namespace Transport.Aca
{
    public class MainViewModel : ViewModelBase
    {
        private readonly IWindowDialogService _windowDialogService;
        private readonly INetwork _network;
        private readonly Lazy<RelayCommand> _initDataCommand;

        public MainViewModel()
        {
            _windowDialogService = new WindowDialogService();
            _initDataCommand = new Lazy<RelayCommand>(() => new RelayCommand(InvokeOpenInitDialog));
        }

        public RelayCommand InitDataCommand => _initDataCommand.Value;

        private void InvokeOpenInitDialog()
        {
            var result = _windowDialogService.ShowDialog("", InitDialogViewModel);

            if (result == null || !result.Value) return;
            
            // обработка полученных путей
            var adjacencyMatrix = DataLoaderService.LoadAdjacencyMatrix(InitDialogViewModel.AdjacencyMatrixFilePath);
            _network.AdjacencyMatrix = adjacencyMatrix;


            double[,] direcTravelersMatrix;
            if (InitDialogViewModel.HasDirectTravelersMatrix)
            {
                direcTravelersMatrix = DataLoaderService.LoadDirectTravelersMatrix(InitDialogViewModel.DirectTravelersMatrixFilePath);
            }
            else
            {
                var arrivals = DataLoaderService.LoadArrivals(InitDialogViewModel.ArrivalsFilePath);
                var departures = DataLoaderService.LoadDepartures(InitDialogViewModel.DeparturesFilePath);
                direcTravelersMatrix = Algorithms.FindCorrespMatrix(adjacencyMatrix, arrivals, departures);
            }

            _network.DirectTravelersMatrix = direcTravelersMatrix;

            DataLoaderService.LoadNodesAtrributes(InitDialogViewModel.NodesPositionsFilePath);
        }

        private InitDialogViewModel _initDialogViewModel;

        public InitDialogViewModel InitDialogViewModel => _initDialogViewModel ??
                                                          (_initDialogViewModel = ServiceLocator.Current.GetInstance<InitDialogViewModel>());
    }
}