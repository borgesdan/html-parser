using HtmlManager.DomManager;

namespace HtmlManager
{
    public class HtmlParserResult
    {
        public IEnumerable<string> Warnings { get; private set; }
        public IEnumerable<string> Errors { get; private set; }
        public IEnumerable<string> InternalErrors { get; private set; }
        public Document Document { get; private set; }

        public HtmlParserResult(DomBuilder domBuilder, IEnumerable<string> warnings, IEnumerable<string> errors, IEnumerable<string> internalErrors)
        {
            Warnings = warnings;
            Errors = errors;
            Document = new Document(domBuilder.Fragment);
            InternalErrors = internalErrors;
        }
    }
}
