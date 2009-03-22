/// <copyright>Copyright (c) 2009 CodeGears.net All rights reserved.</copyright>

using CodeGears.ReSharper.Exceptional.Highlightings;
using CodeGears.ReSharper.Exceptional.Model;

namespace CodeGears.ReSharper.Exceptional.Analyzers
{
    /// <summary>Analyzes throw statements and checks that exceptions thrown outside are documented.</summary>
    internal class IsThrownExceptionDocumentedAnalyzer : AnalyzerBase
    {
        public override void Visit(ThrownExceptionModel thrownExceptionModel)
        {
            if (thrownExceptionModel == null)
            {
                return;
            }
            if (thrownExceptionModel.AnalyzeUnit.IsPublicOrInternal == false)
            {
                return;
            }
            if (thrownExceptionModel.IsCatched)
            {
                return;
            }
            if (thrownExceptionModel.IsDocumented)
            {
                return;
            }

            this.Process.AddHighlighting(new ExceptionNotDocumentedHighlighting(thrownExceptionModel));
        }
    }
}