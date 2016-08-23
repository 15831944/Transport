using System;
using System.ComponentModel;
using System.Linq;

namespace Transport.Aca3.Models
{
    /// <summary>
    /// Класс конфигурации алгоритма
    /// </summary>
    public class AcaConfiguration : IDataErrorInfo
    {
        /// <summary>
        /// Количетсво муравьев в субколонии
        /// </summary>
        public int AntsInSubcolonyCount { get; set; } = 30;

        /// <summary>
        /// Количество субколоний
        /// </summary>
        public int SubcolonysCount { get; set; } = 1;

        /// <summary>
        /// Коэффициент значимости феромона у узла
        /// </summary>
        public double Alpha { get; set; } = 1.0;

        /// <summary>
        /// Коэффициент значимости достижимости до узла
        /// </summary>
        public double Beta { get; set; } = 1.0;

        /// <summary>
        /// Константа для расчета феромона
        /// </summary>
        public double Q { get; set; } = 1.0;

        /// <summary>
        /// Коэфициент испарения феромона (от 0 до 1)
        /// </summary>
        public double EvoporationSpeed { get; set; } = 0.1;

        public int Origin { get; set; } = 0;

        public int Destination { get; set; } = 15;

        #region IDataErrorInfo members

        string IDataErrorInfo.this[string propertyName] => GetValidationError(propertyName);

        string IDataErrorInfo.Error => null;

        #endregion

        #region Validation

        public bool IsValid => ValidatedProperties.All(property => GetValidationError(property) == null);

        static readonly string[] ValidatedProperties =
        {
            "AntsInSubcolonyCount",
            "SubcolonysCount",
            "Alpha",
            "Beta",
            "Q",
            "EvoporationSpeed"
        };

        private string GetValidationError(string propertyName)
        {
            if (Array.IndexOf(ValidatedProperties, propertyName) < 0)
                return null;

            string error = null;

            switch (propertyName)
            {
                case "AntsInSubcolonyCount": error = ValidateAntsSubcolonyCount(); break;
                case "SubcolonysCount": error = ValidateSubcolonysCount(); break;
                case "Alpha": error = ValidateAlpha(); break;
                case "Beta": error = ValidateBeta(); break;
                case "Q": error = ValidateQ(); break;
                case "EvoporationSpeed": error = ValidateEvoporationSpeed(); break;
            }

            return error;
        }

        private string ValidateAntsSubcolonyCount()
        {
            return AntsInSubcolonyCount < 0 ? "Некорректное значение количества муравьев в колонии" : null;
        }

        private string ValidateSubcolonysCount()
        {
            return SubcolonysCount < 0 ? "Некорректное значение количества колоний" : null;
        }

        private string ValidateAlpha()
        {
            return Alpha < 0 ? "Некорректное значение коэффициента" : null;
        }

        private string ValidateBeta()
        {
            return Beta < 0 ? "Некорректное значение коэффициента" : null;
        }

        private string ValidateQ()
        {
            return Q < 0 ? "Некорректное значение коэффициента" : null;
        }
        private string ValidateEvoporationSpeed()
        {
            return EvoporationSpeed < 0 || EvoporationSpeed > 1 ? "Некорректное значение коэффициента испарения" : null;
        }

        #endregion

    }
}
