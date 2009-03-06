using System;
using CodeGears.ReSharper.Exceptional.Model;
using JetBrains.ReSharper.Daemon;

namespace CodeGears.ReSharper.Exceptional.Highlightings
{
    [StaticSeverityHighlighting(Severity.WARNING)]
    public class ExceptionNotDocumentedHighlighting : IHighlighting
    {
        internal ThrowStatementModel ThrowStatementModel { get; set; }

        internal ExceptionNotDocumentedHighlighting(ThrowStatementModel throwStatement)
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
                var exceptionType = this.ThrowStatementModel.ExceptionType;
                var exceptionTupeName = exceptionType != null ? exceptionType.GetCLRName() : "[NOT RESOLVED]";
                return String.Format(Resources.HighLightNotDocumentedExceptions, exceptionTupeName);
            }
        }

        public int NavigationOffsetPatch
        {
            get { return 0; }
        }
    }
}