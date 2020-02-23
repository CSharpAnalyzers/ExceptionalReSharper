using System;
using JetBrains.ReSharper.Psi.CSharp;
using ReSharper.Exceptional;
using ReSharper.Exceptional.Highlightings;
using ReSharper.Exceptional.Models;
using JetBrains.ReSharper.Feature.Services.Daemon;

[assembly: RegisterConfigurableSeverity(ExceptionNotDocumentedOptionalHighlighting.Id, Constants.CompoundName, HighlightingGroupIds.BestPractice,
    "Exceptional.ExceptionNotDocumentedOptional",
    "Exceptional.ExceptionNotDocumentedOptional",
    Severity.HINT
    )]



namespace ReSharper.Exceptional.Highlightings
{
    [ConfigurableSeverityHighlighting(Id, CSharpLanguage.Name)]
    public class ExceptionNotDocumentedOptionalHighlighting : HighlightingBase
    {
        public const string Id = "ExceptionNotDocumentedOptional";

        /// <summary>Initializes a new instance of the <see cref="ExceptionNotDocumentedOptionalHighlighting"/> class. </summary>
        /// <param name="thrownException">The thrown exception. </param>
        internal ExceptionNotDocumentedOptionalHighlighting(ThrownExceptionModel thrownException)
        {
            ThrownException = thrownException;
        }

        /// <summary>Gets the thrown exception. </summary>
        internal ThrownExceptionModel ThrownException { get; private set; }

        /// <summary>Gets the message which is shown in the editor. </summary>
        protected override string Message
        {
            get
            {
                var exceptionType = ThrownException.ExceptionType;
                var exceptionTypeName = exceptionType != null ? exceptionType.GetClrName().FullName : "[NOT RESOLVED]";
                return Constants.OptionalPrefix + String.Format(Resources.HighlightNotDocumentedExceptions, exceptionTypeName);
            }
        }
    }
}