using System.Collections.Generic;
using CodeGears.ReSharper.Exceptional.Model;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Tree;

namespace CodeGears.ReSharper.Exceptional.Analyzers
{
    public class HasInnerExceptionFromOuterCatchClauseAnalyzer : Visitor
    {
        private Stack<CatchClauseModel> OuterCatchClauses { get; set; }

        public HasInnerExceptionFromOuterCatchClauseAnalyzer(Stack<CatchClauseModel> outerCatchClauses)
        {
            OuterCatchClauses = outerCatchClauses;
        }

        public override void Visit(ThrowStatementModel throwStatementModel)
        {
            throwStatementModel.ThrowsWithInnerException = Analyze(throwStatementModel);
        }

        private bool Analyze(ThrowStatementModel throwStatementModel)
        {
            if (this.OuterCatchClauses == null) return true;
            if (this.OuterCatchClauses.Count == 0) return true;

            var catchModel = this.OuterCatchClauses.Peek();
            var outerCatch = catchModel.CatchClause as ILocalScope;
            if (outerCatch == null) return false;

            if(throwStatementModel.IsRethrow)//rethrow case
            {
                catchModel.IsRethrown = true;
                throwStatementModel.ExceptionType = catchModel.CatchClause.ExceptionType;
                return true;
            }

            //Catch clause with no named parameter
            if (outerCatch.LocalVariables.Count == 0) return false;

            var list = new List<IDeclaredElement>(outerCatch.LocalVariables);
            var catchVariable = list.Find(element => element is ICatchVariableDeclaration);
            if (catchVariable == null) return false;

            var exception = throwStatementModel.ThrowStatement.Exception as IObjectCreationExpressionNode;
            if (exception == null) return false;

            var arguments = new List<ICSharpArgumentNode>(exception.ArgumentList.Arguments);
            var match = arguments.Find(arg =>
            {
                var reference = arg.ValueNode as IReferenceExpressionNode;
                if (reference == null) return false;

                return reference.NameIdentifier.Name.Equals(catchVariable.ShortName);
            });

            var result = match != null;

            catchModel.IsRethrown = result;

            return result;
        }
    }
}