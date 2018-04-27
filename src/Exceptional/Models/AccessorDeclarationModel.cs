using JetBrains.ReSharper.Psi.CSharp.Tree;

namespace ReSharper.Exceptional.Models
{
    internal class AccessorDeclarationModel : BlockModelBase<IAccessorDeclaration>
    {
        public AccessorDeclarationModel(IAnalyzeUnit analyzeUnit, IAccessorDeclaration node, IBlockModel parentBlock)
            : base(analyzeUnit, node)
        {
            ParentBlock = parentBlock;
        }

        /// <summary>Gets the content block of the object. </summary>
        public override IBlock Content
        {
            get { return Node.Body; }
        }
    }
}