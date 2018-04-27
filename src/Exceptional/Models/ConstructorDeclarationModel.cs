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
        /// <summary>Initializes a new instance of the <see cref="ConstructorDeclarationModel"/> class. </summary>
        /// <param name="constructorDeclaration">The constructor declaration. </param>
        public ConstructorDeclarationModel(IConstructorDeclaration constructorDeclaration)
            : base(null, constructorDeclaration)
        {
            if (constructorDeclaration.Initializer != null)
                ThrownExceptions.Add(new ConstructorInitializerModel(this, constructorDeclaration.Initializer, this));
            else
            {
                if (constructorDeclaration.DeclaredElement != null && constructorDeclaration.DeclaredElement.IsDefault)
                {
                    var containingType = constructorDeclaration.DeclaredElement.GetContainingType();
                    if (containingType != null)
                    {
                        var baseClass = containingType.GetSuperTypes().FirstOrDefault(t => !t.IsInterfaceType());
                        if (baseClass != null)
                        {
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
        }

        /// <summary>Gets the content block of the object. </summary>
        public override IBlock Content
        {
            get { return Node.Body; }
        }
    }
}