using System;
using JetBrains.ReSharper.Daemon;

namespace ReSharper.Exceptional.Highlightings
{
    [StaticSeverityHighlighting(Severity.SUGGESTION, Category.Title)]
    public class CatchAllClauseHighlighting : HighlightingBase
    {
        protected override string Message
        {
            get { return String.Format(Resources.HighlightCatchAllClauses); }
        }
    }
}