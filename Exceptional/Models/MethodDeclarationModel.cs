using JetBrains.ReSharper.Psi.CSharp.Tree;
using ReSharper.Exceptional.Settings;

namespace ReSharper.Exceptional.Models
{
    /// <summary>Stores data about processed <see cref="IMethodDeclaration"/></summary>
    internal class MethodDeclarationModel : AnalyzeUnitModelBase<IMethodDeclaration>
    {
        public MethodDeclarationModel(IMethodDeclaration methodDeclaration, ExceptionalSettings settings)
            : base(null, methodDeclaration, settings)
        {
        }

        public override IBlock Contents
        {
            get { return Node.Body; }
        }
    }
}