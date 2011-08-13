// Copyright (c) 2009-2010 Cofinite Solutions. All rights reserved.
using CodeGears.ReSharper.Exceptional.Model;

using JetBrains.ReSharper.Daemon;

namespace CodeGears.ReSharper.Exceptional.Highlightings
{
	[StaticSeverityHighlighting(Severity.WARNING, Category.Title)]
    public class ThrowFromCatchWithNoInnerExceptionHighlighting : HighlightingBase
    {
        internal ThrowStatementModel ThrowStatementModel { get; private set; }

        internal ThrowFromCatchWithNoInnerExceptionHighlighting(ThrowStatementModel throwStatementModel)
        {
            ThrowStatementModel = throwStatementModel;
        }

        protected override string Message
        {
            get { return Resources.HighLightThrowingFromCatchWithoutInnerException; }
        }
    }
}