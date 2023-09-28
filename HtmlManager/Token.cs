namespace HtmlManager
{
    public class Token
    {
        public string Value;
        public Interval Interval;        

        public Token()
        {
            Value = string.Empty;
            Interval = new Interval();
        }

        public Token(string value) 
        {
            Value = value;
            Interval = new Interval();
        }

        public Token(string value, Interval interval)
        {
            Value = value;
            Interval = interval;
        }
    }    
}
