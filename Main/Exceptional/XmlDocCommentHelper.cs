using System.Xml;
using JetBrains.DocumentModel;
using JetBrains.ReSharper.Psi.Tree;

namespace CodeGears.ReSharper.Exceptional
{
    public class XmlDocCommentHelper
    {
        public static DocumentRange FindRangeOfExceptionTag(XmlNode node, string exceptionType, IDocCommentBlockNode docNode)
        {
            if (docNode == null) return DocumentRange.InvalidRange;

            for (var currentNode = docNode.FirstChild; currentNode != null; currentNode = currentNode.NextSibling)
            {
                var text = currentNode.GetText();
                if (text.Contains("<exception") == false) continue;

                var index = exceptionType.LastIndexOf('.');
                var exceptiontypeName = exceptionType.Substring(index + 1);

                if (text.Contains(exceptiontypeName) && text.Contains(node.InnerText))
                {
                    return currentNode.GetDocumentRange();
                }
            }

            return DocumentRange.InvalidRange;
        }
    }
}