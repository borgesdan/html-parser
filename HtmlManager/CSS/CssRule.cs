namespace HtmlManager.CSS
{
    public class CssRule
    {        
        public CssRuleDeclaration Declarations { get; set; } = new CssRuleDeclaration();
        public CssRuleSelector Selector { get; set; } = new CssRuleSelector();
    }

    public class CssRuleDeclaration
    {
        public List<Token> Properties { get; set; } = new List<Token>();
        public Interval? Interval;
    }

    public class CssRuleSelector 
    {
        public Interval? Interval;
        public string? Value { get; set; }
    }
}
