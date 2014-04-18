// Copyright (c) 2009-2010 Cofinite Solutions. All rights reserved.
using CodeGears.ReSharper.Exceptional.Highlightings;
using CodeGears.ReSharper.Exceptional.Model;
using JetBrains.ReSharper.Daemon;

namespace CodeGears.ReSharper.Exceptional.Analyzers
{
    /// <summary>Analyzes a catch clause and checks if it is not catch-all clause.</summary>
    internal class CatchAllClauseAnalyzer : AnalyzerBase
    {
        public CatchAllClauseAnalyzer(ExceptionalDaemonStageProcess process) 
            : base(process) { }

        public override void Visit(CatchClauseModel catchClauseModel)
        {
            if (catchClauseModel == null) return;
            if (catchClauseModel.IsCatchAll == false) return;

            Process.Hightlightings.Add(new HighlightingInfo(catchClauseModel.DocumentRange, new CatchAllClauseHighlighting(), null, null));
        }
    }
}