/// <copyright>Copyright (c) 2009 CodeGears.net All rights reserved.</copyright>

using CodeGears.ReSharper.Exceptional.Model;
using JetBrains.DocumentModel;
using JetBrains.ReSharper.Daemon;

namespace CodeGears.ReSharper.Exceptional.Highlightings
{
    [StaticSeverityHighlighting(Severity.WARNING)]
    public class ThrowFromCatchWithNoInnerExceptionHighlighting : HighlightingBase
    {
        internal ThrowStatementModel ThrowStatementModel { get; private set; }

        internal ThrowFromCatchWithNoInnerExceptionHighlighting(ThrowStatementModel throwStatementModel)
        {
            ThrowStatementModel = throwStatementModel;
        }

        public override DocumentRange Range
        {
            get { return this.ThrowStatementModel.DocumentRange; }
        }

        protected override string Message
        {
            get { return Resources.HighLightThrowingFromCatchWithoutInnerException; }
        }
    }
}