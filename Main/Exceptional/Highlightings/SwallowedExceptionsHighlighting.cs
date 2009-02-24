using System;
using JetBrains.ReSharper.Daemon;

namespace CodeGears.ReSharper.Exceptional.Highlightings
{
    [StaticSeverityHighlighting(Severity.WARNING)]
    public class SwallowedExceptionsHighlighting : IHighlighting
    {
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
            get { return String.Format(Resources.HighLightSwallowingExceptions); }
        }

        public int NavigationOffsetPatch
        {
            get { return 0; }
        }
    }
}