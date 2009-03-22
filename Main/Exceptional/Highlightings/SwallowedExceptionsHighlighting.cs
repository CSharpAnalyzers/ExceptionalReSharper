/// <copyright>Copyright (c) 2009 CodeGears.net All rights reserved.</copyright>

using System;
using CodeGears.ReSharper.Exceptional.Model;
using JetBrains.DocumentModel;
using JetBrains.ReSharper.Daemon;

namespace CodeGears.ReSharper.Exceptional.Highlightings
{
    [StaticSeverityHighlighting(Severity.WARNING)]
    public class SwallowedExceptionsHighlighting : HighlightingBase
    {
        private CatchClauseModel CatchClauseModel { get; set; }

        internal SwallowedExceptionsHighlighting(CatchClauseModel catchClauseModel)
        {
            CatchClauseModel = catchClauseModel;
        }

        public override DocumentRange Range
        {
            get { return this.CatchClauseModel.DocumentRange; }
        }

        protected override string Message
        {
            get { return String.Format(Resources.HighLightSwallowingExceptions); }
        }
    }
}