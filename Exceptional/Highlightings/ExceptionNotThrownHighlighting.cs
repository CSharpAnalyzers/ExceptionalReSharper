using System;
using JetBrains.ReSharper.Daemon;

using ReSharper.Exceptional.Models;

namespace ReSharper.Exceptional.Highlightings
{
    [StaticSeverityHighlighting(Severity.WARNING, Constants.HighlightingTitle)]
    public class ExceptionNotThrownHighlighting : ExceptionNotThrownOptionalHighlighting
    {
        /// <summary>Initializes a new instance of the <see cref="ExceptionNotThrownHighlighting"/> class. </summary>
        /// <param name="exceptionDocumentation">The exception documentation. </param>
        internal ExceptionNotThrownHighlighting(ExceptionDocCommentModel exceptionDocumentation)
            :base(exceptionDocumentation)
        {
        }

        /// <summary>Gets the message which is shown in the editor. </summary>
        protected override string Message
        {
            get
            {
                return String.Format(Resources.HighlightNotThrownDocumentedExceptions, ExceptionDocumentation.ExceptionType.GetClrName().ShortName);
            }
        }
    }
}