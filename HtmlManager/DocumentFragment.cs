namespace HtmlManager
{
    public class DocumentFragment
    {
        public Node Node { get; set; }        
        public Node? Body { get; set; }

        public DocumentFragment()
        {
            Node = new Node(NodeType.DocumentFragmentNode, "#document-fragment", null, null)
            {
                OwnerDocument = this
            };
        }

        public DocumentFragment Clone()
        {
            return new DocumentFragment
            {
                Node = this.Node.Clone(),
                Body = this.Body?.Clone(),
            };
        }

        public static Node CreateTextNode(string data) => new(NodeType.TextNode, "#text", data, null);

        public static Node CreateElementNS(string? namespaceURI, string qualifiedName) => new(NodeType.ElementNode, qualifiedName, null, namespaceURI);

        public static Node CreateElementNS(string? namespaceURI, string qualifiedName, DocumentFragment documentFragment)
        {
            var node = CreateElementNS(namespaceURI, qualifiedName);
            var nodeName = node.NodeName.ToLower();

            if (documentFragment.Body == null && (nodeName == "body" || nodeName == "frameset"))
                documentFragment.Body = node;

            return node;
        }

        public static Node CreateElement(string tagName) => CreateElementNS(null, tagName);

        public static Node CreateElement(string tagName, DocumentFragment documentFragment) => CreateElementNS(null, tagName, documentFragment);

        public static Node CreateComment(string data) => new(NodeType.CommentNode, "#comment", data, null);

        public static Node CreateAttribute(string name) => new(NodeType.AttributeNode, name, null, null);
    }
}
