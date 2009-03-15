using System;
using System.Collections.Generic;
using CodeGears.ReSharper.Exceptional.Analyzers;
using JetBrains.Application;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CodeStyle;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.CodeStyle;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.ExtensionsAPI;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Tree;
using JetBrains.ReSharper.Psi.Resolve;
using JetBrains.ReSharper.Psi.Tree;

namespace CodeGears.ReSharper.Exceptional.Model
{
    /// <summary>Stores data about processed <see cref="IDocCommentBlockNode"/>.</summary>
    internal class DocCommentBlockModel : ModelBase
    {
        public IDocCommentBlockNode DocCommentNode { get; set; }
        private List<DocCommentModel> DocCommentModels { get; set; }

        private bool IsReal
        {
            get { return this.DocCommentNode != null; }
        }

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

        public List<IReference> References { get; private set; }

        public DocCommentBlockModel(MethodDeclarationModel methodDeclarationModel) : this(methodDeclarationModel, null)
        { }

        public DocCommentBlockModel(MethodDeclarationModel methodDeclarationModel, IDocCommentBlockNode docCommentNode) : base(methodDeclarationModel)
        {
            DocCommentNode = docCommentNode;
            DocCommentModels = new List<DocCommentModel>();
            References = new List<IReference>();

            Preprocess();
        }

        private void Preprocess()
        {
            if (this.DocCommentNode == null) return;

            this.References.AddRange(this.DocCommentNode.GetFirstClassReferences());
            InitializeExceptionsDocumentation();
        }

        private void InitializeExceptionsDocumentation()
        {
            var whitespaceNodes = new List<ITreeNode>();
            DocCommentModel currentModel = null;
            var exceptionNodeFinished = false;

            for (var currentNode = this.DocCommentNode.FirstChild; currentNode != null; currentNode = currentNode.NextSibling)
            {
                if(currentNode is IWhitespaceNode)
                {
                    whitespaceNodes.Add(currentNode);
                    continue;
                }

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
        }

        public override void Accept(AnalyzerBase analyzerBase)
        {
            foreach (var docCommentModel in this.DocCommentModels)
            {
                docCommentModel.Accept(analyzerBase);
            }
        }

        public ExceptionDocCommentModel AddExceptionDocumentation(IDeclaredType exceptionType)
        {
            if(exceptionType == null) return null;

            Shell.Instance.Locks.AcquireWriteLock();

            ExceptionDocCommentModel result;

            var exceptionDocumentation = String.Format("<exception cref=\"{1}\">[MARKER]</exception>{0}", Environment.NewLine, exceptionType.GetCLRName());

            if (this.IsReal == false)
            {
                var docCommentBlockNode = CSharpElementFactory.GetInstance(this.GetPsiModule()).CreateDocComment(exceptionDocumentation);
                var methodDeclaretionNode = this.MethodDeclarationModel.MethodDeclaration as IMethodDeclarationNode;
                docCommentBlockNode = ModificationUtil.AddChildBefore(methodDeclaretionNode, methodDeclaretionNode.FirstChild, docCommentBlockNode);
                this.DocCommentNode = docCommentBlockNode;
                Preprocess();

                CSharpCodeFormatter.Instance.Format(this.DocCommentNode, CodeFormatProfile.INDENT);

                result = this.DocCommentModels[1] as ExceptionDocCommentModel;
            }
            else
            {
                exceptionDocumentation += exceptionDocumentation;
                var docCommentBlockNode = CSharpElementFactory.GetInstance(this.GetPsiModule()).CreateDocComment(exceptionDocumentation);

                var commentNode = docCommentBlockNode.FirstChild as IDocCommentNode;

                if (commentNode == null) return null;

                var spaces = commentNode.NextSibling;

                if (this.DocCommentNode.LastChild != null)
                {
                    LowLevelModificationUtil.AddChildAfter(this.DocCommentNode.LastChild, spaces, commentNode);
                }
                else
                {
                    LowLevelModificationUtil.AddChild(this.DocCommentNode, spaces, commentNode);
                }

                CSharpCodeFormatter.Instance.Format(this.DocCommentNode, CodeFormatProfile.INDENT);

                result = new ExceptionDocCommentModel(this);
                result.TreeNodes.Add(commentNode);
                result.Initialize();
                this.DocCommentModels.Add(result);
            }

            Shell.Instance.Locks.ReleaseWriteLock();

            return result;
        }

        public void RemoveExceptionDocumentation(ExceptionDocCommentModel exceptionDocCommentModel)
        {
            if (exceptionDocCommentModel == null) return;
            if (exceptionDocCommentModel.DocCommentNodes.Count == 0) return;

            var firstNode = exceptionDocCommentModel.TreeNodes[0];
            var lastNode = exceptionDocCommentModel.TreeNodes[exceptionDocCommentModel.TreeNodes.Count - 1];

            LowLevelModificationUtil.DeleteChildRange(firstNode, lastNode);
            CSharpCodeFormatter.Instance.Format(this.DocCommentNode, CodeFormatProfile.SOFT);
        }
    }
}