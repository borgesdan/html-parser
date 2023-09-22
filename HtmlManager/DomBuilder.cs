namespace HtmlManager
{
    public class DomBuilder
    {
        public DocumentFragment Fragment { get; set; }
        public Node? CurrentNode { get; set; }
        public bool DisallowActiveAttributes { get; set; }
        public List<Context> Contexts { get; } = new List<Context>();

        public DomBuilder(bool disallowActiveAttributes)
        {
            Fragment = new DocumentFragment();
            CurrentNode = Fragment.Node;
            DisallowActiveAttributes = disallowActiveAttributes;
        }

        public void PushElement(string tagname, HtmlParseInfo parseInfo, string? nameSpace)
        {
            var node = nameSpace != null
                ? DocumentFragment.CreateElementNS(nameSpace, tagname, Fragment)
                : DocumentFragment.CreateElement(tagname, Fragment);

            node.ParseInfo = parseInfo;
            CurrentNode?.AppendChild(node);
            CurrentNode = node;
        }

        public void PopElement()
        {
            CurrentNode = CurrentNode?.ParentNode;
        }

        public void PushContext(string context, int position)
        {
            Contexts.Add(new Context(context, position));
        }

        public void Comment(string data, HtmlParseInfo parseInfo)
        {
            var comment = DocumentFragment.CreateComment("");
            comment.NodeValue = data;
            comment.ParseInfo = parseInfo;
            CurrentNode?.AppendChild(comment);
        }

        public void Attribute(string name, string value, HtmlParseInfo parseInfo)
        {
            Node attrNode = DocumentFragment.CreateAttribute(name);
            attrNode.ParseInfo = parseInfo;

            if (DisallowActiveAttributes && name[..2].ToLower() == "on")
                attrNode.NodeValue = "";
            else
                attrNode.NodeValue = value;

            CurrentNode?.Attributes.Add(attrNode);

            if (CurrentNode != null && attrNode.NodeName != null)
            {
                if (CurrentNode.AttributMap.ContainsKey(attrNode.NodeName))
                    CurrentNode.AttributMap[attrNode.NodeName] = attrNode.NodeValue;
                else
                    CurrentNode.AttributMap.Add(attrNode.NodeName, attrNode.NodeValue);
            }
        }

        public void Text(string text, HtmlParseInfo parseInfo)
        {
            var textNode = DocumentFragment.CreateTextNode(text);
            textNode.ParseInfo = parseInfo;
            CurrentNode?.AppendChild(textNode);
        }
    }
}
