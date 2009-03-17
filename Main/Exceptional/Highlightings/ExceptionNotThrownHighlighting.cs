/// <copyright file="ExceptionNotThrownHighlighting.cs" manufacturer="CodeGears">
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
    public class ExceptionNotThrownHighlighting : CSharpHighlightingBase, IHighlighting
    {
        internal ExceptionDocCommentModel ExceptionDocumentationModel { get; private set; }

        internal ExceptionNotThrownHighlighting(ExceptionDocCommentModel exceptionDocumentationModel)
        {
            ExceptionDocumentationModel = exceptionDocumentationModel;
        }

        public override bool IsValid()
        {
            return true;
        }

        public override DocumentRange Range
        {
            get { return this.ExceptionDocumentationModel.DocumentRange; }
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
            get { return String.Format(Resources.HighLightNotThrownDocumentedExceptions, this.ExceptionDocumentationModel.ExceptionType.GetCLRName()); }
        }

        public int NavigationOffsetPatch
        {
            get { return 0; }
        }
    }
}