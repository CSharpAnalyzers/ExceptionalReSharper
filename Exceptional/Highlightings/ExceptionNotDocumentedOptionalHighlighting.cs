using System;
using JetBrains.ReSharper.Daemon;
using ReSharper.Exceptional.Models;

namespace ReSharper.Exceptional.Highlightings
{
    [StaticSeverityHighlighting(Severity.HINT, Constants.HighlightingTitle)]
    public class ExceptionNotDocumentedOptionalHighlighting : HighlightingBase
    {
        /// <summary>Initializes a new instance of the <see cref="ExceptionNotDocumentedOptionalHighlighting"/> class. </summary>
        /// <param name="thrownException">The thrown exception. </param>
        internal ExceptionNotDocumentedOptionalHighlighting(ThrownExceptionModel thrownException)
        {
            ThrownException = thrownException;
        }

        /// <summary>Gets the thrown exception. </summary>
        internal ThrownExceptionModel ThrownException { get; private set; }

        /// <summary>Gets the message which is shown in the editor. </summary>
        protected override string Message
        {
            get
            {
                var exceptionType = ThrownException.ExceptionType;
                var exceptionTypeName = exceptionType != null ? exceptionType.GetClrName().ShortName : "[NOT RESOLVED]";
                return Constants.OptionalPrefix + String.Format(Resources.HighlightNotDocumentedExceptions, exceptionTypeName);
            }
        }
    }
}