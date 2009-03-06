using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Tree;

namespace CodeGears.ReSharper.Exceptional.Model
{
    internal class SpecificCatchClauseModel : CatchClauseModel
    {
        public ISpecificCatchClause SpecificCatchClause
        {
            get { return this.CatchClause as ISpecificCatchClause; }
        }

        public ISpecificCatchClauseNode SpecificCatchClauseNode
        {
            get { return this.CatchClause as ISpecificCatchClauseNode; }
        }

        public SpecificCatchClauseModel(MethodDeclarationModel methodDeclarationModel, ICatchClause catchClause) 
            : base(methodDeclarationModel, catchClause) { }

        public override void AddVariable()
        {
            if (this.HasVariable) return;

            var codeFactory = new CodeElementFactory(this.CatchClause.GetProject());

            var variableDeclaration =
                codeFactory.CreateCatchVariableDeclarationNode(this.SpecificCatchClause.ExceptionType);

            this.SpecificCatchClauseNode.SetExceptionDeclarationNode(variableDeclaration);
            this.VariableModel = new CatchVariableModel(this.MethodDeclarationModel, variableDeclaration);
        }

        public override IDeclaredType GetCatchedException()
        {
            return this.SpecificCatchClause.ExceptionType;
        }

        public override bool Catches(IDeclaredType exception)
        {
            if (exception == null) return false;

            return this.SpecificCatchClause.ExceptionType.GetCLRName().Equals(exception.GetCLRName());
        }

        public override bool HasExceptionType
        {
            get { return true; }
        }
    }
}