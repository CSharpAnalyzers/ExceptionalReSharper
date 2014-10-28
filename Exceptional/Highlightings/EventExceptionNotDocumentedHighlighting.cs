using JetBrains.ReSharper.Daemon;
using ReSharper.Exceptional.Models;

namespace ReSharper.Exceptional.Highlightings
{
    [StaticSeverityHighlighting(Severity.SUGGESTION, Constants.HighlightingTitle)]
    public class EventExceptionNotDocumentedHighlighting : ExceptionNotDocumentedHighlighting
    {
        /// <summary>Initializes a new instance of the <see cref="EventExceptionNotDocumentedHighlighting"/> class. </summary>
        /// <param name="thrownException">The thrown exception. </param>
        internal EventExceptionNotDocumentedHighlighting(ThrownExceptionModel thrownException)
            : base(thrownException)
        {
        }

        /// <summary>Gets the message which is shown in the editor. </summary>
        protected override string Message
        {
            get { return Resources.HighlightEventNotDocumentedExceptions; }
        }
    }
}