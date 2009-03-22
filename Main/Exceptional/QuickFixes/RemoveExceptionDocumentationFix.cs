/// <copyright>Copyright (c) 2009 CodeGears.net All rights reserved.</copyright>

using System;
using CodeGears.ReSharper.Exceptional.Highlightings;
using JetBrains.Application.Progress;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Intentions;
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