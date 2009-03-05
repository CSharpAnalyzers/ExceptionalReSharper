using System.Collections.Generic;
using CodeGears.ReSharper.Exceptional.Model;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Tree;

namespace CodeGears.ReSharper.Exceptional.Analyzers
{
    internal class HasInnerExceptionFromOuterCatchClauseAnalyzer : AnalyzerBase
    {
        public override void Visit(ThrowStatementModel throwStatementModel)
        {
            throwStatementModel.ThrowsWithInnerException = Analyze(throwStatementModel);
        }

        private bool Analyze(ThrowStatementModel throwStatementModel)
        {
            ProcessContext.Instance.AssertMethodDeclaration();

            var catchClauseModelsStack =  ProcessContext.Instance.CatchClauseModelsStack;
            
            if (catchClauseModelsStack.Count == 0) return true;

            var catchModel = catchClauseModelsStack.Peek();

            throwStatementModel.OuterCatchClauseModel = catchModel;
            throwStatementModel.ExceptionType = catchModel.CatchClause.ExceptionType;
            catchModel.IsRethrown = throwStatementModel.IsRethrow;

            var outerCatch = catchModel.CatchClause as ILocalScope;

            //catch clause with no exception defined = Exception
            if (outerCatch == null)
            {
                return throwStatementModel.IsRethrow;
            }

            if(throwStatementModel.IsRethrow)//rethrow case
            {
                return true;
            }

            //Catch clause with no named parameter
            if (outerCatch.LocalVariables.Count == 0)
            {
                return false;
            }

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