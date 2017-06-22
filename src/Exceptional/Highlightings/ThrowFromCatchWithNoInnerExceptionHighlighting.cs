using System;
using JetBrains.ReSharper.Daemon;
using JetBrains.ReSharper.Psi.CSharp;
using ReSharper.Exceptional;
using ReSharper.Exceptional.Highlightings;
using ReSharper.Exceptional.Models;
using ReSharper.Exceptional.Models.ExceptionsOrigins;

#if R9 || R10
using JetBrains.ReSharper.Feature.Services.Daemon;
#endif

[assembly: RegisterConfigurableSeverity(ThrowFromCatchWithNoInnerExceptionHighlighting.Id, Constants.CompoundName, HighlightingGroupIds.BestPractice,
    "Exceptional.ThrowFromCatchWithNoInnerException",
    "Exceptional.ThrowFromCatchWithNoInnerException",
    Severity.WARNING
#if !R2016_3 && !R2017_1
    ,
    false
#endif
    )]

namespace ReSharper.Exceptional.Highlightings
{
    [ConfigurableSeverityHighlighting(Id, CSharpLanguage.Name)]
    public class ThrowFromCatchWithNoInnerExceptionHighlighting : HighlightingBase
    {
        public const string Id = "ThrowFromCatchWithNoInnerException";

        /// <summary>Initializes a new instance of the <see cref="ThrowFromCatchWithNoInnerExceptionHighlighting"/> class. </summary>
        /// <param name="throwStatement">The throw statement. </param>
        internal ThrowFromCatchWithNoInnerExceptionHighlighting(ThrowStatementModel throwStatement)
        {
            ThrowStatement = throwStatement;
        }

        /// <summary>Gets the throw statement. </summary>
        internal ThrowStatementModel ThrowStatement { get; private set; }

        /// <summary>Gets the message which is shown in the editor. </summary>
        protected override string Message
        {
            get { return Resources.HighlightThrowingFromCatchWithoutInnerException; }
        }
    }
}