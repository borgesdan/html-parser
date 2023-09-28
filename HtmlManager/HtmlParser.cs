using HtmlManager.CSS;
using System.ComponentModel;
using System.Text.RegularExpressions;

namespace HtmlManager
{
    public class HtmlParser
    {
        readonly IHtmlStream stream;
        readonly DomBuilder domBuilder;
        readonly List<string> warnings = new();        
        readonly List<string> errors = new();        
        readonly List<string> internalErrors = new();        
        readonly CssParser cssParser;
        Node? activeTagNode = null;
        Node? parentTagNode = null;
        bool parsingSVG = false;

        public IHtmlStream Stream => stream;
        public DomBuilder DomBuilder => domBuilder;

        public HtmlParser(HtmlStream stream, DomBuilder domBuilder)
        {
            this.stream = stream;
            this.domBuilder = domBuilder;
            this.cssParser = new CssParser(stream);
        }

        private static char? ContainsAttribute(IHtmlStream stream)
        {
            return stream.Eat(HtmlRegex.NameStartCharacterSet());
        }

        private static bool IsNextTagParent(IHtmlStream stream, string? parentTagName)
        {
            return stream.FindNext(@"<\/([\w\-]+)\s*>", 1) == parentTagName;
        }

        private static bool IsNextCloseTag(IHtmlStream stream)
        {
            return stream.FindNext(@"<\/([\w\-]+)\s*>", 1) != null;
        }

        public static string ReplaceEntityRefs(string text)
        {
            return HtmlRegex.AmpersandAzGroupSemicolon().Replace(text, m =>
            {                
                var name = m.Value.ToLower();

                if (Html.CharacterEntityRefs.ContainsKey(name))
                    return Html.CharacterEntityRefs[name];
                
               // return m.Value;
                return text;
            });
        }          
        

        public HtmlParserResult Parse()
        {
            if (stream.Match(Html.Html5Doctype, true, true))
            {
                domBuilder.Fragment.Node.ParseInfo = new HtmlParseInfo
                {
                    Doctype = new Interval(0, stream.Position)
                };
            }

            while (!stream.End())
            {
                if (stream.Peek() == '<')
                {
                    BuildTextNode();
                    ParseStartTag();
                }
                else
                    stream.Next();
            }
            
            BuildTextNode();

            if (domBuilder.CurrentNode != domBuilder.Fragment.Node)
            {
                errors.Add("Unclosed tag");
            }

            var result = new HtmlParserResult(domBuilder, warnings, errors, internalErrors);
            return result;
        }

        private void BuildTextNode()
        {
            var token = stream.MakeToken();

            if(token is null)
                return;

            var tokenValue = ReplaceEntityRefs(token.Value);
            var parseInfo = new HtmlParseInfo { Value = token.Interval };

            domBuilder.Text(tokenValue, parseInfo);
        }

        private void ParseStartTag()
        {
            if (stream.Next() != '<')
            {
                errors.Add(HtmlErrors.AssertionFailed());
                return;
            }                

            if (stream.Match("!--", true, false))
            {
                domBuilder.PushContext("text", stream.Position);
                
                ParseComment();

                domBuilder.PushContext("html", stream.Position);
                return;
            }

            stream.Eat(HtmlRegex.EscapedCharacter());
            stream.EatWhile(HtmlRegex.WordDigitAndDash());

            var token = stream.MakeToken();

            if(token == null ||  token.Value is null)
            {
                internalErrors.Add(HtmlErrors.TokenNull("Parse tag"));
                return;
            }

            var tagName = token.Value[1..].ToLower();

            if (tagName == "svg")
                parsingSVG = true;

            if (!string.IsNullOrEmpty(tagName) && tagName[0] == '/')
            {
                activeTagNode = null;
                var closeTagname = tagName[1..].ToLower();

                if (closeTagname == "svg")
                    parsingSVG = false;

                if (Html.KnownVoidHTMLElement(closeTagname))
                {
                    errors.Add(HtmlErrors.CloseTagForVoidElement(closeTagname, token.Interval));
                }

                if (domBuilder.CurrentNode.ParseInfo == null)
                {
                    errors.Add(HtmlErrors.UnexpectedCloseTag(closeTagname, token.Interval));
                    return;
                }

                domBuilder.CurrentNode.ParseInfo = new HtmlParseInfo
                {
                    CloseTag = new Interval(token.Interval.Start, 0)
                };

                var openTagname = domBuilder.CurrentNode.NodeName.ToLower();

                if (closeTagname != openTagname)
                {
                    errors.Add(HtmlErrors.MismatchedCloseTag(openTagname, closeTagname, token.Interval));
                    return;
                }

                ParseEndCloseTag();
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(tagName))
                {
                    var badSVG = parsingSVG && !Html.KnownSVGElement(tagName);
                    var badHTML = !parsingSVG && !Html.KnownHTMLElement(tagName) && !Html.IsCustomElement(tagName);

                    if (badSVG || badHTML)
                    {
                        errors.Add("Invalid tag name.");
                        return;
                    }
                }
                else
                {
                    errors.Add("Invalid tag name.");
                    return;
                }

                var parseInfo = new HtmlParseInfo
                {
                    OpenTag = new Interval(token.Interval.Start, 0),
                };

                var nameSpace = parsingSVG ? Html.SvgNameSpace : null;

                if(activeTagNode != null && parentTagNode != domBuilder.Fragment.Node)
                {
                    var activeTagName = activeTagNode.NodeName.ToLower();

                    if (Html.KnownOmittableCloseTags(activeTagName, tagName))
                        domBuilder.PopElement();
                }

                parentTagNode = domBuilder.CurrentNode;
                domBuilder.PushElement(tagName, parseInfo, nameSpace);

                if (!stream.End())
                    ParseEndOpenTag(tagName);
            }
        }        

        private void ParseComment()
        {
            Token? token;            

            while (!stream.End())
            {
                if(stream.Match("-->", true, false))
                {
                    token = stream.MakeToken();

                    if(token is null)
                    {
                        internalErrors.Add(HtmlErrors.TokenNull("Parse Comment"));
                        return;
                    }

                    domBuilder.Comment(token.Value[4..(token.Value.Length - 3)], new HtmlParseInfo { Value = token.Interval });
                    return;
                }

                stream.Next();
            }

            token = stream.MakeToken();
            warnings.Add(HtmlWarnings.UnterminatedComment(token?.Interval));
        }        

        private void ParseCDATA(string tagName)
        {
            Token? token;
            string matchString = string.Concat("</", tagName, ">");
            string text;
            Interval textInterval = new();

            if(domBuilder.CurrentNode.ParseInfo is null || domBuilder.CurrentNode.ParseInfo.OpenTag is null)
            {
                errors.Add(HtmlErrors.UnableParseTag("CDATA"));
                return;
            }

            var openTagEnd = domBuilder.CurrentNode.ParseInfo.OpenTag.End;
            Interval closeTagInterval;

            stream.MakeToken();

            while (!stream.End())
            {
                if(stream.Match(matchString, true, false))
                {
                    token = stream.MakeToken();

                    if(token is null)
                    {
                        internalErrors.Add(HtmlErrors.TokenNull("Parse CDATA"));
                        return;
                    }

                    text = token.Value[0..(token.Value.Length - matchString.Length)];

                    closeTagInterval = new Interval(openTagEnd + text.Length, token.Interval.End);

                    domBuilder.CurrentNode.ParseInfo.CloseTag = closeTagInterval;
                    textInterval.Start = token.Interval.Start;
                    textInterval.End = token.Interval.End - (closeTagInterval.End - closeTagInterval.Start);
                    domBuilder.Text(text, new HtmlParseInfo { Value = textInterval });
                    domBuilder.PopElement();

                    return;
                }

                stream.Next();
            }
            
            errors.Add(HtmlErrors.UnclosedTag(domBuilder.CurrentNode));
        }        

        private void ParseEndCloseTag()
        {
            stream.EatSpace();

            if(stream.Next() != '>')
            {
                if (ContainsAttribute(stream) != null)
                {
                    errors.Add(HtmlErrors.AttributeInClosingTag(domBuilder.CurrentNode));
                    return;
                }
                else
                {
                    errors.Add(HtmlErrors.UnterminatedCloseTag(domBuilder.CurrentNode));
                    return;
                }
            }

            var token = stream.MakeToken();

            if(token is null)
            {
                internalErrors.Add(HtmlErrors.TokenNull("Parse end close tag"));
                return;
            }

            var end = token.Interval.End;

            if(domBuilder.CurrentNode.ParseInfo is null || domBuilder.CurrentNode.ParseInfo.CloseTag is null)
            {
                errors.Add(HtmlErrors.UnableParseTag(domBuilder.CurrentNode.NodeName));
                return;
            }

            domBuilder.CurrentNode.ParseInfo.CloseTag.End = end;
            domBuilder.PopElement();
        }

        private void ParseEndOpenTag(string tagName)
        {
            var tagMark = stream.Position;
            var startMark = stream.Position;

            while (!stream.End())
            {
                if (ContainsAttribute(stream) != null)
                {
                    if (stream.Peek() != '=')
                        stream.EatWhile(HtmlRegex.NameChararacterSet());

                    ParseAttribute();
                }
                else if (stream.EatSpace())
                {
                    stream.MakeToken();
                    startMark = stream.Position;
                }
                else if(stream.Peek() == '>' || stream.Match("/>", false, false))
                {
                    var selfClosing = stream.Match("/>", true, false);

                    if (selfClosing)
                    {
                        if (!parsingSVG && !Html.KnownVoidHTMLElement(tagName))
                        {
                            var _start = domBuilder.CurrentNode.ParseInfo?.OpenTag?.Start;
                            var _end = stream.MakeToken()?.Interval.End;

                            errors.Add(HtmlErrors.SelfClosingNonVoidElement(tagName, _start, _end));
                            return;
                        }                            
                    }
                    else
                    {
                        stream.Next();
                    }

                    var token = stream.MakeToken();

                    if(token is null)
                    {
                        internalErrors.Add(HtmlErrors.TokenNull("Parse end open tag"));
                        return;
                    }

                    var end = token.Interval.End;

                    if(domBuilder.CurrentNode.ParseInfo?.OpenTag is null)
                    {
                        errors.Add(HtmlErrors.UnableParseTag(tagName));
                        return;
                    }

                    domBuilder.CurrentNode.ParseInfo.OpenTag.End = end;

                    if (!string.IsNullOrWhiteSpace(tagName) && ((selfClosing && Html.KnownSVGElement(tagName)) || Html.KnownVoidHTMLElement(tagName)))
                        domBuilder.PopElement();

                    activeTagNode = null;

                    if(!string.IsNullOrWhiteSpace(tagName) && Html.KnownOmittableCloseTagHtmlElement(tagName))
                    {
                        activeTagNode = domBuilder.CurrentNode;
                    }

                    if(stream.End() && tagName == "style")
                    {
                        domBuilder.PushContext("css", stream.Position);
                        
                        var cssBlock = cssParser.Parse();
                        
                        domBuilder.PushContext("html", stream.Position);
                        domBuilder.Text(cssBlock.Value, cssBlock.ParseInfo);
                    }

                    if (!string.IsNullOrWhiteSpace(tagName))
                    {
                        if(tagName == "script")
                        {
                            domBuilder.PushContext("javascript", stream.Position);
                            ParseCDATA("script");
                            domBuilder.PushContext("html", stream.Position);
                        }
                        else if(tagName == "textarea")
                        {
                            domBuilder.PushContext("text", stream.Position);
                            ParseCDATA("textarea");
                            domBuilder.PushContext("html", stream.Position);
                        }
                    }                    

                    if(parentTagNode != null && parentTagNode != domBuilder.Fragment.Node)
                    {
                        var _parentTagname = parentTagNode.NodeName.ToLower();
                        var nextIsParent = IsNextTagParent(stream, _parentTagname);
                        var needsEndTag = !Html.AllowOmmitedEndTag(_parentTagname, tagName);
                        var optionalEndTag = Html.KnownOmittableCloseTagHtmlElement(_parentTagname);
                        var nextTagCloses = IsNextCloseTag(stream);

                        if(nextIsParent && (needsEndTag || (optionalEndTag && nextTagCloses)))
                        {
                            if (Html.KnownOmittableCloseTagHtmlElement(tagName))
                                domBuilder.PopElement();
                        }
                    }

                    return;
                }
                else
                {
                    stream.EatWhile(new Regex("[^'\"\\s=<>]"));
                    var attrToken = stream.MakeToken();

                    if(attrToken == null)
                    {
                        stream.TokenStart = tagMark;
                        stream.MakeToken();
                        var peek = stream.Peek();

                        if(peek == '\'' || peek == '"')
                        {
                            stream.Next();
                            stream.EatWhile(new Regex("[^" + peek + "]"));
                            stream.Next();
                            var token = stream.MakeToken();
                            
                            errors.Add(HtmlErrors.UnboundAttributeValue(token));
                            return;
                        }
                        
                        errors.Add(HtmlErrors.UnterminatedOpenTag(domBuilder.CurrentNode, stream));
                        return;
                    }

                    attrToken.Interval.Start = startMark;
                    
                    warnings.Add(HtmlWarnings.InvalidAttributeName(attrToken));
                }
            }
        }

        /// <summary>
        /// This helper function parses an HTML tag attribute. It expects
        /// the stream to be right after the end of an attribute name.
        /// </summary>
        private void ParseAttribute()
        {
            var nameTok = stream.MakeToken();

            if(nameTok is null)
            {
                internalErrors.Add(HtmlErrors.TokenNull("Parse attribute"));
                return;
            }

            nameTok.Value = nameTok.Value.ToLower();
            stream.EatSpace();

            if(stream.Peek() == '=')
            {
                stream.Next();

                if(nameTok.Value.IndexOf(":") != -1)
                {
                    var parts = nameTok.Value.Split(":");

                    if (parts.Length > 2)
                    {
                        errors.Add(HtmlErrors.MultipleAttributeNamespaces(nameTok));
                        return;
                    }

                    var nameSpace = parts[0];
                    var attributeName = parts[1];

                    if(!Html.SupportedAttributeNameSpace(nameSpace))
                    {                        
                        warnings.Add(HtmlWarnings.UnsupportedAttributeNamespace(nameTok, attributeName));
                        //return;
                    }
                }

                stream.EatSpace();
                stream.MakeToken();

                var quoteType = stream.Next();

                if (quoteType != '"' && quoteType != '\'')
                {
                    errors.Add(HtmlErrors.UnquotedAttributevalue(null));
                    return;
                }

                if (quoteType == '"')
                    stream.EatWhile(HtmlRegex.NegateQuot());
                else
                    stream.EatWhile(HtmlRegex.NegateQuotMark());

                if (stream.Next() != quoteType)
                {
                    errors.Add(HtmlErrors.UnterminatedAttributeValue(this, nameTok));
                    return;
                }

                var valueTok = stream.MakeToken();

                if(valueTok is null)
                {
                    internalErrors.Add(HtmlErrors.TokenNull("Parse attribute"));
                    return;
                }

                var _valueTok = valueTok.Value[1..(valueTok.Value.Length - 1)];
                var unquoteValue = ReplaceEntityRefs(_valueTok);

                domBuilder.Attribute(nameTok.Value, unquoteValue, new HtmlParseInfo
                {
                    Name = nameTok.Value,
                    Value = valueTok.Interval,
                });
            }
            else
            {
                stream.MakeToken();
                domBuilder.Attribute(nameTok.Value, "", new HtmlParseInfo
                {
                    Name = nameTok.Value,
                });
            }            
        }        
    }    
}