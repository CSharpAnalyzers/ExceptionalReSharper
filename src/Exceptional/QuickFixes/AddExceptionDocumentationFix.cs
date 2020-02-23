using System;
using JetBrains.Application.Progress;
using JetBrains.DocumentModel;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.LiveTemplates.Hotspots;
using JetBrains.ReSharper.Feature.Services.LiveTemplates.LiveTemplates;
using JetBrains.TextControl;
using ReSharper.Exceptional.Highlightings;
using ReSharper.Exceptional.Models;
using JetBrains.ReSharper.Feature.Services.LiveTemplates.Templates;
using JetBrains.ReSharper.Feature.Services.QuickFixes;
using JetBrains.ReSharper.Resources.Shell;

namespace ReSharper.Exceptional.QuickFixes
{
    //[QuickFix(null, BeforeOrAfter.Before)]
    [QuickFix()]
    internal class AddExceptionDocumentationFix : SingleActionFix
    {
        private ExceptionNotDocumentedOptionalHighlighting Error { get; set; }

        public AddExceptionDocumentationFix(ExceptionNotDocumentedOptionalHighlighting error)
        {
            Error = error;
        }

        public override string Text
        {
            get
            {
                return String.Format(Resources.QuickFixInsertExceptionDocumentation,
                    Error.ThrownException.ExceptionType.GetClrName().FullName);
            }
        }

        protected override Action<ITextControl> ExecutePsiTransaction(ISolution solution, IProgressIndicator progress)
        {
            var methodDeclaration = Error.ThrownException.AnalyzeUnit;
            var insertedExceptionModel = methodDeclaration.DocumentationBlock.AddExceptionDocumentation(Error.ThrownException, progress);

            if (insertedExceptionModel == null)
                return null;

            return MarkInsertedDescription(solution, insertedExceptionModel);
        }

        private Action<ITextControl> MarkInsertedDescription(ISolution solution, ExceptionDocCommentModel insertedExceptionModel)
        {
            var exceptionCommentRange = insertedExceptionModel.GetMarkerRange();
            if (exceptionCommentRange == DocumentRange.InvalidRange)
                return null;

            var copyExceptionDescription =
                string.IsNullOrEmpty(insertedExceptionModel.ExceptionDescription) ||
                insertedExceptionModel.ExceptionDescription.Contains("[MARKER]");

            var exceptionDescription = copyExceptionDescription ? "Condition" : insertedExceptionModel.ExceptionDescription.Trim();

            var nameSuggestionsExpression = new NameSuggestionsExpression(new[] {exceptionDescription});
            var field = new TemplateField("name", nameSuggestionsExpression, 0);
            var fieldInfo = new HotspotInfo(field, exceptionCommentRange);

            return textControl =>
            {
                var hotspotSession = Shell.Instance.GetComponent<LiveTemplatesManager>()
                    .CreateHotspotSessionAtopExistingText(
                        solution,
                        DocumentRange.InvalidRange,
                        textControl,
                        LiveTemplatesManager.EscapeAction.LeaveTextAndCaret,
                        fieldInfo);

                hotspotSession.Execute();
            };
        }
    }
}