using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.Tree;

namespace CodeGears.ReSharper.Exceptional.Model
{
    internal abstract class TreeElementModelBase<T> : ModelBase where T : ITreeNode
    {
        public T Node { get; protected set; }

        protected TreeElementModelBase(IAnalyzeUnit analyzeUnit, T node) : base(analyzeUnit)
        {
            Node = node;
        }       

        protected CSharpElementFactory GetElementFactory()
        {
            return CSharpElementFactory.GetInstance(this.AnalyzeUnit.GetPsiModule());
        }
    }
}