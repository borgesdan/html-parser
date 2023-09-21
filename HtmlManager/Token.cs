namespace HtmlManager
{
    public class Token
    {
        public string? Value;
        public Interval? Interval;        

        public Token() 
        {
            Interval = new Interval();
        }

        public Token(string? value, Interval? interval)
        {
            Value = value;
            Interval = interval;
        }
    }    
}
