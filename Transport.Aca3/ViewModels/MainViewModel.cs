using System;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Transport.Aca3.Models;

namespace Transport.Aca3.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private readonly AcaAlgorithm _acaAlgorithm;
        private readonly Lazy<RelayCommand> _startAlgorithmCommand;

        public MainViewModel(AcaAlgorithm acaAlgorithm)
        {
            _acaAlgorithm = acaAlgorithm;
            _startAlgorithmCommand = new Lazy<RelayCommand>(() => new RelayCommand(InvokeStartAlgorithm, _acaAlgorithm.CanStart));
        }

        public ICommand StartAlgorithmCommand => _startAlgorithmCommand.Value;

        private void InvokeStartAlgorithm()
        {
            _acaAlgorithm.Start();
        }
    }
}