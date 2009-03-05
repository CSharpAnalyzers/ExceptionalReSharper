using System;
using CodeGears.ReSharper.Exceptional.Model;
using JetBrains.ReSharper.Daemon;

namespace CodeGears.ReSharper.Exceptional.Highlightings
{
    [StaticSeverityHighlighting(Severity.WARNING)]
    public class ExceptionNotThrownHighlighting : IHighlighting
    {
        public ExceptionDocumentationModel ExceptionDocumentationModel { get; set; }

        public ExceptionNotThrownHighlighting(ExceptionDocumentationModel exceptionDocumentationModel)
        {
            ExceptionDocumentationModel = exceptionDocumentationModel;
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
            get { return String.Format(Resources.HighLightNotThrownDocumentedExceptions, this.ExceptionDocumentationModel.ExceptionTypeName); }
        }

        public int NavigationOffsetPatch
        {
            get { return 0; }
        }
    }
}