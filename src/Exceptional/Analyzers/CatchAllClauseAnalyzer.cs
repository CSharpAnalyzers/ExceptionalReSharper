using JetBrains.ReSharper.Daemon;
using JetBrains.ReSharper.Feature.Services.Daemon;
using ReSharper.Exceptional.Highlightings;
using ReSharper.Exceptional.Models;

namespace ReSharper.Exceptional.Analyzers
{
    /// <summary>Analyzes a catch clause and checks if it is not catch-all clause.</summary>
    internal class CatchAllClauseAnalyzer : AnalyzerBase
    {
        /// <summary>Performs analyze of <paramref name="catchClause"/>.</summary>
        /// <param name="catchClause">Catch clause to analyze.</param>
        public override void Visit(CatchClauseModel catchClause)
        {
            if (catchClause.IsCatchAll)
                ServiceLocator.StageProcess.AddHighlighting(new CatchAllClauseHighlighting(), catchClause.DocumentRange);
        }
    }
}