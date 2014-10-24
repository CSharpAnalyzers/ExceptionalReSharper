using System;
using JetBrains.ReSharper.Daemon;
using ReSharper.Exceptional.Models;

namespace ReSharper.Exceptional.Highlightings
{
    [StaticSeverityHighlighting(Severity.WARNING, Constants.HighlightingTitle)]
    public class ExceptionNotDocumentedHighlighting : ExceptionNotDocumentedOptionalHighlighting
    {
        /// <summary>Initializes a new instance of the <see cref="ExceptionNotDocumentedHighlighting"/> class. </summary>
        /// <param name="thrownException">The thrown exception. </param>
        internal ExceptionNotDocumentedHighlighting(ThrownExceptionModel thrownException)
            : base(thrownException)
        {
        }

        /// <summary>Gets the message which is shown in the editor. </summary>
        protected override string Message
        {
            get
            {
                var exceptionType = ThrownException.ExceptionType;
                var exceptionTypeName = exceptionType != null ? exceptionType.GetClrName().ShortName : "[NOT RESOLVED]";
                return String.Format(Resources.HighlightNotDocumentedExceptions, exceptionTypeName);
            }
        }
    }
}