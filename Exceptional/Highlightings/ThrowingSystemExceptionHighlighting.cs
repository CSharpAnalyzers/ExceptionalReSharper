using System;
using JetBrains.ReSharper.Daemon;

namespace ReSharper.Exceptional.Highlightings
{
    [StaticSeverityHighlighting(Severity.SUGGESTION, Constants.HighlightingTitle)]
    public class ThrowingSystemExceptionHighlighting : HighlightingBase
    {
        /// <summary>Gets the message which is shown in the editor. </summary>
        protected override string Message
        {
            get { return String.Format(Resources.HighlightThrowingSystemException); }
        }
    }
}