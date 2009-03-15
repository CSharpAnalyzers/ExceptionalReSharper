using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;

namespace CodeGears.ReSharper.Exceptional.Model
{
    internal class GeneralCatchClauseModel : CatchClauseModel
    {
        public IGeneralCatchClause GeneralCatchClause
        {
            get { return this.CatchClause as IGeneralCatchClause; }
        }

        public IGeneralCatchClauseNode GeneralCatchClauseNode
        {
            get { return this.CatchClause as IGeneralCatchClauseNode; }
        }

        public GeneralCatchClauseModel(MethodDeclarationModel methodDeclarationModel, ICatchClause catchClause) 
            : base(methodDeclarationModel, catchClause) { }

        public override void AddVariable()
        {
            if (this.HasVariable) return;

            var codeFactory = new CodeElementFactory(this.CatchClause.GetPsiModule());

            var newCatch = codeFactory.CreateSpecificCatchClause(null, this.CatchClause.Body);
            if (newCatch == null) return;

            this.GeneralCatchClause.ReplaceBy(newCatch);

            this.CatchClause = newCatch;
            this.VariableModel = new CatchVariableModel(this.MethodDeclarationModel, newCatch.ExceptionDeclaration);
        }

        public override IDeclaredType GetCatchedException()
        {
            return TypeFactory.CreateTypeByCLRName("System.Exception", this.CatchClause.GetPsiModule());
        }

        public override bool Catches(IDeclaredType exception)
        {
            if (exception == null) return false;

            return exception.GetCLRName().Equals("System.Exception");
        }

        public override bool HasExceptionType
        {
            get { return false; }
        }
    }
}