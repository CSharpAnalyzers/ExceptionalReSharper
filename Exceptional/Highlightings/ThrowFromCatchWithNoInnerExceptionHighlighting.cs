using JetBrains.ReSharper.Daemon;

using ReSharper.Exceptional.Models;

namespace ReSharper.Exceptional.Highlightings
{
    [StaticSeverityHighlighting(Severity.WARNING, Constants.HighlightingTitle)]
    public class ThrowFromCatchWithNoInnerExceptionHighlighting : HighlightingBase
    {
        /// <summary>Initializes a new instance of the <see cref="ThrowFromCatchWithNoInnerExceptionHighlighting"/> class. </summary>
        /// <param name="throwStatement">The throw statement. </param>
        internal ThrowFromCatchWithNoInnerExceptionHighlighting(ThrowStatementModel throwStatement)
        {
            ThrowStatement = throwStatement;
        }

        /// <summary>Gets the throw statement. </summary>
        internal ThrowStatementModel ThrowStatement { get; private set; }

        /// <summary>Gets the message which is shown in the editor. </summary>
        protected override string Message
        {
            get { return Resources.HighlightThrowingFromCatchWithoutInnerException; }
        }
    }
}