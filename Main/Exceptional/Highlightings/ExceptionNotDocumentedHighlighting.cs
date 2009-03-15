/// <copyright file="ExceptionNotDocumentedHighlighting.cs" manufacturer="CodeGears">
///   Copyright (c) CodeGears. All rights reserved.
/// </copyright>

using System;
using CodeGears.ReSharper.Exceptional.Model;
using JetBrains.DocumentModel;
using JetBrains.ReSharper.Daemon;
using JetBrains.ReSharper.Daemon.CSharp.Stages;

namespace CodeGears.ReSharper.Exceptional.Highlightings
{
    [StaticSeverityHighlighting(Severity.WARNING)]
    public class ExceptionNotDocumentedHighlighting : CSharpHighlightingBase, IHighlighting
    {
        internal ThrownExceptionModel ThrownExceptionModel { get; private set; }

        internal ExceptionNotDocumentedHighlighting(ThrownExceptionModel thrownExceptionModel)
        {
            ThrownExceptionModel = thrownExceptionModel;
        }

        public override bool IsValid()
        {
            return true;
        }

        public override DocumentRange Range
        {
            get { return this.ThrownExceptionModel.DocumentRange; }
        }

        public string ToolTip
        {
            get { return Message; }
        }

        public string ErrorStripeToolTip
        {
            get { return Message; }
        }

        private string Message
        {
            get
            {
                var exceptionType = this.ThrownExceptionModel.ExceptionType;
                var exceptionTypeName = exceptionType != null ? exceptionType.GetCLRName() : "[NOT RESOLVED]";
                return String.Format(Resources.HighLightNotDocumentedExceptions, exceptionTypeName);
            }
        }

        public int NavigationOffsetPatch
        {
            get { return 0; }
        }
    }
}