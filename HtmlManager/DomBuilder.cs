namespace HtmlManager
{
    /// <summary>
    /// The DOM builder is used to construct a DOM representation of the
    /// HTML/CSS being parsed. Each node contains a `parseInfo` expando
    /// property that contains information about the text extents of the
    /// original source code that the DOM element maps to.
    ///
    /// The DOM builder is given a single document DOM object that will
    /// be used to create all necessary DOM nodes.
    /// </summary>
    public class DomBuilder
    {
        public DocumentFragment Fragment { get; private set; }
        public Node CurrentNode { get; private set; }
        public bool DisallowActiveAttributes { get; private set; }
        public List<Context> Contexts { get; private set; } = new List<Context>();

        public DomBuilder(bool disallowActiveAttributes = false)
        {
            Fragment = new DocumentFragment();
            CurrentNode = Fragment.Node;
            DisallowActiveAttributes = disallowActiveAttributes;
        }

        /// <summary>
        /// This method pushes a new element onto the DOM builder's stack.
        /// The element is appended to the currently active element and is
        /// then made the new currently active element.
        /// </summary>
        public void PushElement(string tagname, HtmlParseInfo parseInfo, string? nameSpace)
        {
            var node = nameSpace != null
                ? DocumentFragment.CreateElementNS(nameSpace, tagname, Fragment)
                : DocumentFragment.CreateElement(tagname, Fragment);

            node.ParseInfo = parseInfo;
            CurrentNode.AppendChild(node);
            CurrentNode = node;
        }

        /// <summary>
        /// This method pops the current element off the DOM builder's stack,
        /// making its parent element the currently active element.
        /// </summary>
        public void PopElement()
        {
            if (CurrentNode.ParentNode == null)
                throw new NullReferenceException("The parent node cannot be null.");

            CurrentNode = CurrentNode.ParentNode;
        }

        /// <summary>
        /// Record the cursor position for a context change (text/html/css/script)
        /// </summary>
        public void PushContext(string context, int position)
        {
            Contexts.Add(new Context(context, position));
        }

        /// <summary>
        /// This method appends an HTML comment node to the currently active element.
        /// </summary>
        public void Comment(string data, HtmlParseInfo parseInfo)
        {
            var comment = DocumentFragment.CreateComment("");
            comment.NodeValue = data;
            comment.ParseInfo = parseInfo;
            CurrentNode.AppendChild(comment);
        }

        /// <summary>
        /// This method appends an attribute to the currently active element.
        /// </summary>
        public void Attribute(string name, string value, HtmlParseInfo parseInfo)
        {
            Node attrNode = DocumentFragment.CreateAttribute(name);
            attrNode.ParseInfo = parseInfo;

            if (DisallowActiveAttributes && name[..2].ToLower() == "on")
                attrNode.NodeValue = "";
            else
                attrNode.NodeValue = value;

            CurrentNode.Attributes.Add(attrNode);

            if (CurrentNode != null && !string.IsNullOrEmpty(attrNode.NodeName))
            {
                if (CurrentNode.AttributMap.ContainsKey(attrNode.NodeName))
                    CurrentNode.AttributMap[attrNode.NodeName] = attrNode.NodeValue;
                else
                    CurrentNode.AttributMap.Add(attrNode.NodeName, attrNode.NodeValue);
            }
        }

        /// <summary>
        /// This method appends a text node to the currently active element.
        /// </summary>
        public void Text(string text, HtmlParseInfo parseInfo)
        {
            var textNode = DocumentFragment.CreateTextNode(text);
            textNode.ParseInfo = parseInfo;
            CurrentNode.AppendChild(textNode);
        }
    }
}
