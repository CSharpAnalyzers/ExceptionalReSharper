using System.Collections.Generic;
using System.Linq;
using JetBrains.DocumentModel;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;

namespace ReSharper.Exceptional.Models
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
            DocCommentNodes.AddRange(TreeNodes.OfType<IDocCommentNode>());
            _documentRange = GetDocCommentRage();
        }

        protected virtual DocumentRange GetDocCommentRage()
        {
            if (DocCommentNodes.Count == 0)
            {
                return DocumentRange.InvalidRange;
            }

            var textRange = DocCommentNodes[0].GetDocumentRange().TextRange;

            for (var i = 1; i < DocCommentNodes.Count; i++)
            {
                var range = DocCommentNodes[i].GetDocumentRange().TextRange;
                textRange = textRange.Join(range);
            }

            return new DocumentRange(DocCommentNodes[0].GetSourceFile().Document, textRange);
        }
    }
}