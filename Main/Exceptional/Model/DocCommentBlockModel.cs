/// <copyright file="DocCommentBlockModel.cs" manufacturer="CodeGears">
///   Copyright (c) CodeGears. All rights reserved.
/// </copyright>

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
        public IDocCommentBlockNode DocCommentNode { get; private set; }
        private List<DocCommentModel> DocCommentModels { get; set; }
        public List<IReference> References { get; private set; }

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

        public DocCommentBlockModel(MethodDeclarationModel methodDeclarationModel) 
            : this(methodDeclarationModel, null) { }

        public DocCommentBlockModel(MethodDeclarationModel methodDeclarationModel, IDocCommentBlockNode docCommentNode) 
            : base(methodDeclarationModel)
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
            this.DocCommentModels = DocCommentReader.Read(this.DocCommentNode, this);
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
                var docCommentBlockNode = this.GetElementFactory().CreateDocComment(exceptionDocumentation);
                var methodDeclaretionNode = this.MethodDeclarationModel.MethodDeclaration;
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