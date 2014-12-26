using System;
using JetBrains.ReSharper.Daemon;
using JetBrains.ReSharper.Psi.CSharp;
using ReSharper.Exceptional;
using ReSharper.Exceptional.Highlightings;

#if R9
using JetBrains.ReSharper.Feature.Services.Daemon;
#endif

[assembly: RegisterConfigurableSeverity(ThrowingSystemExceptionHighlighting.Id, Constants.CompoundName, HighlightingGroupIds.BestPractice,
    "Exceptional.ThrowingSystemException",
    "Exceptional.ThrowingSystemException",
    Severity.SUGGESTION,
    false)]

namespace ReSharper.Exceptional.Highlightings
{
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