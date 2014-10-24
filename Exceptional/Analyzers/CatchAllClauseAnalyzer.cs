using JetBrains.ReSharper.Daemon;
using ReSharper.Exceptional.Highlightings;
using ReSharper.Exceptional.Models;
using ReSharper.Exceptional.Settings;

namespace ReSharper.Exceptional.Analyzers
{
    /// <summary>Analyzes a catch clause and checks if it is not catch-all clause.</summary>
    internal class CatchAllClauseAnalyzer : AnalyzerBase
    {
        /// <summary>Initializes a new instance of the <see cref="CatchAllClauseAnalyzer"/> class. </summary>
        /// <param name="process">The process. </param>
        /// <param name="settings">The settings. </param>
        public CatchAllClauseAnalyzer(ExceptionalDaemonStageProcess process, ExceptionalSettings settings)
            : base(process, settings) { }

        /// <summary>Performs analyze of <paramref name="catchClause"/>.</summary>
        /// <param name="catchClause">Catch clause to analyze.</param>
        public override void Visit(CatchClauseModel catchClause)
        {
            if (catchClause.IsCatchAll)
                Process.Hightlightings.Add(new HighlightingInfo(catchClause.DocumentRange, new CatchAllClauseHighlighting(), null));
        }
    }
}