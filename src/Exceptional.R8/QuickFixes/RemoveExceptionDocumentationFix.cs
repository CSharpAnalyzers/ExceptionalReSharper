using System;
using JetBrains.Application.Progress;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.Bulbs;
using JetBrains.TextControl;
using ReSharper.Exceptional.Highlightings;

#if R9 || R10
using JetBrains.ReSharper.Feature.Services.QuickFixes;
#endif

namespace ReSharper.Exceptional.QuickFixes
{
    [QuickFix]
    internal class RemoveExceptionDocumentationFix : SingleActionFix
    {
        private ExceptionNotThrownHighlighting Error { get; set; }

        public RemoveExceptionDocumentationFix(ExceptionNotThrownHighlighting error)
        {
            Error = error;
        }

        public override string Text
        {
            get { return Resources.QuickFixRemoveExceptionDocumentation; }
        }

        protected override Action<ITextControl> ExecutePsiTransaction(ISolution solution, IProgressIndicator progress)
        {
            var docCommentModel = Error.ExceptionDocumentation.AnalyzeUnit.DocumentationBlock;
            docCommentModel.RemoveExceptionDocumentation(Error.ExceptionDocumentation, progress);
            return null;
        }
    }
}