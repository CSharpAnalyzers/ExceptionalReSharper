using System;
using JetBrains.ReSharper.Daemon;

namespace ReSharper.Exceptional.Highlightings
{
    [StaticSeverityHighlighting(Severity.SUGGESTION, Constants.HighlightingTitle)]
    public class CatchAllClauseHighlighting : HighlightingBase
    {
        protected override string Message
        {
            get { return String.Format(Resources.HighlightCatchAllClauses); }
        }
    }
}