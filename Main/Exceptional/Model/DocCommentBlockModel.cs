using System;
using System.Collections.Generic;
using CodeGears.ReSharper.Exceptional.Analyzers;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CodeStyle;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.CodeStyle;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.ExtensionsAPI;
using JetBrains.ReSharper.Psi.Tree;

namespace CodeGears.ReSharper.Exceptional.Model
{
    /// <summary>Stores data about processed <see cref="IDocCommentBlockNode"/>.</summary>
    internal class DocCommentBlockModel : ModelBase
    {
        public IDocCommentBlockNode DocCommentNode { get; set; }
        private List<DocCommentModel> DocCommentModels { get; set; }

        public IEnumerable<ExceptionDocCommentModel> ExceptionDocCommentModels
        {
            get
            {
                foreach (var docCommentModel in this.DocCommentModels)
                {
                    if(docCommentModel is ExceptionDocCommentModel)
                    {
                        yield return docCommentModel as ExceptionDocCommentModel;
                    }
                }
            }
        }

        public DocCommentBlockModel(MethodDeclarationModel methodDeclarationModel, IDocCommentBlockNode docCommentNode) : base(methodDeclarationModel)
        {
            DocCommentNode = docCommentNode;
            DocCommentModels = new List<DocCommentModel>();

            InitializeExceptionsDocumentation();
        }

        private void InitializeExceptionsDocumentation()
        {
            DocCommentModel currentModel = null;
            var exceptionNodeFinished = false;
            for (var currentNode = this.DocCommentNode.FirstChild; currentNode != null; currentNode = currentNode.NextSibling)
            {
                if(currentNode is IDocCommentNode && exceptionNodeFinished)
                {
                    currentModel = null;
                    exceptionNodeFinished = false;
                }

                if(currentModel == null)
                {
                    currentModel = new GeneralDocCommentModel(this);
                    this.DocCommentModels.Add(currentModel);
                }

                if((currentNode is IDocCommentNode) == false)
                {
                    currentModel.TreeNodes.Add(currentNode);
                    continue;
                }

                var text = currentNode.GetText();

                if (text.Contains("<exception"))
                {
                    currentModel.Initialize();

                    currentModel = new ExceptionDocCommentModel(this);
                    this.DocCommentModels.Add(currentModel);
                    currentModel.TreeNodes.Add(currentNode);
                }
                else if (text.Contains("</exception>"))
                {
                    currentModel.TreeNodes.Add(currentNode);
                    currentModel.Initialize();
                    exceptionNodeFinished = true;
                }
                else
                {
                    currentModel.TreeNodes.Add(currentNode);
                }
            }

            if (currentModel != null)
            {
                currentModel.Initialize();
            }
        }

        public override void Accept(AnalyzerBase analyzerBase)
        {
            analyzerBase.Visit(this);

            foreach (var docCommentModel in this.DocCommentModels)
            {
                docCommentModel.Accept(analyzerBase);
            }
        }

        public void AddExceptionDocumentation(IDeclaredType exceptionType)
        {
            if(exceptionType == null) return;

            var exceptionDocumentation = String.Format("<exception cref=\"{1}\"></exception>{0}", Environment.NewLine, exceptionType.GetCLRName());
            exceptionDocumentation += exceptionDocumentation;
            var docCommentBlockNode = CSharpElementFactory.GetInstance(this.DocCommentNode.GetProject()).CreateDocComment(exceptionDocumentation);

            var commentNode = docCommentBlockNode.FirstChild as IDocCommentNode;
            
            if(commentNode == null) return;

            var spaces = commentNode.NextSibling;

            if(this.DocCommentNode.LastChild != null)
            {
                LowLevelModificationUtil.AddChildAfter(this.DocCommentNode.LastChild, spaces, commentNode);
            }
            else
            {
                LowLevelModificationUtil.AddChild(this.DocCommentNode, spaces, commentNode);
            }

            CSharpCodeFormatter.Instance.Format(this.DocCommentNode, CodeFormatProfile.INDENT);
        }

        public void RemoveExceptionDocumentation(ExceptionDocCommentModel exceptionDocCommentModel)
        {
            if (exceptionDocCommentModel == null) return;
            if (exceptionDocCommentModel.DocCommentNodes.Count == 0) return;

            var firstNode = exceptionDocCommentModel.TreeNodes[0];
            var lastNode = exceptionDocCommentModel.TreeNodes[exceptionDocCommentModel.TreeNodes.Count - 1];

            LowLevelModificationUtil.DeleteChildRange(firstNode, lastNode);
            CSharpCodeFormatter.Instance.Format(this.DocCommentNode, CodeFormatProfile.INDENT);
        }
    }
}