namespace HtmlManager
{
    public class Context
    {
        public string Value { get; set; }
        public int Position { get; set; }

        public Context(string value, int position)
        {
            Value = value;
            Position = position;
        }
    }
}