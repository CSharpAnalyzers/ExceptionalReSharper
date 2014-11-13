using System.Linq;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Util;
using ReSharper.Exceptional.Models.ExceptionsOrigins;
using ReSharper.Exceptional.Settings;

namespace ReSharper.Exceptional.Models
{
    /// <summary>Stores data about processed <see cref="IConstructorDeclaration"/></summary>
    internal class ConstructorDeclarationModel : AnalyzeUnitModelBase<IConstructorDeclaration>
    {
        public ConstructorDeclarationModel(IConstructorDeclaration constructorDeclaration, ExceptionalSettings settings)
            : base(null, constructorDeclaration, settings)
        {
            if (constructorDeclaration.Initializer != null)
                ThrownExceptions.Add(new ConstructorInitializerModel(this, constructorDeclaration.Initializer, this));
            else
            {
                if (constructorDeclaration.DeclaredElement.IsDefault)
                {
                    var containingType = constructorDeclaration.DeclaredElement.GetContainingType();
                    if (containingType != null)
                    {
                        var baseClass = containingType.GetSuperTypes().First(t => !t.IsInterfaceType());
                        var baseClassTypeElement = baseClass.GetTypeElement();
                        if (baseClassTypeElement != null)
                        {
                            IConstructor defaultBaseConstructor = baseClassTypeElement.Constructors.First(c => c.IsDefault);
                            if (defaultBaseConstructor != null)
                                ThrownExceptions.Add(new ConstructorInitializerModel(this, defaultBaseConstructor, this));
                        }
                    }
                }
            }
        }

        /// <summary>Gets the content block of the object. </summary>
        public override IBlock Content
        {
            get { return Node.Body; }
        }
    }
}