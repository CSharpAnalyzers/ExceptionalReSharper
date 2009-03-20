using JetBrains.ReSharper.Psi.CSharp.Tree;

namespace CodeGears.ReSharper.Exceptional.Model
{
    internal class AccessorDeclarationModel : BlockModelBase<IAccessorDeclarationNode>
    {
        public AccessorDeclarationModel(IAnalyzeUnit analyzeUnit, IAccessorDeclarationNode node)
            : base(analyzeUnit, node) { }
    }
}