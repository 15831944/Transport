using System.ComponentModel;
using GalaSoft.MvvmLight;
using Transport.Aca3.Models;

namespace Transport.Aca3.ViewModels
{
    public class AcaConfigViewModel : ViewModelBase, IDataErrorInfo
    {
        private readonly AcaConfiguration _acaConfiguration;

        public AcaConfigViewModel(AcaConfiguration acaConfiguration)
        {
            _acaConfiguration = acaConfiguration;
        }

        /// <summary>
        /// Количетсво муравьев в субколонии
        /// </summary>
        public int AntsInSubcolonyCount
        {
            get { return _acaConfiguration.AntsInSubcolonyCount; }
            set
            {
                if (_acaConfiguration.AntsInSubcolonyCount == value) return;

                _acaConfiguration.AntsInSubcolonyCount = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Количество субколоний
        /// </summary>
        public int SubcolonysCount
        {
            get { return _acaConfiguration.SubcolonysCount; }
            set
            {
                if (_acaConfiguration.SubcolonysCount == value) return;

                _acaConfiguration.SubcolonysCount = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Коэффициент значимости феромона у узла
        /// </summary>
        public double Alpha
        {
            get { return _acaConfiguration.Alpha; }
            set
            {
                _acaConfiguration.Alpha = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Коэффициент значимости достижимости до узла
        /// </summary>
        public double Beta
        {
            get { return _acaConfiguration.Beta; }
            set
            {
                _acaConfiguration.Beta = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Константа для расчета феромона
        /// </summary>
        public double Q
        {
            get { return _acaConfiguration.Q; }
            set
            {
                _acaConfiguration.Q = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Коэфициент испарения феромона (от 0 до 1)
        /// </summary>
        public double EvoporationSpeed
        {
            get { return _acaConfiguration.EvoporationSpeed; }
            set
            {
                _acaConfiguration.EvoporationSpeed = value;
                RaisePropertyChanged();
            }
        }

        public int Origin
        {
            get { return _acaConfiguration.Origin; }
            set
            {
                if (_acaConfiguration.Origin == value) return;
                _acaConfiguration.Origin = value;
                RaisePropertyChanged();
            }
        }

        public int Destination
        {
            get { return _acaConfiguration.Destination; }
            set
            {
                if (_acaConfiguration.Destination == value) return;
                _acaConfiguration.Destination = value;
                RaisePropertyChanged();
            }
        }

        public string this[string propertyName] => (_acaConfiguration as IDataErrorInfo)[propertyName];

        public string Error => (_acaConfiguration as IDataErrorInfo).Error;
    }
}
