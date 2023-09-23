using System.Text.RegularExpressions;

namespace HtmlManager
{
    /// <summary>
    /// HtmlStream is an internal class used for tokenization. 
    /// </summary>
    public class HtmlStream : IHtmlStream
    {
        readonly string text = string.Empty;
        int pos = 0;

        public int Position => pos;
        public string Text => text;
        public int TokenStart { get; set; }        

        public HtmlStream(string text)
        {
            this.text = text;
        }

        /// <summary>
        /// Returns the next character in the stream without advancing it.
        /// It will return `null` at the end of the text.
        /// </summary>
        public char? Peek()
        {
            if (string.IsNullOrEmpty(text) || End())
                return null;

            return text[pos];
        }

        /// <summary>
        /// Returns a substream from the stream without advancing it, with length `len`.
        /// It will return `null` if out of range.
        /// </summary>
        public string? SubStream(int len)
        {
            if (pos + len > text.Length)
                return null;

            return text.Substring(pos, len);
        }

        /// <summary>
        /// returns true only if the stream is at the end of the text.    
        /// </summary>
        public bool End() => pos == text.Length;

        /// <summary>
        /// Returns the next character in the stream and advances
        /// it. It also returns `null` when no more characters are available.
        /// </summary>
        public char? Next() => !End() ? text[pos++] : null;

        /// <summary>
        /// Rewinds the stream position by X places.
        /// </summary>
        public void Rewind(int x)
        {
            pos -= x;

            if (pos < 0)
                pos = 0;
        }        

        /// <summary>
        /// Takes a regular expression. If the next character in
        /// the stream matches the given argument, it is consumed and returned.
        /// Otherwise, `null` is returned.
        /// </summary>
        public char? Eat(Regex match)
        {
            if (End())
                return null;

            var character = Peek();

            if (character != null)
            {
                var characterString = character.ToString();                                

                if (!string.IsNullOrEmpty(characterString) && match.IsMatch(characterString))
                    return Next();
            }                

            return null;
        }        

        /// <summary>
        /// Repeatedly calls `eat()` with the given argument,
        /// until it fails. Returns `true` if any characters were eaten.
        /// </summary>
        public bool EatWhile(Regex matcher)
        {
            var wereAnyEaten = false;

            while (!End())
            {
                if (Eat(matcher) != null)
                    wereAnyEaten = true;
                else
                    break;
            }

            return wereAnyEaten;
        }

        /// <summary>
        /// Is a shortcut for `eatWhile()` when matching
        /// white-space (including newlines).
        /// </summary>
        public bool EatSpace() => EatWhile(HtmlRegex.WhiteSpaceAndEscaped());       

        /// <summary>
        /// Is like `eatWhile()`, but it
        /// automatically deals with eating block comments like `/* foo */`.
        /// </summary>
        public bool EatCssWhile(Regex matcher)
        {
            var wereAnyEaten = false;
            char? next = '\0';

            while (!End())
            {
                char? chr = Eat(matcher);

                if (chr != null)
                    wereAnyEaten = true;
                else
                    break;

                if(chr == '/')
                {
                    char? peek = Peek();

                    if (peek != null && peek == '*')
                    {
                        while(next != '/' && !End())
                        {
                            EatWhile(HtmlRegex.NegateAsterisk());
                            Next();
                            next = Next();
                        }

                        next = '\0';
                    }
                }
            }

            return wereAnyEaten;
        }

        /// <summary>
        /// CSS content values terminate on ; only when that ; is not
        /// inside a quoted string.
        /// </summary>
        public void FindContentEnd()
        {
            char? quoted = null;
            char? _c = null;

            while (!End())
            {
                var c = Next();

                if(c == '"' || c == '\'')
                {
                    if (quoted == null)
                        quoted = c;
                    else if (quoted == c && _c != '\\')
                        quoted = null;
                    continue;
                }
                
                if (c == ';' && quoted == null)
                    break;

                _c = c;
            }

            pos--;
        }

        /// <summary>
        /// Strip any CSS comment block starting at this position.
        /// if there is no such block, don't do anything to the stream.
        /// </summary>
        public void StripCommentBlock()
        {
            if (SubStream(2) != "/*")
                return;

            pos += 2;

            for(; !End(); pos++)
            {
                if(SubStream(2) == "*/")
                {
                    pos += 2;
                    break;
                }
            }

            EatSpace();
        }

        /// <summary>
        /// Will set the start for the next token to
        /// the current stream position (i.e., "where we are now").
        /// </summary>
        public void MarkTokenStart() => TokenStart = pos;

        /// <summary>
        /// Is a wrapper function for eating
        /// up space, then marking the start for a new token.
        /// </summary>
        public void MarkTokenStartAfterSpace()
        {
            EatSpace();
            MarkTokenStart();
        }

        /// <summary>
        /// Generates a JSON-serializable token object
        /// representing the interval of text between the end of the last
        /// generated token and the current stream position.
        /// </summary>
        /// <returns></returns>
        public Token? MakeToken()
        {
            if (pos == TokenStart)
                return null;

            var token = new Token
            {
                Value = text[TokenStart..pos],
                Interval = new Interval(TokenStart, pos),
            };

            TokenStart = pos;

            return token;
        }

        /// <summary>
        /// Acts like a multi-character eat—if *consume* is `true`
        /// or not given—or a look-ahead that doesn't update the stream
        /// position—if it is `false`. *string* must be a string. *caseFold* can
        /// be set to `true` to make the match case-insensitive.
        /// </summary>
        public bool Match(string str, bool consume, bool caseFold)
        {
            var substring = text[pos..(pos + str.Length)];

            if (caseFold)
            {
                str = str.ToLower();
                substring = substring.ToLower();
            }

            if(str == substring)
            {
                if (consume)
                    pos += str.Length;

                return true;
            }

            return false;
        }

        /// <summary>
        /// Is a look-ahead match that doesn't update the stream position
        /// by a given regular expression
        /// </summary>
        public string? FindNext(string pattern, int groupNumber)
        {
            var currentPos = pos;
            EatWhile(HtmlRegex.NegateAngleBracket());
            Next();

            var nextPos = pos;
            pos = currentPos;

            var token = SubStream(nextPos - currentPos);

            if (token == null)
                return null;

            var captureGroups = Regex.Match(token, pattern);

            pos = currentPos;

            if (captureGroups.Success)
                return captureGroups.Groups[groupNumber].Value;

            return null;
        }        
    }
}
