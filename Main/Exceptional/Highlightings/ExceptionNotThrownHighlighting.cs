/// <copyright file="ExceptionNotThrownHighlighting.cs" manufacturer="CodeGears">
///   Copyright (c) CodeGears. All rights reserved.
/// </copyright>

using System;
using CodeGears.ReSharper.Exceptional.Model;
using JetBrains.DocumentModel;
using JetBrains.ReSharper.Daemon;

namespace CodeGears.ReSharper.Exceptional.Highlightings
{
    [StaticSeverityHighlighting(Severity.WARNING)]
    public class ExceptionNotThrownHighlighting : HighlightingBase
    {
        internal ExceptionDocCommentModel ExceptionDocumentationModel { get; private set; }

        internal ExceptionNotThrownHighlighting(ExceptionDocCommentModel exceptionDocumentationModel)
        {
            ExceptionDocumentationModel = exceptionDocumentationModel;
        }

        public override DocumentRange Range
        {
            get { return this.ExceptionDocumentationModel.DocumentRange; }
        }

        protected override string Message
        {
            get { return String.Format(Resources.HighLightNotThrownDocumentedExceptions, this.ExceptionDocumentationModel.ExceptionType.GetCLRName()); }
        }
    }
}