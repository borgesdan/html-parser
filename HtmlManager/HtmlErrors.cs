using System.Xml.Linq;

namespace HtmlManager
{
    internal class HtmlErrors
    {
        public static string TokenNull(string? section)
            => $"{section}: The generated token is null or its value is null when parsing the tag";

        public static string UnableParseTag(string section)
            => $"{section} tag analysis unavailable!";

        public static string AssertionFailed()
            => "Assertion failed, expected to be on '<'";

        public static string CloseTagForVoidElement(string tagName, Interval? interval)
            => $"Close tag for void element! tag name: '{tagName}', interval '{interval}', cursor: '{interval?.Start}'";

        public static string UnexpectedCloseTag(string tagName, Interval? interval)
            => $"Unexpected close tag! tag name: '{tagName}', interval '{interval}', cursor: '{interval?.Start}'";

        public static string MismatchedCloseTag(string openTag, string closeTag, Interval? interval)
           => $"Mismatched close tag! open: '{openTag}', close '{closeTag}', interval: '{interval}' cursor: '{interval?.Start}'";

        public static string InvalidTagname(string tagName, Interval? interval)
            => $"Invalid tag name! tag name: '{tagName}', interval '{interval}', cursor: '{interval?.Start}'";

        public static string UnterminatedComment(string tagName, Interval? interval)
            => $"Invalid tag name! tag name: '{tagName}', interval '{interval}', cursor: '{interval?.Start}'";

        public static string UnclosedTag(Node node)
            => $"Unclosed tag! tag name: '{node.NodeName.ToLower()}', interval '{node.ParseInfo?.OpenTag}', cursor: '{node.ParseInfo?.OpenTag?.Start}'";

        public static string AttributeInClosingTag(Node node)
            => $"Close tag contains attribute! tag name: '{node.NodeName.ToLower()}', interval '{node.ParseInfo?.OpenTag}', cursor: '{node.ParseInfo?.OpenTag?.Start}'";

        public static string UnterminatedCloseTag(Node node)
            => $"Unterminated close tag! tag name: '{node.NodeName.ToLower()}', interval '{node.ParseInfo?.OpenTag}', cursor: '{node.ParseInfo?.OpenTag?.Start}'";

        public static string SelfClosingNonVoidElement(string tagName, int? start, int? end)
            => $"Self closing in non void element! tag name: '{tagName}'," +
            $" start: '{start}'," +
            $" end: '{end}'" +
            $" cursor: '{start}'";

        public static string UnboundAttributeValue(Token? token)
            => $"Unbound attribute value! token value: '{token?.Value}', interval: '{token?.Interval}', cursor: '{token?.Interval.Start}'";

        public static string UnterminatedOpenTag(Node? node, IHtmlStream stream)
            => $"Unterminated open tag! tag name: '{node?.NodeName}', start: '{node?.ParseInfo?.OpenTag?.Start}', " +
            $"end: '{stream.Position}', cursor: '{node?.ParseInfo?.OpenTag?.Start}'";

        public static string InvalidAttributeName(Token? token)
            => $"Invalid attribute name! name: '{token?.Value}', interval: '{token?.Interval}'";

        public static string MultipleAttributeNamespaces(Token? token)
            => $"Multiple attributes namespaces! name: '{token?.Value}', interval: '{token?.Interval}'";

        public static string UnsupportedAttributeNamespace(Token? token, string? attributeName)
            => $"Unsupported attribute namespace! attribute: '{attributeName}', namespace: '{token?.Value}', interval: '{token?.Interval}' ";

        public static string UnquotedAttributevalue(HtmlParser parser)
        {
            var pos = parser.Stream.Position;

            if (!parser.Stream.End())
                pos = parser.Stream.MakeToken()?.Interval.Start ?? pos;

            return $"Unquoted attribute value! start: '{pos}', cursor: '{pos}'";
        }

        public static string UnterminatedAttributeValue(HtmlParser parser, Token? nameTok)
        {
            var currentNode = parser.DomBuilder.CurrentNode;
            var valueTok = parser.Stream.MakeToken();

            return $"Unterminated attribute value! attribute: [{nameTok?.Value}, {nameTok?.Interval}, open tag: '{currentNode.NodeName}, start '{currentNode.ParseInfo?.OpenTag?.Start}'']";
        }
    }

    internal class HtmlWarnings
    {
        public static string UnterminatedComment(Interval? interval)
           => $"Unterminated comment! start: '{interval?.Start}'";

        public static string InvalidAttributeName(Token? token)
            => $"Invalid attribute name! name: '{token?.Value}', interval: '{token?.Interval}'";

        public static string UnsupportedAttributeNamespace(Token? token, string? attributeName)
            => $"Unsupported attribute namespace! attribute: '{attributeName}', namespace: '{token?.Value}', interval: '{token?.Interval}' ";
    }
}

