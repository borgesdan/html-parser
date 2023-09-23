using System.Text;
using System.Text.RegularExpressions;

namespace HtmlManager.CSS
{
    public class CssParser
    {
        IHtmlStream stream;
        List<string> warnings = new List<string>();
        List<CssRule> rules = new List<CssRule>();
        List<Interval> comments = new List<Interval>();
        CssRule currentRule;
        bool nested = false;
        Token? currentProperty;

        public CssParser(HtmlStream stream)
        {
            this.stream = stream;
        }        

        public CssBlock Parse()
        {
            rules = new List<CssRule>();
            comments = new List<Interval>();

            var sliceStart = stream.Position;
            stream.MarkTokenStartAfterSpace();
            ParseBlockType();
            var sliceEnd = stream.Position;

            var cssBlock = new CssBlock
            {
                Value = stream.Text[sliceStart..sliceEnd],
                ParseInfo = new HtmlParseInfo
                {
                    Value = new Interval(sliceStart, sliceEnd),
                    Rules = rules,
                    Comments = comments
                }
            };

            rules = new List<CssRule>();
            comments = new List<Interval>();

            return cssBlock;
        }    

        private string StripComments(string term, int startPos)
        {
            var last = term.Length;
            var stripped = new StringBuilder();

            for(int pos = 0; pos < last; pos++)
            {
                if (term[pos] == '/' && pos < last - 1 && term[pos + 1] == '*')
                {
                    int commentStart = startPos + pos;
                    pos += 3;

                    while (pos < last - 1 && term.Substring(pos - 1, 2) != "*/")
                        pos++;

                    if (pos >= last - 1 && term.Substring(pos - 1, 2) != "*/")
                        throw new Exception("UNTERMINATED_CSS_COMMENT");

                    int commentEnd = startPos + pos + 1;
                    comments.Add(new Interval(commentStart, commentEnd));
                }
                else
                {
                    stripped.Append(term[pos]);
                }
            }

            return stripped.ToString().Trim();
        }

        private void FilterComments(Token token)
        {
            var text = token.Value;        
            var stripped = StripComments(text, token.Interval.Start);

            text = Regex.Replace(text, @"/^\s +/", "");
            text = Regex.Replace(text, @"/^\/\*[\w\W]*?\*\/\s*/", "");
            int ntSize = text.Length;
            int tSize;
            token.Interval.Start += ntSize;
            text = string.Join("", text.Split("").Reverse());
            text = Regex.Replace(text, @"/^\s+/", "");
            text = Regex.Replace(text, @"/^\/\*[\w\W]*?\*\/\s*/", "");

            ntSize = text.Length;
            token.Interval.End -= tSize = ntSize;
            token.Value = stripped;
        }
        
        private void ParseBlockType() 
        {
            if(currentRule != null)
            {
                rules.Add(currentRule);
                currentRule = null;
            }

            stream.StripCommentBlock();

            if (stream.Peek() == '{')
                throw new Exception("MISSING_CSS_SELECTOR");

            if(stream.Peek() == '@')
            {
                stream.EatCssWhile(new Regex(@"/[^\{]/"));
                var token = stream.MakeToken();
                var name = token.Value.Trim();

                if(Regex.Match(name, @"/@(-[^-]+-)?keyframes/").Success)
                {
                    stream.Next();
                    nested = true;

                    //return this._parseSelector();
                }
            }
        }

        private void ParseSelector()
        {
            stream.EatCssWhile(new Regex(@"/[^\{;\}<]/"));
            var token = stream.MakeToken();
            var peek = stream.Peek();

            if(peek == '}')
            {
                stream.Next();
                ParseBlockType();
            }

            if(token == null)
            {
                if(!stream.End() && stream.Peek() == '<')
                {
                    if (stream.SubStream(2) != "</")
                        throw new Exception("HTML_CODE_IN_CSS_BLOCK");
                }

                return;
            }

            FilterComments(token);
            var selector = token.Value;
            var selectorStart = token.Interval.Start;
            var selectorEnd = token.Interval.End;

            if(selector == "")
            {
                ParseBlockType();
                return;
            }

            currentRule = new CssRule
            {

                Selector = new CssRuleSelector
                {
                    Value = selector,
                    Interval = new Interval(selectorStart, selectorEnd),
                },
                Declarations = new CssRuleDeclaration
                {
                    Interval = null,
                    Properties = new List<Token>(),
                },
            };

            if(stream.End() || peek == '<')
            {
                if (!nested)
                    throw new Exception("UNFINISHED_CSS_SELECTOR");

                return;
            }

            if (!stream.End())
            {
                var next = stream.Next();
                var errorMsg = "[_parseBlockType] Expected {, }, ; or :, instead found " + next;

                if(next == '{')
                {
                    currentRule.Declarations.Interval.Start = stream.Position - 1;
                    ParseDeclaration(selector, selectorStart, null);
                }
                else if(next == ';' || next == '}')
                {
                    throw new Exception("MISSING_CSS_BLOCK_OPENER");
                }
                else
                {
                    throw new Exception("UNCAUGHT_CSS_PARSE_ERROR");
                }
            }
            else
            {
                throw new Exception("MISSING_CSS_BLOCK_OPENER");
            }
        }

        private void ParseDeclaration(string selector, int selectorStart, string? value)
        {
            stream.MarkTokenStartAfterSpace();

            var peek = stream.Peek();

            if(peek == '}')
            {
                stream.Next();
                currentRule.Declarations.Interval.End = stream.Position;
                stream.MarkTokenStartAfterSpace();
                ParseBlockType();
            }
            else if(!string.IsNullOrWhiteSpace(value) && (stream.End() || peek == '<'))
            {
                throw new Exception("MISSING_CSS_BLOCK_CLOSER");
            }
            else
            {
                ParseProperty(selector, selectorStart);
            }
        }

        private void ParseProperty(string selector, int selectorStart)
        {
            stream.EatCssWhile(new Regex(@"/[^\{\}<;:]/"));
            var token = stream.MakeToken();

            if (token == null)
                throw new Exception("MISSING_CSS_PROPERTY");

            FilterComments(token);

            var property = token.Value;
            var propertyStart = token.Interval.Start;
            var propertyEnd = token.Interval.End;

            if(property == "")
            {
                ParseDeclaration(selector, selectorStart, null);
                return;
            }

            var next = stream.Next();
            var errorMsg = "[_parseProperty] Expected }, {, <, ; or :, instead found " + next;

            if (next == '{')
                throw new Exception("MISSING_CSS_BLOCK_CLOSER");

            if((stream.End() && next != ':') || next == '<')
                throw new Exception("UNFINISHED_CSS_PROPERTY");

            currentProperty = new Token
            {
                Value = property,
                Interval = new Interval(propertyStart, propertyEnd),
            };

            if(next == ':')
            {
                if (!(!string.IsNullOrWhiteSpace(property) && Regex.Match(property, @"/^[a-z\-]+$/").Success) || !Css.KnowCssProperty(property))
                    throw new Exception("INVALID_CSS_PROPERTY_NAME");

                stream.MarkTokenStartAfterSpace();
                ParseValue(selector, property, propertyStart);
            }
            else if(next == ';')
                throw new Exception("MISSING_CSS_VALUE");
            else
                throw new Exception("UNCAUGHT_CSS_PARSE_ERROR");
        }

        private void ParseValue(string selector, string property, int propertyStart)
        {
            if (property == "content")
                stream.FindContentEnd();
            else
                stream.EatCssWhile(new Regex(@"/[^}<;]/"));

            var token = stream.MakeToken() ?? throw new Exception("MISSING_CSS_VALUE");

            char? next = (!stream.End() ? stream.Next() : null);
            //var errorMsg = "[_parseValue] Expected }, <, or ;, instead found " + (next != '\0' ? next : "end of stream";

            FilterComments(token);

            var value = token.Value;
            var valueStart = token.Interval.Start;
            var valueEnd = token.Interval.End;

            if (value == "")
                throw new Exception("MISSING_CSS_VALUE");

            currentProperty = new Token
            {
                Value = value,
                Interval = new Interval(valueStart, valueEnd)
            };

            if((stream.End() && next != ';') || next == '<')
                throw new Exception("UNFINISHED_CSS_VALUE");

            //if (Regex.Match(value, @"/,?\s*url\(\s*['""]?http:\/\/.+\)/").Success)
            //{
            //    valueStart = valueStart + value.IndexOf("url");

            //    warnings.Add("CSS_MIXED_ACTIVECONTENT");
            //}

            if(next == ';')
            {
                BindCurrentRule();
                stream.MarkTokenStartAfterSpace();
                ParseDeclaration(selector, valueStart, value);
            }
            else if(next == '}')
            {
                currentRule.Declarations.Interval.End = stream.Position;
                BindCurrentRule();
                stream.MarkTokenStartAfterSpace();
                ParseBlockType();
            }
            else
            {
                throw new Exception("UNCAUGHT_CSS_PARSE_ERROR");
            }
        }

        private void BindCurrentRule()
        {
            currentRule.Declarations.Properties.Add(currentProperty);
            currentProperty = null;
        }
    }
}
