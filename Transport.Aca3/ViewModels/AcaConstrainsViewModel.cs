using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using Transport.Aca3.Models;

namespace Transport.Aca3.ViewModels
{
    public class AcaConstrainsViewModel : ViewModelBase
    {
        private readonly AcaConstrains _acaConstrains;

        public AcaConstrainsViewModel(AcaConstrains acaConstrains)
        {
            _acaConstrains = acaConstrains;
        }

        // минимальная длина маршрута
        public double MinPathLenght
        {
            get { return _acaConstrains.MinPathLenght; }
            set
            {
                _acaConstrains.MinPathLenght = value;
                RaisePropertyChanged();
            }
        }
        // максимальная длина маршрута
        public double MaxPathLenght
        {
            get { return _acaConstrains.MaxPathLenght; }
            set
            {
                _acaConstrains.MaxPathLenght = value;
                RaisePropertyChanged();
            }
        }

        // минимальное количество пассажиров
        public int MinPeopleQuantity
        {
            get { return _acaConstrains.MinPeopleQuantity; }
            set
            {
                if (_acaConstrains.MinPeopleQuantity == value) return;
                _acaConstrains.MinPeopleQuantity = value;
                RaisePropertyChanged();
            }
         
        }
    }
}
