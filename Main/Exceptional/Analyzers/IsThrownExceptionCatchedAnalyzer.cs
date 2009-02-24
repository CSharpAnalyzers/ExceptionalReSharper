using System.Collections.Generic;
using CodeGears.ReSharper.Exceptional.Model;
using JetBrains.ReSharper.Psi;

namespace CodeGears.ReSharper.Exceptional.Analyzers
{
    public class IsThrownExceptionCatchedAnalyzer : Visitor
    {
        private Stack<List<IDeclaredType>> TryBlockStack { get; set; }
        private Stack<CatchClauseModel> CatchClauses { get; set; }

        public IsThrownExceptionCatchedAnalyzer(Stack<List<IDeclaredType>> tryBlockStack, Stack<CatchClauseModel> catchClauses)
        {
            TryBlockStack = tryBlockStack;
            CatchClauses = catchClauses;
        }

        public override void Visit(ThrowStatementModel throwStatementModel)
        {
            throwStatementModel.IsCatched = Analyze(throwStatementModel);
        }

        private bool Analyze(ThrowStatementModel throwStatementModel)
        {
            var exception = GetThrownExceptionType(throwStatementModel);
            if (exception == null) return false;

            foreach (var list in this.TryBlockStack)
            {
                foreach (var type in list)
                {
                    if (type.Equals(exception))
                        return true;
                }
            }

            return false;
        }

        private IDeclaredType GetThrownExceptionType(ThrowStatementModel throwStatementModel)
        {
            if (throwStatementModel.IsRethrow)
            {
                if (this.CatchClauses.Count == 0) return null;
                var outerCatchClause = this.CatchClauses.Peek();

                return outerCatchClause.CatchClause.ExceptionType;
            }

            return throwStatementModel.ExceptionType;
        }
    }
}