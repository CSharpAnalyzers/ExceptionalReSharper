// Copyright (c) 2009-2010 Cofinite Solutions. All rights reserved.
using System;
using JetBrains.ReSharper.Daemon;

namespace CodeGears.ReSharper.Exceptional.Highlightings
{
    [StaticSeverityHighlighting(Severity.SUGGESTION)]
    public class CatchAllClauseHighlighting : HighlightingBase
    {
        protected override string Message
        {
            get { return String.Format(Resources.HighLightCatchAllClauses); }
        }
    }
}