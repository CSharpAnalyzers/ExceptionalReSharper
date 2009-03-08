using CodeGears.ReSharper.Exceptional.Highlightings;
using CodeGears.ReSharper.Exceptional.Model;

namespace CodeGears.ReSharper.Exceptional.Analyzers
{
    /// <summary>Analyzes throw statements and checks that exceptions thrown outside are documented.</summary>
    internal class IsThrownExceptionDocumentedAnalyzer : AnalyzerBase
    {
        public override void Visit(ThrowStatementModel throwStatementModel)
        {
            if (throwStatementModel == null) return;
            if (throwStatementModel.IsCatched) return;
            if (throwStatementModel.IsDocumented) return;

            this.Process.AddHighlighting(throwStatementModel.DocumentRange, new ExceptionNotDocumentedHighlighting(throwStatementModel));
        }
    }
}