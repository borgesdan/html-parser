namespace HtmlManager.DomManager
{
    public class Document
    {
        private readonly DomBuilder domBuilder;
        private readonly DocumentFragment currentDocument;

        public Document(DomBuilder builder)
        {
            domBuilder = builder;
            currentDocument = builder.Fragment.Clone();
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

        public Node? Body
        {
            get
            {
                currentDocument.Body ??= SearchByNodeName(domBuilder.CurrentNode, "body");
                return currentDocument.Body;
            }
            set
            {
                currentDocument.Body = value;
            }
        }

        public int ChildElementCount => currentDocument.Node.ChildNodes.Count;

        public IEnumerable<Node> Children => currentDocument.Node.ChildNodes;
    }
}
