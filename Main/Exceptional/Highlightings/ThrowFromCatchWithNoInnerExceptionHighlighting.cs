using System;
using JetBrains.ReSharper.Daemon;

namespace CodeGears.ReSharper.Exceptional.Highlightings
{
    [StaticSeverityHighlighting(Severity.WARNING)]
    public class ThrowFromCatchWithNoInnerExceptionHighlighting : IHighlighting
    {
        public string ToolTip
        {
            get { return Message; }
        }

        public string ErrorStripeToolTip
        {
            get { return Message; }
        }

        private string Message
        {
            get { return String.Format("While throwing from catch clause include substituted exception as inner exception. [Exceptional]"); }
        }

        public int NavigationOffsetPatch
        {
            get { return 0; }
        }
    }
}