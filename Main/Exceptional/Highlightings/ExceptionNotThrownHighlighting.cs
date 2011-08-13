// Copyright (c) 2009-2010 Cofinite Solutions. All rights reserved.
using System;
using CodeGears.ReSharper.Exceptional.Model;

using JetBrains.ReSharper.Daemon;

namespace CodeGears.ReSharper.Exceptional.Highlightings
{
	[StaticSeverityHighlighting(Severity.WARNING, Category.Title)]
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
                return String.Format(Resources.HighLightNotThrownDocumentedExceptions,
                                     this.ExceptionDocumentationModel.ExceptionType.GetClrName().ShortName);
            }
        }
    }
}