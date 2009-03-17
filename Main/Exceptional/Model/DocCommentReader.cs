using System.Collections.Generic;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.Util;

namespace CodeGears.ReSharper.Exceptional.Model
{
    /// <summary>Creates a documentation models out of <see cref="IDocCommentBlockNode"/>.</summary>
    internal static class DocCommentReader
    {
        public static List<DocCommentModel> Read(IDocCommentBlockNode docCommentBlockNode, DocCommentBlockModel docCommentBlockModel)
        {
            Logger.Assert(docCommentBlockNode != null, "[Exceptional] Given docCommentBlockNode cannot be null.");
            Logger.Assert(docCommentBlockModel != null, "[Exceptional] Given docCommentBlockModel cannot be null.");

            var result = new List<DocCommentModel>();

            if (docCommentBlockNode == null) return result;
            if (docCommentBlockModel == null) return result;

            var whitespaceNodes = new List<ITreeNode>();
            DocCommentModel currentModel = null;
            var exceptionNodeFinished = false;

            for (var currentNode = docCommentBlockNode.FirstChild; currentNode != null; currentNode = currentNode.NextSibling)
            {
                if (currentNode is IWhitespaceNode)
                {
                    whitespaceNodes.Add(currentNode);
                    continue;
                }

                if (currentNode is IDocCommentNode && exceptionNodeFinished)
                {
                    currentModel = null;
                    exceptionNodeFinished = false;
                }

                if (currentModel == null)
                {
                    currentModel = new GeneralDocCommentModel(docCommentBlockModel);
                    result.Add(currentModel);
                }

                if ((currentNode is IDocCommentNode) == false)
                {
                    currentModel.TreeNodes.Add(currentNode);
                    continue;
                }

                var text = currentNode.GetText();

                if (text.Contains("<exception"))
                {
                    currentModel.Initialize();

                    currentModel = new ExceptionDocCommentModel(docCommentBlockModel);
                    result.Add(currentModel);
                    currentModel.TreeNodes.AddRange(whitespaceNodes);
                    whitespaceNodes.Clear();
                    currentModel.TreeNodes.Add(currentNode);
                }
                else if (text.Contains("</exception>"))
                {
                    currentModel.TreeNodes.AddRange(whitespaceNodes);
                    whitespaceNodes.Clear();
                    currentModel.TreeNodes.Add(currentNode);
                    currentModel.Initialize();
                    exceptionNodeFinished = true;
                }
                else
                {
                    currentModel.TreeNodes.AddRange(whitespaceNodes);
                    whitespaceNodes.Clear();
                    currentModel.TreeNodes.Add(currentNode);
                }
            }

            if (currentModel != null)
            {
                currentModel.Initialize();
            }

            return result;
        }
    }
}