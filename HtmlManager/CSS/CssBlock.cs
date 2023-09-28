namespace HtmlManager.CSS
{
    public class CssBlock
    {
        public string Value { get; set; }
        public HtmlParseInfo ParseInfo { get; set; }

        public CssBlock() 
        {
            Value = string.Empty;
            ParseInfo = new HtmlParseInfo();
        }
    }
}
