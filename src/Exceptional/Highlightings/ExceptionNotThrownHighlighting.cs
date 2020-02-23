using System;
using JetBrains.ReSharper.Psi.CSharp;
using ReSharper.Exceptional;
using ReSharper.Exceptional.Highlightings;
using ReSharper.Exceptional.Models;
using JetBrains.ReSharper.Feature.Services.Daemon;

[assembly: RegisterConfigurableSeverity(ExceptionNotThrownHighlighting.Id, Constants.CompoundName, HighlightingGroupIds.BestPractice,
    "Exceptional.ExceptionNotThrown",
    "Exceptional.ExceptionNotThrown",
    Severity.WARNING
    )]

namespace ReSharper.Exceptional.Highlightings
{
    [ConfigurableSeverityHighlighting(Id, CSharpLanguage.Name)]
    public class ExceptionNotThrownHighlighting : ExceptionNotThrownOptionalHighlighting
    {
        public new const string Id = "ExceptionNotThrown";

        /// <summary>Initializes a new instance of the <see cref="ExceptionNotThrownHighlighting"/> class. </summary>
        /// <param name="exceptionDocumentation">The exception documentation. </param>
        internal ExceptionNotThrownHighlighting(ExceptionDocCommentModel exceptionDocumentation)
            : base(exceptionDocumentation)
        {
        }

        /// <summary>Gets the message which is shown in the editor. </summary>
        protected override string Message
        {
            get
            {
                return String.Format(Resources.HighlightNotThrownDocumentedExceptions, ExceptionDocumentation.ExceptionType.GetClrName().FullName);
            }
        }
    }
}