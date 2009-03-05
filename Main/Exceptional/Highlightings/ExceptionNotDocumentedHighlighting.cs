using System;
using CodeGears.ReSharper.Exceptional.Model;
using JetBrains.ReSharper.Daemon;

namespace CodeGears.ReSharper.Exceptional.Highlightings
{
    [StaticSeverityHighlighting(Severity.WARNING)]
    public class ExceptionNotDocumentedHighlighting : IHighlighting
    {
        public ThrowStatementModel ThrowStatementModel { get; set; }

        public ExceptionNotDocumentedHighlighting(ThrowStatementModel throwStatement)
        {
            ThrowStatementModel = throwStatement;
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
            get
            {
                if (this.ThrowStatementModel.ExceptionType == null)
                    throw new InvalidOperationException("The given exception was null.");

                return String.Format(Resources.HighLightNotDocumentedExceptions, this.ThrowStatementModel.ExceptionType.GetCLRName());
            }
        }

        public int NavigationOffsetPatch
        {
            get { return 0; }
        }
    }
}