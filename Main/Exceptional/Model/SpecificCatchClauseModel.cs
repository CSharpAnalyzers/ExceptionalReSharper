using System;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Tree;

namespace CodeGears.ReSharper.Exceptional.Model
{
    internal class SpecificCatchClauseModel : CatchClauseModel
    {
        private ISpecificCatchClauseNode SpecificCatchClauseNode
        {
            get { return this.CatchClauseNode as ISpecificCatchClauseNode; }
        }

        public SpecificCatchClauseModel(MethodDeclarationModel methodDeclarationModel, ICatchClauseNode catchClause) 
            : base(methodDeclarationModel, catchClause) { }

        public override void AddCatchVariable(string variableName)
        {
            if (this.HasVariable) return;

            if (String.IsNullOrEmpty(variableName))
            {
                variableName = SuggestVariableName();
            }

            var exceptionType = this.SpecificCatchClauseNode.ExceptionTypeUsage.GetText();

            var tempTry = this.GetElementFactory().CreateStatement("try {} catch($0 $1) {}", exceptionType, variableName) as ITryStatementNode;
            if (tempTry == null) return;

            var tempCatch = tempTry.Catches[0] as ISpecificCatchClauseNode;
            if (tempCatch == null) return;

            var resultVariable = this.SpecificCatchClauseNode.SetExceptionDeclarationNode(tempCatch.ExceptionDeclarationNode);
            this.VariableModel = new CatchVariableModel(this.MethodDeclarationModel, resultVariable);
        }

        public override IDeclaredType GetCatchedException()
        {
            return this.SpecificCatchClauseNode.ExceptionType;
        }

        public override bool Catches(IDeclaredType exception)
        {
            if (exception == null) return false;

            return this.SpecificCatchClauseNode.ExceptionType.GetCLRName().Equals(exception.GetCLRName());
        }

        public override bool HasExceptionType
        {
            get { return true; }
        }
    }
}