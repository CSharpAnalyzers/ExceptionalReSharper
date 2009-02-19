using System;
using JetBrains.ReSharper.Daemon;

namespace CodeGears.ReSharper.Exceptional.Highlightings
{
    [StaticSeverityHighlighting(Severity.WARNING)]
    public class CatchAllClauseHighlighting : IHighlighting
    {
        private const string MESSAGE = "Swallowing exceptions in catch-all clause is not recommended.";

        public CatchAllClauseHighlighting() { }

        public string ToolTip
        {
            get { return GetMessage(); }
        }

        public string ErrorStripeToolTip
        {
            get { return GetMessage(); }
        }

        private string GetMessage()
        {
            return String.Format(MESSAGE);
        }

        public int NavigationOffsetPatch
        {
            get { return 0; }
        }
    }
}