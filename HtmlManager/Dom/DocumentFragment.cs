namespace HtmlManager.Dom
{
    public class DocumentFragment
    {
        public Node Node { get; set; }
        public Node Body { get; private set; }

        public DocumentFragment()
        {
            Node = new Node(Node.DOCUMENT_FRAGMENT_NODE, "#document-fragment", null, null)
            {
                OwnerDocument = this
            };
        }

        public static Node CreateTextNode(string data) => new(Node.TEXT_NODE, "#text", data, null);

        public static Node CreateElementNS(string? namespaceURI, string qualifiedName) => new(Node.ELEMENT_NODE, qualifiedName, null, namespaceURI);

        public static Node CreateElement(string tagName) => CreateElementNS(null, tagName);

        public static Node CreateComment(string data) => new(Node.COMMENT_NODE, "#comment", data, null);

        public static Node CreateAttribute(string name) => new(Node.ATTRIBUTE_NODE, name, null, null);
    }
}
