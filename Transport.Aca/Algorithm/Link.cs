namespace Transport.Aca.Algorithm
{
    public class Link
    {
        public Link(int start, int finish)
        {
            Start = start;
            Finish = finish;
        }

        public int Start { get; }
        public int Finish { get; }
    }
}
