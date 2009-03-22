/// <copyright>Copyright (c) 2009 CodeGears.net All rights reserved.</copyright>

using System.Collections.Generic;
using JetBrains.DocumentModel;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;

namespace CodeGears.ReSharper.Exceptional.Model
{
    internal abstract class DocCommentModel : ModelBase
    {
        protected DocCommentBlockModel DocCommentBlockModel { get; private set; }
        public List<IDocCommentNode> DocCommentNodes { get; private set; }

        public List<ITreeNode> TreeNodes { get; private set; }

        private DocumentRange _documentRange;

        public override DocumentRange DocumentRange
        {
            get { return _documentRange; }
        }

        protected DocCommentModel(DocCommentBlockModel docCommentBlockModel)
            : base(docCommentBlockModel.AnalyzeUnit)
        {
            DocCommentBlockModel = docCommentBlockModel;
            DocCommentNodes = new List<IDocCommentNode>();
            TreeNodes = new List<ITreeNode>();
        }

        public virtual void Initialize()
        {
            foreach (var treeNode in this.TreeNodes)
            {
                if (treeNode is IDocCommentNode)
                {
                    this.DocCommentNodes.Add(treeNode as IDocCommentNode);
                }
            }

            this._documentRange = GetDocCommentRage();
        }

        protected virtual DocumentRange GetDocCommentRage()
        {
            if (this.DocCommentNodes.Count == 0)
            {
                return DocumentRange.InvalidRange;
            }

            var textRange = this.DocCommentNodes[0].GetDocumentRange().TextRange;

            for (var i = 1; i < this.DocCommentNodes.Count; i++)
            {
                var range = this.DocCommentNodes[i].GetDocumentRange().TextRange;
                textRange = textRange.Join(range);
            }

            return new DocumentRange(this.DocCommentNodes[0].GetProjectFile(), textRange);
        }
    }
}