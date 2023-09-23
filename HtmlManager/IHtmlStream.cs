using System.Text.RegularExpressions;
using static System.Net.Mime.MediaTypeNames;

namespace HtmlManager
{
    public interface IHtmlStream
    {
        int Position { get; }
        int TokenStart { get; set; }
        string Text { get; }

        char? Peek();
        string? SubStream(int len);
        bool End();
        char? Next();
        void Rewind(int x);
        char? Eat(Regex match);        
        bool EatWhile(Regex matcher);
        bool EatSpace();        
        bool EatCssWhile(Regex matcher);
        void FindContentEnd();
        void StripCommentBlock();
        void MarkTokenStart();
        void MarkTokenStartAfterSpace();
        Token? MakeToken();
        bool Match(string str, bool consume, bool caseFold);
        string? FindNext(string pattern, int groupNumber);
    }
}
