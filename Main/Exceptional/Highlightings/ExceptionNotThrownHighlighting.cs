using System;
using JetBrains.ReSharper.Daemon;

namespace CodeGears.ReSharper.Exceptional.Highlightings
{
    [StaticSeverityHighlighting(Severity.WARNING)]
    public class ExceptionNotThrownHighlighting : IHighlighting
    {
        private readonly string _excetionType;

        public ExceptionNotThrownHighlighting(string excetionType)
        {
            _excetionType = excetionType;
        }

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
            get { return String.Format(Resources.HighLightNotThrownDocumentedExceptions, _excetionType); }
        }

        public int NavigationOffsetPatch
        {
            get { return 0; }
        }
    }
}