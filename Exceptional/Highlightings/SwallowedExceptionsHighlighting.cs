using System;
using JetBrains.ReSharper.Daemon;

using ReSharper.Exceptional.Models;

namespace ReSharper.Exceptional.Highlightings
{
    [StaticSeverityHighlighting(Severity.WARNING, Constants.HighlightingTitle)]
    public class SwallowedExceptionsHighlighting : HighlightingBase
    {
        /// <summary>Initializes a new instance of the <see cref="SwallowedExceptionsHighlighting"/> class. </summary>
        /// <param name="catchClause">The catch clause. </param>
        internal SwallowedExceptionsHighlighting(CatchClauseModel catchClause)
        {
            CatchClause = catchClause;
        }

        /// <summary>Gets the catch clause. </summary>
        internal CatchClauseModel CatchClause { get; private set; }

        /// <summary>Gets the message which is shown in the editor. </summary>
        protected override string Message
        {
            get { return String.Format(Resources.HighlightSwallowingExceptions); }
        }
    }
}