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
using JetBrains.ReSharper.Psi.Resolve;
using JetBrains.ReSharper.Psi.Tree;

namespace CodeGears.ReSharper.Exceptional.Model
{
    /// <summary>Stores data about processed <see cref="IDocCommentBlockNode"/>.</summary>
    internal class DocCommentBlockModel : TreeElementModelBase<IDocCommentBlockNode>
    {
        private List<DocCommentModel> DocCommentModels { get; set; }
        public List<IReference> References { get; private set; }

        private bool IsReal
        {
            get { return this.Node != null; }
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

        public DocCommentBlockModel(IAnalyzeUnit analyzeUnit) 
            : this(analyzeUnit, null) { }

        public DocCommentBlockModel(IAnalyzeUnit analyzeUnit, IDocCommentBlockNode docCommentNode) 
            : base(analyzeUnit, docCommentNode)
        {
            DocCommentModels = new List<DocCommentModel>();
            References = new List<IReference>();

            Preprocess();
        }

        private void Preprocess()
        {
            if (this.Node == null) return;

            this.References.AddRange(this.Node.GetFirstClassReferences());
            this.DocCommentModels = DocCommentReader.Read(this.Node, this);
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
                docCommentBlockNode  = this.AnalyzeUnit.AddDocCommentNode(docCommentBlockNode);
                this.Node = docCommentBlockNode;
                Preprocess();

                CSharpCodeFormatter.Instance.Format(this.Node, CodeFormatProfile.INDENT);

                result = this.DocCommentModels[1] as ExceptionDocCommentModel;
            }
            else
            {
                exceptionDocumentation += exceptionDocumentation;
                var docCommentBlockNode = CSharpElementFactory.GetInstance(this.AnalyzeUnit.GetPsiModule()).CreateDocComment(exceptionDocumentation);

                var commentNode = docCommentBlockNode.FirstChild as IDocCommentNode;

                if (commentNode == null) return null;

                var spaces = commentNode.NextSibling;

                if (this.Node.LastChild != null)
                {
                    LowLevelModificationUtil.AddChildAfter(this.Node.LastChild, spaces, commentNode);
                }
                else
                {
                    LowLevelModificationUtil.AddChild(this.Node, spaces, commentNode);
                }

                CSharpCodeFormatter.Instance.Format(this.Node, CodeFormatProfile.INDENT);

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
            CSharpCodeFormatter.Instance.Format(this.Node, CodeFormatProfile.SOFT);
        }
    }
}