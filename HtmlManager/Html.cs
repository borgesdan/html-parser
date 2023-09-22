using System.Text.RegularExpressions;

namespace HtmlManager
{
    public class Html
    {
        public const string SvgNameSpace = "http://www.w3.org/2000/svg";
        public const string Html5Doctype = "<!DOCTYPE html>";

        /// <summary>
        /// List of void HTML elements.
        /// </summary>
        public static readonly string[] VoidElements = new[]
        {
            "area", "base", "br",
            "col", "command", "embed",
            "hr", "img", "input",
            "keygen", "link", "meta",
            "param", "source", "track", "wbr"
        };

        public static readonly string[] OmittableCloseTagElements = new[]
        {
            "p", "li", "td", "th"
        };

        public static readonly string[] HtmlElements = new[]
        {
            "a", "abbr", "address", "area", "article", "aside",
            "audio", "b", "base", "bdi", "bdo", "bgsound", "blink",
            "blockquote", "body", "br", "button", "canvas", "caption",
            "cite", "code", "col", "colgroup", "command", "datalist",
            "dd", "del", "details", "dfn", "div", "dl", "dt", "em",
            "embed", "fieldset", "figcaption", "figure", "footer",
            "form", "frame", "frameset", "h1", "h2", "h3", "h4", "h5",
            "h6", "head", "header", "hgroup", "hr", "html", "i",
            "iframe", "img", "input", "ins", "kbd", "keygen", "label",
            "legend", "li", "link", "main", "map", "mark", "marquee", "menu",
            "meta", "meter", "nav", "nobr", "noscript", "object", "ol",
            "optgroup", "option", "output", "p", "param", "pre",
            "progress", "q", "rp", "rt", "ruby", "samp", "script",
            "section", "select", "small", "source", "spacer", "span",
            "strong", "style", "sub", "summary", "sup", "svg", "table",
            "tbody", "td", "textarea", "tfoot", "th", "thead", "time",
            "title", "tr", "track", "u", "ul", "var", "video", "wbr"
        };

        public static readonly string[] SvgElements = new[]
        {
            "a", "altglyph", "altglyphdef", "altglyphitem", "animate",
            "animatecolor", "animatemotion", "animatetransform", "circle",
            "clippath", "color-profile", "cursor", "defs", "desc",
            "ellipse", "feblend", "fecolormatrix", "fecomponenttransfer",
            "fecomposite", "feconvolvematrix", "fediffuselighting",
            "fedisplacementmap", "fedistantlight", "feflood", "fefunca",
            "fefuncb", "fefuncg", "fefuncr", "fegaussianblur", "feimage",
            "femerge", "femergenode", "femorphology", "feoffset",
            "fepointlight", "fespecularlighting", "fespotlight",
            "fetile", "feturbulence", "filter", "font", "font-face",
            "font-face-format", "font-face-name", "font-face-src",
            "font-face-uri", "foreignobject", "g", "glyph", "glyphref",
            "hkern", "image", "line", "lineargradient", "marker", "mask",
            "metadata", "missing-glyph", "mpath", "path", "pattern",
            "polygon", "polyline", "radialgradient", "rect", "script",
            "set", "stop", "style", "svg", "switch", "symbol", "text",
            "textpath", "title", "tref", "tspan", "use", "view", "vkern"
        };

        public static readonly string[] AttributeNamespaces = new[]
        {
            "xlink", "xml"
        };

        public static readonly string[] ObsoleteHtmlElements = new[]
        {
            "acronym", "applet", "basefont", "big", "center",
            "dir", "font", "isindex", "listing", "noframes",
            "plaintext", "s", "strike", "tt", "xmp"
        };

        public static readonly string[] WebComponentElements = new[]
        {
            "template", "shadow", "content"
        };

        public static readonly Dictionary<string, string[]> OmittableCloseTag = new()
        {
            { "p", new string[] {
                "address", "article", "aside", "blockquote", "dir", "div", "dl",
                "fieldset", "footer", "form", "h1", "h2", "h3", "h4", "h5", "h6",
                "header", "hgroup", "hr", "main", "nav", "ol", "p", "pre",
                "section", "table", "ul" }
            },
            { "th", new string[] { "th", "td" } },
            { "td", new string[] { "th", "td" } },
            { "li", new string[] { "li"} },
        };

        public static readonly Dictionary<string, string> CharacterEntityRefs = new()
        {
            { "lt", "<" },
            { "gt", ">" },
            { "apos", "'" },
            { "quot", "\"" },
            { "amp", "&" },
        };

        public static bool IsActiveContent(string tagName, string attrName)
        {
            var list = new List<string> { "link", "script", "iframe", "object" };

            if (attrName == "href")
                return list.GetRange(0, 1).IndexOf(tagName) != -1;
            else if (attrName == "src")
                return list.GetRange(1, 2).IndexOf(tagName) != -1;
            else if (attrName == "data")
                return list.GetRange(3, 1).IndexOf(tagName) != -1;

            return false;
        }

        public static bool AllowOmmitedEndTag(string parentTagName, string tagName)
        {
            if (tagName == "p")
                return new List<string> { "a" }.IndexOf(parentTagName) != -1;

            return false;
        }

        public static bool IsCustomElement(string tagName)
        {
            return Regex.Match(tagName, "^[\\w\\d]+-[\\w\\d]+$").Index != -1;
        }

        public static bool KnownHTMLElement(string tagName)
        {
            return Array.IndexOf(VoidElements, tagName) != -1
                || Array.IndexOf(HtmlElements, tagName) != -1
                || Array.IndexOf(ObsoleteHtmlElements, tagName) != -1
                || Array.IndexOf(WebComponentElements, tagName) != -1;
        }

        public static bool KnownVoidHTMLElement(string tagName)
        {
            return Array.IndexOf(VoidElements, tagName) != -1;
        }

        public static bool KnownOmittableCloseTagHtmlElement(string tagName)
        {
            return Array.IndexOf(OmittableCloseTagElements, tagName) != -1;
        }

        public static bool KnownOmittableCloseTags(string activeTagname, string foundTagName)
        {
            return Array.IndexOf(OmittableCloseTagElements, activeTagname) != -1;
        }

        public static bool KnownSVGElement(string tagName)
        {
            return Array.IndexOf(SvgElements, tagName) != -1;
        }

        public static bool SupportedAttributeNameSpace(string ns)
        {
            return Array.IndexOf(AttributeNamespaces, ns) != -1;
        }        
    }
}