using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.Tree;

namespace CodeGears.ReSharper.Exceptional.Model
{
    internal abstract class TreeElementModelBase<T> : ModelBase where T : ITreeNode
    {
        public T Node { get; protected set; }

        protected TreeElementModelBase(MethodDeclarationModel methodDeclarationModel, T node) : base(methodDeclarationModel)
        {
            Node = node;
        }

        protected IPsiModule GetPsiModule()
        {
            return this.Node.GetPsiModule();
        }

        protected CSharpElementFactory GetElementFactory()
        {
            return CSharpElementFactory.GetInstance(this.GetPsiModule());
        }
    }
}