using System;
using JetBrains.ReSharper.Daemon;

using ReSharper.Exceptional.Models;

namespace ReSharper.Exceptional.Highlightings
{
	[StaticSeverityHighlighting(Severity.WARNING, Constants.HighlightingTitle)]
    public class ExceptionNotThrownHighlighting : HighlightingBase
    {
        internal ExceptionDocCommentModel ExceptionDocumentationModel { get; private set; }

        internal ExceptionNotThrownHighlighting(ExceptionDocCommentModel exceptionDocumentationModel)
        {
            ExceptionDocumentationModel = exceptionDocumentationModel;
        }

        protected override string Message
        {
            get
            {
                return String.Format(Resources.HighlightNotThrownDocumentedExceptions, 
                    ExceptionDocumentationModel.ExceptionType.GetClrName().ShortName);
            }
        }
    }
}