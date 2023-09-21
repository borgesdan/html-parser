namespace HtmlManager
{
    public class HtmlParserResult
    {
        public IEnumerable<string> Warnings { get; private set; }

        public HtmlParserResult(IEnumerable<string> warnings)
        {
            Warnings = warnings;
        }
    }
}
