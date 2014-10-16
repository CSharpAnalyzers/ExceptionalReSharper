using System;
using JetBrains.ReSharper.Daemon;

using ReSharper.Exceptional.Models;

namespace ReSharper.Exceptional.Highlightings
{
	[StaticSeverityHighlighting(Severity.WARNING, Category.Title)]
    public class SwallowedExceptionsHighlighting : HighlightingBase
    {
        private CatchClauseModel CatchClauseModel { get; set; }

        internal SwallowedExceptionsHighlighting(CatchClauseModel catchClauseModel)
        {
            CatchClauseModel = catchClauseModel;
        }

        protected override string Message
        {
            get { return String.Format(Resources.HighlightSwallowingExceptions); }
        }
    }
}