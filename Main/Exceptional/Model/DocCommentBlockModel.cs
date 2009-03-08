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
            for (var currentNode = this.DocCommentNode.FirstChild; currentNode != null; currentNode = currentNode.NextSibling)
            {
                if ((currentNode is IDocCommentNode) == false) continue;
                var currentDocCommentNode = currentNode as IDocCommentNode;
                var text = currentNode.GetText();

                if (text.Contains("<exception"))
                {
                    if (currentModel != null)
                    {
                        currentModel.Initialize();
                    }

                    currentModel = new ExceptionDocCommentModel(this);
                    currentModel.AddDocCommentNode(currentDocCommentNode);
                    this.DocCommentModels.Add(currentModel);
                }
                else if (text.Contains("</exception>") && currentModel != null)
                {
                    currentModel.AddDocCommentNode(currentDocCommentNode);
                    currentModel.Initialize();
                    currentModel = null;
                }
                else
                {
                    if (currentModel == null)
                    {
                        currentModel = new GeneralDocCommentModel(this);
                        this.DocCommentModels.Add(currentModel);
                    }

                    currentModel.AddDocCommentNode(currentDocCommentNode);
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

            var firstNode = exceptionDocCommentModel.DocCommentNodes[0];
            var lastNode = exceptionDocCommentModel.DocCommentNodes[exceptionDocCommentModel.DocCommentNodes.Count - 1];

            LowLevelModificationUtil.DeleteChildRange(firstNode, lastNode);
            CSharpCodeFormatter.Instance.Format(this.DocCommentNode, CodeFormatProfile.INDENT);
        }
    }
}