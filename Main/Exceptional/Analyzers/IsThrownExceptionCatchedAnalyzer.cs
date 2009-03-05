using CodeGears.ReSharper.Exceptional.Model;
using JetBrains.ReSharper.Psi;

namespace CodeGears.ReSharper.Exceptional.Analyzers
{
    internal class IsThrownExceptionCatchedAnalyzer : AnalyzerBase
    {
        public override void Visit(ThrowStatementModel throwStatementModel)
        {
            throwStatementModel.IsCatched = Analyze(throwStatementModel);
        }

        private bool Analyze(ThrowStatementModel throwStatementModel)
        {
            ProcessContext.Instance.AssertMethodDeclaration();

            var exception = GetThrownExceptionType(throwStatementModel);
            if (exception == null) return false;

            var tryStatementModelsStack =  ProcessContext.Instance.TryStatementModelsStack;

            foreach (var tryStatementModel in tryStatementModelsStack)
            {
                if (tryStatementModel.Catches(exception))
                {
                    return true;
                }
            }

            return false;
        }

        private static IDeclaredType GetThrownExceptionType(ThrowStatementModel throwStatementModel)
        {
            if (throwStatementModel.IsRethrow)
            {
                var catchClauseModelsStack = ProcessContext.Instance.CatchClauseModelsStack;

                if (catchClauseModelsStack.Count == 0) return null;
                var outerCatchClause = catchClauseModelsStack.Peek();

                return outerCatchClause.CatchClause.ExceptionType;
            }

            return throwStatementModel.ExceptionType;
        }
    }
}