using System;
using JetBrains.ReSharper.Daemon;
using JetBrains.ReSharper.Psi.CSharp;
using ReSharper.Exceptional;
using ReSharper.Exceptional.Highlightings;
using ReSharper.Exceptional.Models;

#if R9 || R10
using JetBrains.ReSharper.Feature.Services.Daemon;
#endif

[assembly: RegisterConfigurableSeverity(ExceptionNotThrownOptionalHighlighting.Id, Constants.CompoundName, HighlightingGroupIds.BestPractice,
    "Exceptional.ExceptionNotThrownOptional",
    "Exceptional.ExceptionNotThrownOptional",
    Severity.HINT
#if !R2016_3 && !R2017_1
    ,
    false
#endif
    )]

namespace ReSharper.Exceptional.Highlightings
{
    [ConfigurableSeverityHighlighting(Id, CSharpLanguage.Name)]
    public class ExceptionNotThrownOptionalHighlighting : HighlightingBase
    {
        public const string Id = "ExceptionNotThrownOptional";

        /// <summary>Initializes a new instance of the <see cref="ExceptionNotThrownOptionalHighlighting"/> class. </summary>
        /// <param name="exceptionDocumentation">The exception documentation. </param>
        internal ExceptionNotThrownOptionalHighlighting(ExceptionDocCommentModel exceptionDocumentation)
        {
            ExceptionDocumentation = exceptionDocumentation;
        }

        /// <summary>Gets the exception documentation. </summary>
        internal ExceptionDocCommentModel ExceptionDocumentation { get; private set; }

        /// <summary>Gets the message which is shown in the editor. </summary>
        protected override string Message
        {
            get
            {
                return Constants.OptionalPrefix + String.Format(
                    Resources.HighlightNotThrownDocumentedExceptions, ExceptionDocumentation.ExceptionType.GetClrName().FullName);
            }
        }
    }
}