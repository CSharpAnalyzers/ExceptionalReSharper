// Copyright (c) 2009-2010 Cofinite Solutions. All rights reserved.
using System;
using CodeGears.ReSharper.Exceptional.Model;

using JetBrains.ReSharper.Daemon;

namespace CodeGears.ReSharper.Exceptional.Highlightings
{
	[StaticSeverityHighlighting(Severity.WARNING, Category.Title)]
    public class ExceptionNotDocumentedHighlighting : HighlightingBase
    {
        internal ThrownExceptionModel ThrownExceptionModel { get; private set; }

        internal ExceptionNotDocumentedHighlighting(ThrownExceptionModel thrownExceptionModel)
        {
            ThrownExceptionModel = thrownExceptionModel;
        }

        protected override string Message
        {
            get
            {
                var exceptionType = this.ThrownExceptionModel.ExceptionType;
                var exceptionTypeName = exceptionType != null ? exceptionType.GetClrName().ShortName : "[NOT RESOLVED]";
                return String.Format(Resources.HighLightNotDocumentedExceptions, exceptionTypeName);
            }
        }
    }
}