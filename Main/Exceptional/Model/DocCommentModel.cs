using System.Collections.Generic;
using JetBrains.DocumentModel;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;

namespace CodeGears.ReSharper.Exceptional.Model
{
    internal abstract class DocCommentModel : ModelBase
    {
        protected DocCommentBlockModel DocCommentBlockModel { get; set; }
        public List<IDocCommentNode> DocCommentNodes { get; set; }


        private DocumentRange _documentRange;
        public override DocumentRange DocumentRange
        {
            get { return _documentRange; }
        }

        protected DocCommentModel(DocCommentBlockModel docCommentBlockModel)
            : base(docCommentBlockModel.MethodDeclarationModel)
        {
            DocCommentBlockModel = docCommentBlockModel;
            this.DocCommentNodes = new List<IDocCommentNode>();
        }

        public void AddDocCommentNode(IDocCommentNode docCommentNode)
        {
            this.DocCommentNodes.Add(docCommentNode);
        }

        public virtual void Initialize()
        {
            this._documentRange = GetDocCommentRage();
        }

        protected virtual DocumentRange GetDocCommentRage()
        {
            if (this.DocCommentNodes.Count == 0)
                return DocumentRange.InvalidRange;

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