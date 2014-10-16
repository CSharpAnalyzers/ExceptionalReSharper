using JetBrains.DocumentModel;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.Tree;

namespace ReSharper.Exceptional.Models
{
    internal abstract class TreeElementModelBase<T> : ModelBase where T : ITreeNode
    {
        public T Node { get; protected set; }

        public override DocumentRange DocumentRange
        {
            get { return Node.GetDocumentRange(); }
        }

        protected TreeElementModelBase(IAnalyzeUnit analyzeUnit, T node) : base(analyzeUnit)
        {
            Node = node;
        }

        protected CSharpElementFactory GetElementFactory()
        {
            return CSharpElementFactory.GetInstance(AnalyzeUnit.GetPsiModule());
        }
    }
}