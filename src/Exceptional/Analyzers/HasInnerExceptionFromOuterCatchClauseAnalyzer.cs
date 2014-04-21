// Copyright (c) 2009-2010 Cofinite Solutions. All rights reserved.
using CodeGears.ReSharper.Exceptional.Highlightings;
using CodeGears.ReSharper.Exceptional.Model;

using JetBrains.ReSharper.Daemon;

namespace CodeGears.ReSharper.Exceptional.Analyzers
{
    /// <summary>Analyzes throw statements and checks if the contain inner exception when thrown from inside a catch clause.</summary>
    internal class HasInnerExceptionFromOuterCatchClauseAnalyzer : AnalyzerBase
    {
        public HasInnerExceptionFromOuterCatchClauseAnalyzer(ExceptionalDaemonStageProcess process) 
            : base(process) { }

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
                //There is no variable decaration or there is no exception creation
                return throwStatementModel.IsRethrow;
            }

            if (outerCatchClause.HasVariable == false) return false;

            return throwStatementModel.HasInnerException(outerCatchClause.VariableModel.VariableName.Name);
        }
    }
}