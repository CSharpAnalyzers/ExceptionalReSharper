using JetBrains.ReSharper.Daemon;

using ReSharper.Exceptional.Models;

namespace ReSharper.Exceptional.Highlightings
{
	[StaticSeverityHighlighting(Severity.WARNING, Constants.HighlightingTitle)]
    public class ThrowFromCatchWithNoInnerExceptionHighlighting : HighlightingBase
    {
        internal ThrowStatementModel ThrowStatementModel { get; private set; }

        internal ThrowFromCatchWithNoInnerExceptionHighlighting(ThrowStatementModel throwStatementModel)
        {
            ThrowStatementModel = throwStatementModel;
        }

        protected override string Message
        {
            get { return Resources.HighlightThrowingFromCatchWithoutInnerException; }
        }
    }
}