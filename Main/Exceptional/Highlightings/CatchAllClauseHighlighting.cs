using System;
using CodeGears.ReSharper.Exceptional.Model;
using JetBrains.ReSharper.Daemon;

namespace CodeGears.ReSharper.Exceptional.Highlightings
{
    [StaticSeverityHighlighting(Severity.WARNING)]
    public class CatchAllClauseHighlighting : IHighlighting
    {
        internal CatchClauseModel CatchClauseModel { get; set; }

        internal CatchAllClauseHighlighting(CatchClauseModel catchClauseModel)
        {
            CatchClauseModel = catchClauseModel;
        }

        public string ToolTip
        {
            get { return Message; }
        }

        public string ErrorStripeToolTip
        {
            get { return Message; }
        }

        private static string Message
        {
            get { return String.Format(Resources.HighLiightCatchAllClauses); }
        }

        public int NavigationOffsetPatch
        {
            get { return 0; }
        }
    }
}