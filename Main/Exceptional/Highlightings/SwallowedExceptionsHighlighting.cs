// Copyright (c) 2009-2010 Cofinite Solutions. All rights reserved.
using System;
using CodeGears.ReSharper.Exceptional.Model;

using JetBrains.ReSharper.Daemon;

namespace CodeGears.ReSharper.Exceptional.Highlightings
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
            get { return String.Format(Resources.HighLightSwallowingExceptions); }
        }
    }
}