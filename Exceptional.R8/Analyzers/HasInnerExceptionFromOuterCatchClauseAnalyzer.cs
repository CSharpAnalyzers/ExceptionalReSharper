using JetBrains.ReSharper.Daemon;
using JetBrains.ReSharper.Feature.Services.Daemon;
using ReSharper.Exceptional.Highlightings;
using ReSharper.Exceptional.Models.ExceptionsOrigins;

namespace ReSharper.Exceptional.Analyzers
{
    /// <summary>Analyzes throw statements and checks if the contain inner exception when thrown from inside a catch clause.</summary>
    internal class HasInnerExceptionFromOuterCatchClauseAnalyzer : AnalyzerBase
    {
        /// <summary>Performs analyze of throw <paramref name="throwStatement"/>.</summary>
        /// <param name="throwStatement">Throw statement model to analyze.</param>
        public override void Visit(ThrowStatementModel throwStatement)
        {
            if (throwStatement != null && RequiresInnerExceptionPassing(throwStatement))
            {
                var highlighting = new ThrowFromCatchWithNoInnerExceptionHighlighting(throwStatement);
                ServiceLocator.StageProcess.Hightlightings.Add(new HighlightingInfo(throwStatement.DocumentRange, highlighting, null));
            }
        }

        private static bool RequiresInnerExceptionPassing(ThrowStatementModel throwStatementModel)
        {
            if (!throwStatementModel.IsDirectExceptionInstantiation)
                return false;

            if (throwStatementModel.IsRethrow)
                return false;

            var outerCatchClause = throwStatementModel.FindOuterCatchClause();
            if (outerCatchClause == null)
                return false;

            if (!outerCatchClause.IsExceptionTypeSpecified)
                return true;

            if (!outerCatchClause.HasVariable)
                return true;

            return !throwStatementModel.IsInnerExceptionPassed(outerCatchClause.Variable.VariableName.Name);
        }
    }
}