using JetBrains.ReSharper.Psi.CSharp.Tree;

namespace ReSharper.Exceptional.Models
{
    internal class AccessorDeclarationModel : BlockModelBase<IAccessorDeclaration>
    {
        public AccessorDeclarationModel(IAnalyzeUnit analyzeUnit, IAccessorDeclaration node)
            : base(analyzeUnit, node)
        {
        }

        /// <summary>Gets the content block of the object. </summary>
        public override IBlock Contents
        {
            get { return Node.Body; }
        }
    }
}