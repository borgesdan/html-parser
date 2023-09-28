using System.Text;

namespace HtmlManager
{
    public enum NodeType
    {
        ElementNode = 1,
        AttributeNode = 2,
        TextNode = 3,
        CommentNode = 4,
        DocumentFragmentNode = 11
    }

    /// <summary>
    /// This represents a superficial form of a DOM node which contains most of
    /// the properties that a regular DOM node would contain, but only the ones
    /// needed by parse.
    /// </summary>
    public class Node
    {
        private Dictionary<string, string> attributMap = new();

        public NodeType NodeType { get; set; }
        public string NodeValue { get; set; }
        public string NodeName { get; set; }
        public string? NamespaceURI { get; set; }
        public Node? ParentNode { get; set; }
        public Node? NextSibling { get; private set; }
        public bool IsVoid { get; set; }
        public List<Node> ChildNodes { get; private set; } = new List<Node>();
        public List<Node> Attributes { get; private set; } = new List<Node>();
        public DocumentFragment? OwnerDocument { get; set; }
        public HtmlParseInfo? ParseInfo { get; set; }

        public string? InnerHTML
        {
            get
            {
                if (NodeType != NodeType.ElementNode)
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
                if (NodeType == NodeType.AttributeNode)
                    return null;

                if (NodeType == NodeType.CommentNode)
                    return "<!--" + NodeValue + "-->";

                if (NodeType != NodeType.ElementNode)
                    return NodeValue;

                var attributes = attributMap;
                var attributeStr = new StringBuilder();

                attributeStr.Append(
                    string.Join(" ", attributes.Keys.Select(a => a + "=\"" + attributes[a] + "\""))
                    );

                if (attributeStr.Length > 0)
                    attributeStr.Insert(0, " ");

                var nodeName = NodeName.ToLower();
                var openingTag = "<" + nodeName + attributeStr.ToString() + ">";
                var closingTag = IsVoid ? "" : "</" + nodeName + ">";

                return openingTag + (InnerHTML ?? "") + closingTag;
            }
        }        

        public Node(NodeType nodeType, string nodeName, string? nodeValue, string? namespaceURI)
        {
            NodeType = nodeType;
            NodeValue = nodeValue ?? string.Empty;
            NamespaceURI = namespaceURI ?? string.Empty;
            IsVoid = Array.IndexOf(Html.VoidElements, nodeName) != -1;

            NodeName = NodeType switch
            {
                NodeType.ElementNode => nodeName.ToUpper(),
                NodeType.AttributeNode => nodeName.ToLower(),
                _ => nodeName,
            };
        }

        /// <summary>
        /// Add a node to the current node as a child
        /// </summary>
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

        /// <summary>
        /// Create a deep copy of the current node
        /// </summary>
        public Node Clone()
        {
            var node = new Node(NodeType, NodeName, NodeValue, NamespaceURI);
            node.NextSibling = this.NextSibling?.Clone();
            node.IsVoid = this.IsVoid;
            node.ChildNodes = this.ChildNodes.Select(c => c.Clone()).ToList();
            node.Attributes = this.Attributes.Select(c => c.Clone()).ToList();
            node.attributMap = this.attributMap;

            return node;
        }

        /// <summary>
        /// Get the attribute value of an element node
        /// </summary>
        public string? GetAttribute(string attributeName)
        {
            if (NodeType == NodeType.ElementNode && attributMap.ContainsKey(attributeName.ToLower()))
                return attributMap[attributeName.ToLower()];

            return null;
        }

        /// <summary>
        /// Set the attribute value of an element node.
        /// </summary>
        public bool SetAttribute(string attributeName, string attributeValue, bool overwrite = false)
        {
            if (NodeType != NodeType.ElementNode)
                return false;

            if (attributMap.ContainsKey(attributeName.ToLower()))
            {
                if (!overwrite)
                    return false;
                else
                    attributMap[attributeName.ToLower()] = attributeValue.ToLower();

                return true;
            }

            attributMap.Add(attributeName.ToLower(), attributeValue.ToLower());
            return true;
        }

        public void RemoveAttribute(string attributeName)
        {
            if (NodeType == NodeType.ElementNode && attributMap.ContainsKey(attributeName.ToLower()))
                attributMap.Remove(attributeName.ToLower());
        }

        public bool ContainsAttribute(string attributeName)
        {
            return attributMap.ContainsKey(attributeName.ToLower());
        }
    }
}
