namespace Transport.Aca.Algorithm
{
    /// <summary>
    /// Класс конфигурации алгоритма
    /// </summary>
    public class AcaConfiguration
    {
        /// <summary>
        /// Количетсво муравьев в субколонии
        /// </summary>
        public int AntsInSubcolonyCount { get; set; }
        /// <summary>
        /// Количество субколоний
        /// </summary>
        public int SubcolonysCount { get; set; }

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
        public double EvoporationSpeed { get; set; }
    }
}
