using System;
using JetBrains.ReSharper.Daemon;
using ReSharper.Exceptional.Models;

namespace ReSharper.Exceptional.Highlightings
{
	[StaticSeverityHighlighting(Severity.HINT, Constants.HighlightingTitle)]
    public class ExceptionNotDocumentedOptionalHighlighting : HighlightingBase
    {
        internal ThrownExceptionModel ThrownExceptionModel { get; private set; }

        internal ExceptionNotDocumentedOptionalHighlighting(ThrownExceptionModel thrownExceptionModel)
        {
            ThrownExceptionModel = thrownExceptionModel;
        }

        protected override string Message
        {
            get
            {
                var exceptionType = ThrownExceptionModel.ExceptionType;
                var exceptionTypeName = exceptionType != null ? exceptionType.GetClrName().ShortName : "[NOT RESOLVED]";
                return String.Format(Resources.HighlightNotDocumentedOptionalExceptions, exceptionTypeName);
            }
        }
    }
}