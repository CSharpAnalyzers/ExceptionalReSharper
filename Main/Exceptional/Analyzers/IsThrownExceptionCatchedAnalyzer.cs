using System.Collections.Generic;
using CodeGears.ReSharper.Exceptional.Model;
using JetBrains.ReSharper.Psi;

namespace CodeGears.ReSharper.Exceptional.Analyzers
{
    public class IsThrownExceptionCatchedAnalyzer : Visitor
    {
        private Stack<List<IDeclaredType>> TryBlockStack { get; set; }

        public IsThrownExceptionCatchedAnalyzer(Stack<List<IDeclaredType>> tryBlockStack)
        {
            TryBlockStack = tryBlockStack;
        }

        public override void Visit(ThrowStatementModel throwStatementModel)
        {
            throwStatementModel.IsCatched = Analyze(throwStatementModel);
        }

        private bool Analyze(ThrowStatementModel throwStatementModel)
        {
            var exception = throwStatementModel.ThrowStatement.Exception.GetExpressionType() as IDeclaredType;
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
    }
}