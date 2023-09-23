namespace HtmlManager.DomManager
{
    public class Document
    {
        private readonly DocumentFragment sourceDocument;
        private readonly DocumentFragment currentDocument;

        public Document(DocumentFragment document)
        {
            sourceDocument = document;
            currentDocument = document.Clone();
        }

        private Node? SearchByNodeName(Node node, string nodeName)
        {
            if (node.NodeName.ToLower() == nodeName.ToLower())
                return node;

            Node? nodeResult = null;

            foreach (var child in node.ChildNodes)
            {
                nodeResult = SearchByNodeName(child, nodeName);

                if (nodeResult != null)
                    break;
            }

            return nodeResult;
        }

        private Node? FindById(Node node, string id)
        {
            var attr = node.GetAttribute("id");

            if (attr == id)
                return node;

            Node? nodeResult = null;

            foreach (var child in node.ChildNodes)
            {
                nodeResult = FindById(child, id);

                if (nodeResult != null)
                    break;
            }

            return nodeResult;
        }

        public Node? Body
        {
            get
            {
                currentDocument.Body ??= SearchByNodeName(currentDocument.Node, "body");
                return currentDocument.Body;
            }
            set
            {
                if (value is not null && (value.NodeType != NodeType.ElementNode || value.NodeName.ToLower() != "body" || value.NodeName.ToLower() != "frameset"))
                    throw new FormatException("The value contains an incorret NodeType or NodeName.");

                currentDocument.Body = value;
            }
        }

        public Node? Head
        {
            get
            {
                currentDocument.Head ??= SearchByNodeName(currentDocument.Node, "head");
                return currentDocument.Head;
            }
            set
            {
                if (value is not null && (value.NodeType != NodeType.ElementNode || value.NodeName.ToLower() != "head"))
                    throw new FormatException("The value contains an incorret NodeType or NodeName.");

                currentDocument.Body = value;
            }
        }

        public Node? Title
        {
            get
            {
                currentDocument.Title ??= SearchByNodeName(currentDocument.Node, "title");
                return currentDocument.Title;
            }
            set
            {
                if (value is not null && (value.NodeType != NodeType.ElementNode || value.NodeName.ToLower() != "title"))
                    throw new FormatException("The value contains an incorret NodeType or NodeName.");

                currentDocument.Title = value;
            }
        }

        public int ChildElementCount => currentDocument.Node.ChildNodes.Count;

        public IEnumerable<Node> Children => currentDocument.Node.ChildNodes;

        /// <summary>
        /// Inserts a set of Node objects or string objects after the last child of the document. 
        /// String objects are inserted as equivalent Text nodes. 
        /// </summary>
        public void Append(params Node[] nodes)
        {
            currentDocument.Node.ChildNodes.AddRange(nodes);
        }

        /// <summary>
        /// Inserts a set of Node objects or string objects after the last child of the document. 
        /// String objects are inserted as equivalent Text nodes. 
        /// </summary>
        public void Append(params string[] textNodes)
        {
            var nodes = textNodes.Select(t => DocumentFragment.CreateTextNode(t));
            currentDocument.Node.ChildNodes.AddRange(nodes);
        }

        /// <summary>
        /// Creates a new attribute node, and returns it.
        /// </summary>
        public Node? CreateAttribute(string name)
        {
            var attribute = DocumentFragment.CreateAttribute(name);
            return attribute;
        }

        /// <summary>
        /// Creates a new attribute node with the specified namespace URI and qualified name, and returns it. 
        /// </summary>
        public Node? CreateAttributeNS(string nameSpace, string name)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Creates a new CDATA section node, and returns it. 
        /// </summary>
        public Node? CreateCDATASection(string value)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Creates a new comment node, and returns it. 
        /// </summary>
        public Node? CreateComment(string data)
        {
            var comment = DocumentFragment.CreateComment(data);
            return comment;

        }

        /// <summary>
        /// Creates a new empty DocumentFragment into which DOM nodes can be added to build an offscreen DOM tree. 
        /// </summary>
        public DocumentFragment CreateDocumentFragment()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Creates the HTML element specified by tagName
        /// </summary>
        public Node? CreateElement(string tagName)
        {
            var element = DocumentFragment.CreateElement(tagName);
            return element;
        }

        /// <summary>
        /// Creates an element with the specified namespace URI and qualified name.
        /// </summary>
        public Node? CreateElementNS(string? namespaceURI, string qualifiedName)
        {
            var element = DocumentFragment.CreateElementNS(namespaceURI, qualifiedName);
            return element;
        }

        /// <summary>
        /// Creates a new Text node.
        /// </summary>
        public Node? CreateTextNode(string data)
        {
            var text = DocumentFragment.CreateTextNode(data);
            return text;
        }

        /// <summary>
        /// Returns an Element object representing the element whose id property matches the specified string
        /// </summary>
        public Node? GetElementById(string id)
        {
            return FindById(currentDocument.Node, id);
        }

        /// <summary>
        /// Returns an array-like object of all child elements which have all of the given class name(s). 
        /// </summary>
        public IEnumerable<Node> GetElementsByClassName(string name)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns a NodeList Collection of elements with a given name attribute in the document. 
        /// </summary>
        public IEnumerable<Node> GetElementsByName(string name)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns a list of elements with the given tag name. 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public IEnumerable<Node> GetElementsByTagName(string name)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns a list of elements with the given tag name belonging to the given namespace.
        /// </summary>
        public IEnumerable<Node> GetElementsByTagNameNS(string nameSpace, string name)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Inserts a set of Node objects or string objects before the first child of the document.
        /// </summary>
        public void Prepend(params Node[] nodes)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns the first Element within the document that matches the specified selector, or group of selectors. If no matches are found, null is returned. 
        /// </summary>
        public Node? QuerySelector(string selectors)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns a static (not live) NodeList representing a list of the document's elements that match the specified group of selectors. 
        /// </summary>
        public IEnumerable<Node> QuerySelectorAll(string selectors)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Replaces the existing children of a Document with a specified new set of children. 
        /// </summary>
        public void ReplaceChildren(params Node[] nodes)
        {
            throw new NotImplementedException();
        }
    }
}
