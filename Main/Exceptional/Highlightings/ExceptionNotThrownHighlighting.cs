using System;
using JetBrains.ReSharper.Daemon;

namespace CodeGears.ReSharper.Exceptional.Highlightings
{
    [StaticSeverityHighlighting(Severity.WARNING)]
    public class ExceptionNotThrownHighlighting : IHighlighting
    {
        private readonly string _excetionType;
        private const string MESSAGE = "The '{0}' exception is not thrown from documented method.";

        public ExceptionNotThrownHighlighting(string excetionType)
        {
            _excetionType = excetionType;
        }

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
            return String.Format(MESSAGE, _excetionType);
        }

        public int NavigationOffsetPatch
        {
            get { return 0; }
        }
    }
}