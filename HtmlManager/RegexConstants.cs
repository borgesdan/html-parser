using System.Text.RegularExpressions;

namespace HtmlManager
{
    public partial class HtmlRegex
    {
        private const string AttributeNameStartChar = "A-Za-z_\\u00C0-\\u00D6\\u00D8-\\u00F6\\u00F8-\\u02FF\\u0370-\\u037D\\u037F-\\u1FFF\\u200C-\\u200D\\u2070-\\u218F\\u2C00-\\u2FEF\\u3001-\\uD7FF\\uF900-\\uFDCF\\uFDF0-\\uFFFD";
        private const string AttributeNameChar = AttributeNameStartChar + "0-9\\-\\.\\u00B7\\u0300-\\u036F\\u203F-\\u2040:";

        [GeneratedRegex(@"[\s\n]", RegexOptions.CultureInvariant | RegexOptions.IgnoreCase)]
        public static partial Regex WhiteSpaceAndEscaped();

        [GeneratedRegex("[^*]", RegexOptions.CultureInvariant | RegexOptions.IgnoreCase)]
        public static partial Regex NegateAsterisk();

        [GeneratedRegex("[^>]", RegexOptions.CultureInvariant | RegexOptions.IgnoreCase)]
        public static partial Regex NegateAngleBracket();

        [GeneratedRegex("[" + AttributeNameStartChar + "]", RegexOptions.CultureInvariant | RegexOptions.IgnoreCase)]
        public static partial Regex NameStartCharacterSet();

        [GeneratedRegex("[" + AttributeNameChar + "]", RegexOptions.CultureInvariant | RegexOptions.IgnoreCase)]
        public static partial Regex NameChararacterSet();

        [GeneratedRegex("&([A-Za-z]+);", RegexOptions.CultureInvariant | RegexOptions.IgnoreCase)]
        public static partial Regex AmpersandAzGroupSemicolon();
        
        [GeneratedRegex(@"\/", RegexOptions.CultureInvariant | RegexOptions.IgnoreCase)]
        public static partial Regex EscapedCharacter();
        
        [GeneratedRegex(@"[\w\d-]", RegexOptions.CultureInvariant | RegexOptions.IgnoreCase)]
        public static partial Regex WordDigitAndDash();
        
        [GeneratedRegex(@"[^""]", RegexOptions.CultureInvariant | RegexOptions.IgnoreCase)]
        public static partial Regex NegateQuot();

        [GeneratedRegex(@"[^']", RegexOptions.CultureInvariant | RegexOptions.IgnoreCase)]
        public static partial Regex NegateQuotMark();

        [GeneratedRegex(@"<\/([\w\-]+)\s*>", RegexOptions.CultureInvariant | RegexOptions.IgnoreCase)]
        public static partial Regex FindNext();
    }
}
