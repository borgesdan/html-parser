using System.Text;

namespace HtmlManager.Dom
{
    public class Node
    {
        public Dictionary<string, string> AttributMap { get; set; } = new();
        public int NodeType { get; set; }
        public string? NodeValue { get; set; }
        public string? NamespaceURI { get; set; }
        public string? NodeName { get; set; }
        public Node? ParentNode { get; set; }
        public Node? NextSibling { get; private set; }
        public bool IsVoid { get; set; }
        public List<Node> ChildNodes { get; private set; } = new List<Node>();
        public List<Node> Attributes { get; set; } = new List<Node>();
        public DocumentFragment? OwnerDocument { get; set; }
        public HtmlParseInfo? ParseInfo { get; set; }

        public string? InnerHTML
        {
            get
            {
                if (NodeType != ELEMENT_NODE)
                    return null;

                string innerHTML = string.Empty;

                ChildNodes.ForEach(child => innerHTML += child.OuterHTML ?? string.Empty);

                return innerHTML;
            }
        }

        public string? OuterHTML
        {
            get
            {
                if (NodeType == ATTRIBUTE_NODE)
                    return null;

                if (NodeType == COMMENT_NODE)
                    return "<!--" + NodeValue + "-->";

                if (NodeType != ELEMENT_NODE)
                    return NodeValue;

                var attributes = AttributMap;
                var attributeStr = new StringBuilder();
                attributeStr.Append(
                    string.Join(" ", attributes.Keys.Select(a => a + "=\"" + attributes[a] + "\""))
                    );     

                if (attributeStr.Length > 0)
                    attributeStr.Insert(0, " ");

                var nodeName = NodeName?.ToLower();
                var openingTag = "<" + nodeName + attributeStr.ToString() + ">";
                var closingTag = IsVoid ? "" : "</" + nodeName + ">";

                return openingTag + (InnerHTML ?? "") + closingTag;
            }
        }

        public const int ELEMENT_NODE = 1;
        public const int ATTRIBUTE_NODE = 2;
        public const int TEXT_NODE = 3;
        public const int COMMENT_NODE = 4;
        public const int DOCUMENT_FRAGMENT_NODE = 11;

        public Node(int nodeType, string? nodeName, string? nodeValue, string? namespaceURI)
        {
            NodeType = nodeType;
            NodeValue = nodeValue ?? string.Empty;
            NamespaceURI = namespaceURI ?? string.Empty;
            IsVoid = Array.IndexOf(Html.VoidElements, nodeName) != -1;

            NodeName = NodeType switch
            {
                ELEMENT_NODE => nodeName?.ToUpper(),
                ATTRIBUTE_NODE => nodeName?.ToLower(),
                _ => nodeName,
            };
        }

        public void AppendChild(Node node)
        {
            if (ChildNodes.Any())
            {
                var lastChild = ChildNodes.Last();
                lastChild.NextSibling = node;
            }

            node.ParentNode = this;
            ChildNodes.Add(node);
        }

        public Node Clone()
        {
            var node = new Node(NodeType, NodeName, NodeValue, NamespaceURI)
            {
                NextSibling = NextSibling?.Clone(),
                IsVoid = IsVoid,
                ChildNodes = ChildNodes.Select(c => c.Clone()).ToList(),
                Attributes = Attributes.Select(c => c.Clone()).ToList(),
                AttributMap = AttributMap
            };

            return node;
        }

        public bool GetAttribute(string name)
        {
            return NodeType == ELEMENT_NODE && AttributMap.ContainsKey(name.ToLower());
        }
    }
}
