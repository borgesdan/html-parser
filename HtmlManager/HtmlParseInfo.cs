using HtmlManager.CSS;

namespace HtmlManager
{
    public class HtmlParseInfo
    {
        public string? Name;
        public Interval? Value;
        public Interval? Doctype;
        public Interval? CloseTag;
        public Interval? OpenTag;
        public List<CssRule> Rules = new();
        public List<Interval> Comments = new();

        public HtmlParseInfo() { }
    }
}
