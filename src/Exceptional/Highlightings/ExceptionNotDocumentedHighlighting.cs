using System;
using JetBrains.ReSharper.Psi.CSharp;
using ReSharper.Exceptional;
using ReSharper.Exceptional.Highlightings;
using ReSharper.Exceptional.Models;

using JetBrains.ReSharper.Feature.Services.Daemon;


[assembly: RegisterConfigurableSeverity(ExceptionNotDocumentedHighlighting.Id, Constants.CompoundName, HighlightingGroupIds.BestPractice,
    "Exceptional.ExceptionNotDocumented",
    "Exceptional.ExceptionNotDocumented",
    Severity.WARNING
    )]


namespace ReSharper.Exceptional.Highlightings
{
    [ConfigurableSeverityHighlighting(Id, CSharpLanguage.Name)]
    public class ExceptionNotDocumentedHighlighting : ExceptionNotDocumentedOptionalHighlighting
    {
        public new const string Id = "ExceptionNotDocumented";

        /// <summary>Initializes a new instance of the <see cref="ExceptionNotDocumentedHighlighting"/> class. </summary>
        /// <param name="thrownException">The thrown exception. </param>
        internal ExceptionNotDocumentedHighlighting(ThrownExceptionModel thrownException)
            : base(thrownException)
        {
        }

        /// <summary>Gets the message which is shown in the editor. </summary>
        protected override string Message
        {
            get
            {
                var exceptionType = ThrownException.ExceptionType;
                var exceptionTypeName = exceptionType != null ? exceptionType.GetClrName().FullName : "[NOT RESOLVED]";
                return String.Format(Resources.HighlightNotDocumentedExceptions, exceptionTypeName);
            }
        }
    }
}