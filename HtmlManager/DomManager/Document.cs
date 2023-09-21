using HtmlManager.Dom;

namespace HtmlManager.DomManager
{
    public class Document
    {
        private readonly DomBuilder domBuilder;

        public Document(DomBuilder builder)
        {
            domBuilder = builder;
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

        public Node? Body()
        {
            return SearchByNodeName(domBuilder.CurrentNode, "body");
        }
    }
}
