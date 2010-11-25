// Copyright (c) 2009-2010 Cofinite Solutions. All rights reserved.
using System;
using CodeGears.ReSharper.Exceptional.Highlightings;
using JetBrains.Application.Progress;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.Bulbs;
using JetBrains.TextControl;

namespace CodeGears.ReSharper.Exceptional.QuickFixes
{
    [QuickFix]
    internal class RemoveExceptionDocumentationFix : SingleActionFix
    {
        private ExceptionNotThrownHighlighting Error { get; set; }

        public RemoveExceptionDocumentationFix(ExceptionNotThrownHighlighting error)
        {
            Error = error;
        }

        protected override Action<ITextControl> ExecuteTransaction(ISolution solution, IProgressIndicator progress)
        {
            var docCommentModel = this.Error.ExceptionDocumentationModel.AnalyzeUnit.DocCommentBlockModel;
            docCommentModel.RemoveExceptionDocumentation(this.Error.ExceptionDocumentationModel);

            return null;
        }

        public override string Text
        {
            get { return Resources.QuickFixRemoveExceptionDocumentation; }
        }
    }
}