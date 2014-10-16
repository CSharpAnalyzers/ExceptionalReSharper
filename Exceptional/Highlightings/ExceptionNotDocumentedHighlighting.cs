using System;
using JetBrains.ReSharper.Daemon;
using ReSharper.Exceptional.Models;

namespace ReSharper.Exceptional.Highlightings
{
    [StaticSeverityHighlighting(Severity.WARNING, Category.Title)]
    public class ExceptionNotDocumentedHighlighting : ExceptionNotDocumentedOptionalHighlighting
    {
        internal ExceptionNotDocumentedHighlighting(ThrownExceptionModel thrownExceptionModel)
            : base(thrownExceptionModel)
        {
        }

        protected override string Message
        {
            get
            {
                var exceptionType = ThrownExceptionModel.ExceptionType;
                var exceptionTypeName = exceptionType != null ? exceptionType.GetClrName().ShortName : "[NOT RESOLVED]";
                return String.Format(Resources.HighlightNotDocumentedExceptions, exceptionTypeName);
            }
        }
    }
}