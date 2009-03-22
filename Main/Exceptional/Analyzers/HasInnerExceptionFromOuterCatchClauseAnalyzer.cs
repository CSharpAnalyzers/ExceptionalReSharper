/// <copyright file="HasInnerExceptionFromOuterCatchClauseAnalyzer.cs" manufacturer="CodeGears">
///   Copyright (c) CodeGears. All rights reserved.
/// </copyright>

using CodeGears.ReSharper.Exceptional.Highlightings;
using CodeGears.ReSharper.Exceptional.Model;

namespace CodeGears.ReSharper.Exceptional.Analyzers
{
    /// <summary>Analyzes throw statements and checks if the contain inner exception when thrown from inside a catch clause.</summary>
    internal class HasInnerExceptionFromOuterCatchClauseAnalyzer : AnalyzerBase
    {
        public override void Visit(ThrowStatementModel throwStatementModel)
        {
            if (throwStatementModel == null) return;
            if (AnalyzeIfHasInnerException(throwStatementModel)) return;

            this.Process.AddHighlighting(new ThrowFromCatchWithNoInnerExceptionHighlighting(throwStatementModel));
        }

        private static bool AnalyzeIfHasInnerException(ThrowStatementModel throwStatementModel)
        {
            var outerCatchClause = throwStatementModel.FindOuterCatchClause();
            if (outerCatchClause == null) return true;

            outerCatchClause.IsRethrown = throwStatementModel.IsRethrow;

            if (outerCatchClause.HasExceptionType == false || throwStatementModel.IsRethrow)
            {
                //There is no variable decaration or there is no exception creation
                return throwStatementModel.IsRethrow;
            }

            if(outerCatchClause.HasVariable == false)
            {
                return false;
            }

            return throwStatementModel.HasInnerException(outerCatchClause.VariableModel.VariableName.Name);
        }
    }
}