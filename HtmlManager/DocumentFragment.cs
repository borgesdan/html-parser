﻿namespace HtmlManager
{
    public class DocumentFragment
    {
        public Node Node { get; set; }
        public Node? Body { get; set; }
        public Node? Head { get; set; }
        public Node? Title { get; set; }

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
                Node = Node.Clone(),
                Body = Body?.Clone(),
            };
        }

        public static Node CreateTextNode(string data)
        {
            return new(NodeType.TextNode, "#text", data, null);
        }

        public static Node CreateElementNS(string? namespaceURI, string qualifiedName)
        {
            return new(NodeType.ElementNode, qualifiedName, null, namespaceURI);
        }

        public static Node CreateElementNS(string? namespaceURI, string qualifiedName, DocumentFragment documentFragment)
        {
            var node = CreateElementNS(namespaceURI, qualifiedName);

            GetElements(node, documentFragment);

            return node;
        }

        public static Node CreateElement(string tagName)
        {
            return CreateElementNS(null, tagName);
        }

        public static Node CreateElement(string tagName, DocumentFragment documentFragment)
        {
            return CreateElementNS(null, tagName, documentFragment);
        }

        public static Node CreateComment(string data)
        {
            return new(NodeType.CommentNode, "#comment", data, null);
        }

        public static Node CreateAttribute(string name)
        {
            return new(NodeType.AttributeNode, name, null, null);
        }

        private static void GetElements(Node node, DocumentFragment documentFragment)
        {
            var nodeName = node.NodeName.ToLower();

            switch (nodeName)
            {
                case "body":
                case "frameset":
                    documentFragment.Body = node;
                    break;
                case "head":
                    documentFragment.Head = node;
                    break;
                case "title":
                    documentFragment.Title = node;
                    break;
            }
        }
    }
}
