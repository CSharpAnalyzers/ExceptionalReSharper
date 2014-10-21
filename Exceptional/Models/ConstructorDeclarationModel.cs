using JetBrains.ReSharper.Psi.CSharp.Tree;
using ReSharper.Exceptional.Settings;

namespace ReSharper.Exceptional.Models
{
    /// <summary>Stores data about processed <see cref="IConstructorDeclaration"/></summary>
    internal class ConstructorDeclarationModel : AnalyzeUnitModelBase<IConstructorDeclaration>
    {
        public ConstructorDeclarationModel(IConstructorDeclaration constructorDeclaration, ExceptionalSettings settings)
            : base(null, constructorDeclaration, settings)
        {
        }

        public override IBlock Contents
        {
            get { return Node.Body; }
        }
    }
}