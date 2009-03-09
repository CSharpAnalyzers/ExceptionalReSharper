using System;
using CodeGears.ReSharper.Exceptional.Model;
using JetBrains.ReSharper.Daemon;

namespace CodeGears.ReSharper.Exceptional.Highlightings
{
    [StaticSeverityHighlighting(Severity.WARNING)]
    public class ExceptionNotDocumentedHighlighting : IHighlighting
    {
        internal ThrownExceptionModel ThrownExceptionModel { get; private set; }

        internal ExceptionNotDocumentedHighlighting(ThrownExceptionModel thrownExceptionModel)
        {
            ThrownExceptionModel = thrownExceptionModel;
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
                var exceptionType = this.ThrownExceptionModel.ExceptionType;
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