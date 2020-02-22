using JetBrains.ReSharper.Daemon;
using JetBrains.ReSharper.Psi.CSharp;
using ReSharper.Exceptional;
using ReSharper.Exceptional.Highlightings;
using ReSharper.Exceptional.Models;

#if R9 || R10
using JetBrains.ReSharper.Feature.Services.Daemon;
#endif

[assembly: RegisterConfigurableSeverity(EventExceptionNotDocumentedHighlighting.Id, Constants.CompoundName, HighlightingGroupIds.BestPractice,
    "Exceptional.EventExceptionNotDocumented",
    "Exceptional.EventExceptionNotDocumented",
    Severity.SUGGESTION
#if !R2016_3 && !R2017_1
    ,
    false
#endif
    )]


namespace ReSharper.Exceptional.Highlightings
{
    [ConfigurableSeverityHighlighting(Id, CSharpLanguage.Name)]
    public class EventExceptionNotDocumentedHighlighting : ExceptionNotDocumentedHighlighting
    {
        public new const string Id = "EventExceptionNotDocumented";

        /// <summary>Initializes a new instance of the <see cref="EventExceptionNotDocumentedHighlighting"/> class. </summary>
        /// <param name="thrownException">The thrown exception. </param>
        internal EventExceptionNotDocumentedHighlighting(ThrownExceptionModel thrownException)
            : base(thrownException)
        {
        }

        /// <summary>Gets the message which is shown in the editor. </summary>
        protected override string Message
        {
            get { return Resources.HighlightEventNotDocumentedExceptions; }
        }
    }
}