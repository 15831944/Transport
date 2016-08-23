using System;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using MvvmDialogs;
using MvvmDialogs.FrameworkDialogs.OpenFile;
using Transport.Aca3.Models;
using Transport.Aca3.Services;

namespace Transport.Aca3.ViewModels
{
    public class DataSourceViewModel : ViewModelBase
    {
        private readonly IDialogService _dialogService;
        private readonly DataSource _dataSource;

        private enum DataTypes
        {
            AdjacencyMatrix,
            Departures,
            Arrivals,
            DemandMatrix,
            NodesCoords
        }

        public DataSourceViewModel(IDialogService dialogService, DataSource dataSource)
        {
            _dialogService = dialogService;
            _dataSource = dataSource;

            _openAdjacencyMatrixFileCommand = new Lazy<RelayCommand>(() => new RelayCommand(() => OpenFile(DataTypes.AdjacencyMatrix)));
            _clearAdjacencyMatrixCommand = new Lazy<RelayCommand>(() => new RelayCommand(InvokeClearAdjacencyMatrix));

            _openDeparturesFileCommand = new Lazy<RelayCommand>(() => new RelayCommand(() => OpenFile(DataTypes.Departures)));
            _clearDeparturesCommand = new Lazy<RelayCommand>(() => new RelayCommand(InvokeClearDepartures));

            _openArrivalsFileCommand = new Lazy<RelayCommand>(() => new RelayCommand(() => OpenFile(DataTypes.Arrivals)));
            _clearArrivalsCommand = new Lazy<RelayCommand>(() => new RelayCommand(InvokeClearArrivals));

            _openDemandMatrixFileCommand = new Lazy<RelayCommand>(() => new RelayCommand(() => OpenFile(DataTypes.DemandMatrix)));
            _clearDemandMatrixCommand = new Lazy<RelayCommand>(() => new RelayCommand(InvokeClearDemandMatrix));

            _openNodesCoordsFileCommand = new Lazy<RelayCommand>(() => new RelayCommand(() => OpenFile(DataTypes.NodesCoords)));
            _clearNodesCoordsCommand = new Lazy<RelayCommand>(() => new RelayCommand(InvokeClearNodesCoords));
        }

        private bool _isAdjacencyMatrixCorrect;

        public bool IsAdjacencyMatrixCorrect
        {
            get { return _isAdjacencyMatrixCorrect; }
            set { Set(ref _isAdjacencyMatrixCorrect, value); }
        }

        private bool _isDeparturesCorrect;

        public bool IsDeparturesCorrect
        {
            get { return _isDeparturesCorrect; }
            set { Set(ref _isDeparturesCorrect, value); }
        }

        private bool _isArrivalsCorrect;

        public bool IsArrivalsCorrect
        {
            get { return _isArrivalsCorrect; }
            set { Set(ref _isArrivalsCorrect, value); }
        }

        private bool _isDemandMatrixCorrect;

        public bool IsDemandMatrixCorrect
        {
            get { return _isDemandMatrixCorrect; }
            set { Set(ref _isDemandMatrixCorrect, value); }
        }

        private bool _isNodesCoordsCorrect;
        public bool IsNodesCoordsCorrect
        {
            get { return _isNodesCoordsCorrect; }
            set { Set(ref _isNodesCoordsCorrect, value); }
        }

        private void OpenFile(DataTypes dataType)
        {
            var settings = new OpenFileDialogSettings
            {
                Title = "Открыть файл с данными",
                Filter = "Текстовый документ (*.txt)|*.txt|Все файлы (*.*)|*.*"
            };

            var success = _dialogService.ShowOpenFileDialog(this, settings);
            if (success == true)
            {
                switch (dataType)
                {
                    case DataTypes.AdjacencyMatrix:
                        LoadAdjacencyMatrixFile(settings.FileName);
                        break;
                    case DataTypes.DemandMatrix:
                        LoadDemandMatrixFile(settings.FileName);
                        break;
                    case DataTypes.Arrivals:
                        LoadArrivalsFile(settings.FileName);
                        break;
                    case DataTypes.Departures:
                        LoadDeparturesFile(settings.FileName);
                        break;
                    case DataTypes.NodesCoords:
                        LoadNodesCoordsFile(settings.FileName);
                        break;
                }
            }
        }



        #region OpenAdjacencyMatrixFileCommand

        private readonly Lazy<RelayCommand> _openAdjacencyMatrixFileCommand;
        public ICommand OpenAdjacencyMatrixFileCommand => _openAdjacencyMatrixFileCommand.Value;

        private void LoadAdjacencyMatrixFile(string path)
        {
            _dataSource.AdjacencyMatrixFilePath = path;
            _dataSource.AdjacencyMatrix = DataLoaderService.LoadAdjacencyMatrix(path);
            IsAdjacencyMatrixCorrect = true;
        }

        #endregion

        #region ClearAdjacencyMatrixFileCommand

        private readonly Lazy<RelayCommand> _clearAdjacencyMatrixCommand;
        public ICommand ClearAdjacencyMatrixCommand => _clearAdjacencyMatrixCommand.Value;

        private void InvokeClearAdjacencyMatrix()
        {
            _dataSource.AdjacencyMatrixFilePath = String.Empty;
            _dataSource.AdjacencyMatrix = null;
            IsAdjacencyMatrixCorrect = false;
        }

        #endregion

        #region OpenDeparturesFileCommand

        private readonly Lazy<RelayCommand> _openDeparturesFileCommand;
        public ICommand OpenDeparturesFileCommand => _openDeparturesFileCommand.Value;

        private void LoadDeparturesFile(string path)
        {
            _dataSource.DeparturesFilePath = path;
            _dataSource.Departures = DataLoaderService.LoadDepartures(path);
            IsDeparturesCorrect = true;
        }

        #endregion

        #region ClearDeparturesFileCommand

        private readonly Lazy<RelayCommand> _clearDeparturesCommand;
        public ICommand ClearDeparturesCommand => _clearDeparturesCommand.Value;

        private void InvokeClearDepartures()
        {
            _dataSource.DeparturesFilePath = String.Empty;
            _dataSource.Departures = null;
            IsDeparturesCorrect = false;
        }

        #endregion

        #region OpenArrivalsFileCommand

        private readonly Lazy<RelayCommand> _openArrivalsFileCommand;
        public ICommand OpenArrivalsFileCommand => _openArrivalsFileCommand.Value;

        private void LoadArrivalsFile(string path)
        {
            _dataSource.ArrivalsFilePath = path;
            _dataSource.Arrivals = DataLoaderService.LoadArrivals(path);
            IsArrivalsCorrect = true;
        }

        #endregion

        #region ClearArrivalsFileCommand

        private readonly Lazy<RelayCommand> _clearArrivalsCommand;
        public ICommand ClearArrivalsCommand => _clearArrivalsCommand.Value;

        private void InvokeClearArrivals()
        {
            _dataSource.ArrivalsFilePath = String.Empty;
            _dataSource.Arrivals = null;
            IsArrivalsCorrect = false;
        }

        #endregion

        #region OpenDemandMatrixFileCommand

        private readonly Lazy<RelayCommand> _openDemandMatrixFileCommand;
        public ICommand OpenDemandMatrixFileCommand => _openDemandMatrixFileCommand.Value;

        private void LoadDemandMatrixFile(string path)
        {
            _dataSource.DemandMatrixFilePath = path;
            _dataSource.DemandMatrix = DataLoaderService.LoadDirectTravelersMatrix(path);
            IsDemandMatrixCorrect = true;
        }

        #endregion

        #region ClearDemandMatrixCommand

        private readonly Lazy<RelayCommand> _clearDemandMatrixCommand;
        public ICommand ClearDemandMatrixCommand => _clearDemandMatrixCommand.Value;

        private void InvokeClearDemandMatrix()
        {
            _dataSource.DemandMatrixFilePath = String.Empty;
            _dataSource.DemandMatrix = null;
            IsDemandMatrixCorrect = false;
        }

        #endregion

        #region OpenNodesCoordsFileCommand

        private readonly Lazy<RelayCommand> _openNodesCoordsFileCommand;
        public ICommand OpenNodesCoordsFileCommand => _openNodesCoordsFileCommand.Value;

        private void LoadNodesCoordsFile(string path)
        {
            _dataSource.NodesCoordsFilePath = path;
            _dataSource.NodesCoords = DataLoaderService.LoadNodesCoords(path);
            IsNodesCoordsCorrect = true;
        }

        #endregion

        #region ClearNodesCoordsCommand

        private readonly Lazy<RelayCommand> _clearNodesCoordsCommand;
        public ICommand ClearNodesCoordsCommand => _clearNodesCoordsCommand.Value;

        private void InvokeClearNodesCoords()
        {
            _dataSource.NodesCoordsFilePath = String.Empty;
            _dataSource.NodesCoords = null;
            IsNodesCoordsCorrect = false;
        }

        #endregion
    }
}
