using System;
using System.ComponentModel;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;

namespace Transport.Aca.Dialogs.InitDialog
{
    public class InitDialogViewModel : ViewModelBase, IDialogVewModel, IDataErrorInfo
    {
        private readonly IFileDialogService _fileDialogService;
        private readonly Lazy<RelayCommand> _okCommand;
        private readonly Lazy<RelayCommand> _cancelCommand;
        private readonly Lazy<RelayCommand<string>> _openFileCommand;

        private readonly InitialDataFilePaths _paths;

        public InitDialogViewModel(IFileDialogService fileDialogService)
        {
            _fileDialogService = fileDialogService;
            _paths = new InitialDataFilePaths();

            _okCommand = new Lazy<RelayCommand>(() => new RelayCommand(() =>
                InvokeRequestCloseDialog(new RequestCloseDialogEventArgs(true)),
                CanContinue));
            _cancelCommand = new Lazy<RelayCommand>(() => new RelayCommand(() => InvokeRequestCloseDialog(new RequestCloseDialogEventArgs(false))));
            _openFileCommand = new Lazy<RelayCommand<string>>(() => new RelayCommand<string>(InvokeOpenFileDialog));
        }

        public ICommand OkCommand => _okCommand.Value;

        public ICommand CancelCommand => _cancelCommand.Value;

        public ICommand OpenFileCommand => _openFileCommand.Value;

        public event EventHandler<RequestCloseDialogEventArgs> RequestCloseDialog;

        private void InvokeRequestCloseDialog(RequestCloseDialogEventArgs e)
        {
            var handler = RequestCloseDialog;
            handler?.Invoke(this, e);
        }

        private bool CanContinue()
        {
            return _paths.IsValid;
        }

        private void InvokeOpenFileDialog(string name)
        {
            var path = _fileDialogService.OpenFileDialog();
            if (path == null) return;

            switch (name)
            {
                case "AdjacencyMatrix":
                    AdjacencyMatrixFilePath = path;
                    break;
                case "Departures":
                    DeparturesFilePath = path;
                    break;
                case "Arrivals":
                    ArrivalsFilePath = path;
                    break;
                case "DirectTravelersMatrix":
                    DirectTravelersMatrixFilePath = path;
                    break;
                case "NodesPositions":
                    NodesPositionsFilePath = path;
                    break;
                default:
                    return;
            }
        }

        public string DeparturesFilePath
        {
            get { return _paths.DeparturesFilePath; }
            set
            {
                if (value == _paths.DeparturesFilePath) return;

                _paths.DeparturesFilePath = value;
                RaisePropertyChanged();
            }
        }

        public string ArrivalsFilePath
        {
            get { return _paths.ArrivalsFilePath; }
            set
            {
                if (value == _paths.ArrivalsFilePath) return;

                _paths.ArrivalsFilePath = value;
                RaisePropertyChanged();
            }
        }

        public string AdjacencyMatrixFilePath
        {
            get { return _paths.AdjacencyMatrixFilePath; }
            set
            {
                if (value == _paths.AdjacencyMatrixFilePath) return;

                _paths.AdjacencyMatrixFilePath = value;
                RaisePropertyChanged();
            }
        }

        public string DirectTravelersMatrixFilePath
        {
            get { return _paths.DirectTravelersMatrixFilePath; }
            set
            {
                if (value == _paths.DirectTravelersMatrixFilePath) return;

                _paths.DirectTravelersMatrixFilePath = value;
                RaisePropertyChanged();
            }
        }

        public string NodesPositionsFilePath
        {
            get { return _paths.NodesPositionsFilePath; }
            set
            {
                if (value == _paths.NodesPositionsFilePath) return;

                _paths.NodesPositionsFilePath = value;
                RaisePropertyChanged();
            }
        }

        public bool HasDirectTravelersMatrix
        {
            get { return _paths.HasDirectTravelersMatrix; }
            set
            {
                if (value == _paths.HasDirectTravelersMatrix) return;

                _paths.HasDirectTravelersMatrix = value;
                RaisePropertyChanged();

                // нужно обновлять возможные ошибки этих свойств
                RaisePropertyChanged(nameof(ArrivalsFilePath));
                RaisePropertyChanged(nameof(DeparturesFilePath));
                RaisePropertyChanged(nameof(DirectTravelersMatrixFilePath));
            }
        }

        public string this[string propertyName] => (_paths as IDataErrorInfo)[propertyName];

        public string Error => (_paths as IDataErrorInfo).Error;
    }
}
