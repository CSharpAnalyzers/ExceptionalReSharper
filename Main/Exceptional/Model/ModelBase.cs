using CodeGears.ReSharper.Exceptional.Analyzers;
using JetBrains.DocumentModel;
using JetBrains.ReSharper.Psi;

namespace CodeGears.ReSharper.Exceptional.Model
{
    internal abstract class ModelBase
    {
        public MethodDeclarationModel MethodDeclarationModel { get; private set; }

        public virtual DocumentRange DocumentRange { get { return DocumentRange.InvalidRange; } }

        protected ModelBase(MethodDeclarationModel methodDeclarationModel)
        {
            MethodDeclarationModel = methodDeclarationModel;
        }

        public virtual void Accept(AnalyzerBase analyzerBase) {}

        protected IPsiModule GetPsiModule()
        {
            return this.MethodDeclarationModel.MethodDeclaration.GetPsiModule();
        }
    }
}