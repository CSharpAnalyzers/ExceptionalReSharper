using JetBrains.ReSharper.Daemon;
using ReSharper.Exceptional.Highlightings;
using ReSharper.Exceptional.Models.ExceptionsOrigins;
using ReSharper.Exceptional.Settings;

namespace ReSharper.Exceptional.Analyzers
{
    /// <summary>Analyzes throw statements and checks if the contain inner exception when thrown from inside a catch clause.</summary>
    internal class HasInnerExceptionFromOuterCatchClauseAnalyzer : AnalyzerBase
    {
        /// <summary>Initializes a new instance of the <see cref="AnalyzerBase"/> class. </summary>
        /// <param name="process">The process. </param>
        /// <param name="settings">The settings. </param>
        public HasInnerExceptionFromOuterCatchClauseAnalyzer(ExceptionalDaemonStageProcess process, ExceptionalSettings settings)
            : base(process, settings) { }

        /// <summary>Performs analyze of throw <paramref name="throwStatement"/>.</summary>
        /// <param name="throwStatement">Throw statement model to analyze.</param>
        public override void Visit(ThrowStatementModel throwStatement)
        {
            if (throwStatement != null && RequiresInnerExceptionPassing(throwStatement))
            {
                var highlighting = new ThrowFromCatchWithNoInnerExceptionHighlighting(throwStatement);
                Process.Hightlightings.Add(new HighlightingInfo(throwStatement.DocumentRange, highlighting, null));
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