using JetBrains.ReSharper.Psi.CSharp.Tree;

namespace ReSharper.Exceptional.Models
{
    internal class AccessorDeclarationModel : BlockModelBase<IAccessorDeclaration>
    {
        public AccessorDeclarationModel(IAnalyzeUnit analyzeUnit, IAccessorDeclaration node)
            : base(analyzeUnit, node)
        {
        }

        public override IBlock Contents
        {
            get { return Node.Body; }
        }
    }
}