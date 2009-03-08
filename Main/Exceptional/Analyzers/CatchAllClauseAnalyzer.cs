using CodeGears.ReSharper.Exceptional.Highlightings;
using CodeGears.ReSharper.Exceptional.Model;

namespace CodeGears.ReSharper.Exceptional.Analyzers
{
    /// <summary>Analyzes a catch clause and checks if it is not catch-all clause.</summary>
    internal class CatchAllClauseAnalyzer : AnalyzerBase
    {
        public override void Visit(CatchClauseModel catchClauseModel)
        {
            if (catchClauseModel == null) return;
            if (catchClauseModel.IsCatchAll == false) return;

            this.Process.AddHighlighting(catchClauseModel.DocumentRange, new CatchAllClauseHighlighting(catchClauseModel));
        }
    }
}