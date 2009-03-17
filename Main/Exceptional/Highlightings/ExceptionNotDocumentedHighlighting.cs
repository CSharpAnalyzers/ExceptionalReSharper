/// <copyright file="ExceptionNotDocumentedHighlighting.cs" manufacturer="CodeGears">
///   Copyright (c) CodeGears. All rights reserved.
/// </copyright>

using System;
using CodeGears.ReSharper.Exceptional.Model;
using JetBrains.DocumentModel;
using JetBrains.ReSharper.Daemon;

namespace CodeGears.ReSharper.Exceptional.Highlightings
{
    [StaticSeverityHighlighting(Severity.WARNING)]
    public class ExceptionNotDocumentedHighlighting : HighlightingBase
    {
        internal ThrownExceptionModel ThrownExceptionModel { get; private set; }

        internal ExceptionNotDocumentedHighlighting(ThrownExceptionModel thrownExceptionModel)
        {
            ThrownExceptionModel = thrownExceptionModel;
        }

        public override DocumentRange Range
        {
            get { return this.ThrownExceptionModel.DocumentRange; }
        }

        protected override string Message
        {
            get
            {
                var exceptionType = this.ThrownExceptionModel.ExceptionType;
                var exceptionTypeName = exceptionType != null ? exceptionType.GetCLRName() : "[NOT RESOLVED]";
                return String.Format(Resources.HighLightNotDocumentedExceptions, exceptionTypeName);
            }
        }
    }
}