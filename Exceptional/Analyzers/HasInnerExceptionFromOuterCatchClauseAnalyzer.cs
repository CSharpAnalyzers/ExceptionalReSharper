using JetBrains.ReSharper.Daemon;
using ReSharper.Exceptional.Highlightings;
using ReSharper.Exceptional.Models;
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

        /// <summary>Performs analyze of throw <paramref name="throwStatementModel"/>.</summary>
        /// <param name="throwStatementModel">Throw statement model to analyze.</param>
        public override void Visit(ThrowStatementModel throwStatementModel)
        {
            if (throwStatementModel == null) return;
            if (AnalyzeIfHasInnerException(throwStatementModel)) return;

            Process.Hightlightings.Add(new HighlightingInfo(throwStatementModel.DocumentRange, new ThrowFromCatchWithNoInnerExceptionHighlighting(throwStatementModel), null, null));            
        }

        private static bool AnalyzeIfHasInnerException(ThrowStatementModel throwStatementModel)
        {
            var outerCatchClause = throwStatementModel.FindOuterCatchClause();
            if (outerCatchClause == null) return true;

            if (outerCatchClause.HasExceptionType == false || throwStatementModel.IsRethrow)
            {
                //There is no variable declaration or there is no exception creation
                return throwStatementModel.IsRethrow;
            }

            if (outerCatchClause.HasVariable == false) return false;

            return throwStatementModel.HasInnerException(outerCatchClause.Variable.VariableName.Name);
        }
    }
}