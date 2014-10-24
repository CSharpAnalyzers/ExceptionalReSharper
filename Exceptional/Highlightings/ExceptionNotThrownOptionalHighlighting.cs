using System;
using JetBrains.ReSharper.Daemon;
using ReSharper.Exceptional.Models;

namespace ReSharper.Exceptional.Highlightings
{
    [StaticSeverityHighlighting(Severity.HINT, Constants.HighlightingTitle)]
    public class ExceptionNotThrownOptionalHighlighting : HighlightingBase
    {
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
                    Resources.HighlightNotThrownDocumentedExceptions, ExceptionDocumentation.ExceptionType.GetClrName().ShortName);
            }
        }
    }
}