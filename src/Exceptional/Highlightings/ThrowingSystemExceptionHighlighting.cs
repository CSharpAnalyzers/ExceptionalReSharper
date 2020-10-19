using System;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi.CSharp;

namespace ReSharper.Exceptional.Highlightings
{
    [RegisterConfigurableSeverity(Id, Constants.CompoundName, HighlightingGroupIds.BestPractice,
        "Exceptional.ThrowingSystemException",
        "Exceptional.ThrowingSystemException",
        Severity.SUGGESTION
    )]
    [ConfigurableSeverityHighlighting(Id, CSharpLanguage.Name)]
    public class ThrowingSystemExceptionHighlighting : HighlightingBase
    {
        public const string Id = "ThrowingSystemException";

        /// <summary>Gets the message which is shown in the editor. </summary>
        protected override string Message
        {
            get { return String.Format(Resources.HighlightThrowingSystemException); }
        }
    }
}