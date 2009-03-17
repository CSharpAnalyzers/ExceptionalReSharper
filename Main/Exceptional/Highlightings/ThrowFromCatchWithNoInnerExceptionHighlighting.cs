/// <copyright file="ThrowFromCatchWithNoInnerExceptionHighlighting.cs" manufacturer="CodeGears">
///   Copyright (c) CodeGears. All rights reserved.
/// </copyright>

using CodeGears.ReSharper.Exceptional.Model;
using JetBrains.DocumentModel;
using JetBrains.ReSharper.Daemon;
using JetBrains.ReSharper.Daemon.CSharp.Stages;

namespace CodeGears.ReSharper.Exceptional.Highlightings
{
    [StaticSeverityHighlighting(Severity.WARNING)]
    public class ThrowFromCatchWithNoInnerExceptionHighlighting : CSharpHighlightingBase, IHighlighting
    {
        internal ThrowStatementModel ThrowStatementModel { get; private set; }

        internal ThrowFromCatchWithNoInnerExceptionHighlighting(ThrowStatementModel throwStatementModel)
        {
            ThrowStatementModel = throwStatementModel;
        }

        public override bool IsValid()
        {
            return true;
        }

        public override DocumentRange Range
        {
            get { return this.ThrowStatementModel.DocumentRange; }
        }

        public string ToolTip
        {
            get { return Message; }
        }

        public string ErrorStripeToolTip
        {
            get { return Message; }
        }

        private string Message
        {
            get { return Resources.HighLightThrowingFromCatchWithoutInnerException; }
        }

        public int NavigationOffsetPatch
        {
            get { return 0; }
        }
    }
}