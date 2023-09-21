namespace HtmlManager
{
    public class Interval
    {
        public int Start { get; set; }
        public int End { get; set; }

        public Interval()
        {
        }

        public Interval(int start, int end)
        {
            Start = start;
            End = end;
        }
    }
}
