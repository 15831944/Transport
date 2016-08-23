namespace Transport.Aca3.Models
{
    public class AcaConstrains
    {
        // минимальная длина маршрута
        public double MinPathLenght { get; set; } = 0;
        // максимальная длина маршрута
        public double MaxPathLenght { get; set; } = 100;
        // минимальное количество пассажиров
        public int MinPeopleQuantity { get; set; } = 0;

    }
}
