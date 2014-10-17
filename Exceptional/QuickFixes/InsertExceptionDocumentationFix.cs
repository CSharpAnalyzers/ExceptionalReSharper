using System;
using JetBrains.Application;
using JetBrains.Application.Progress;
using JetBrains.DocumentModel;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.Bulbs;
using JetBrains.ReSharper.Feature.Services.LiveTemplates.Hotspots;
using JetBrains.ReSharper.Feature.Services.LiveTemplates.LiveTemplates;
using JetBrains.ReSharper.LiveTemplates;
using JetBrains.TextControl;
using JetBrains.Util;
using ReSharper.Exceptional.Highlightings;
using ReSharper.Exceptional.Models;

namespace ReSharper.Exceptional.QuickFixes
{
    [QuickFix]
    internal class InsertExceptionDocumentationFix : SingleActionFix
    {
        private ExceptionNotDocumentedOptionalHighlighting Error { get; set; }

        public InsertExceptionDocumentationFix(ExceptionNotDocumentedOptionalHighlighting error)
        {
            Error = error;
        }

        protected override Action<ITextControl> ExecutePsiTransaction(ISolution solution, IProgressIndicator progress)
        {
            var methodDeclaration = Error.ThrownExceptionModel.AnalyzeUnit;

            ExceptionDocCommentModel insertedExceptionModel = null;
            try
            {
                insertedExceptionModel = methodDeclaration.DocCommentBlockModel
                    .AddExceptionDocumentation(Error.ThrownExceptionModel.ExceptionType, Error.ThrownExceptionModel.ExceptionDescription, progress);
            }
            catch (Exception exception)
            {
                // TODO: Find reason for very sporadic InvalidOperationException in AnalyzeUnitModelBase`1.AddDocCommentNode => ModificationUtil.AddChildBefore
                MessageBox.ShowError("Error in QuickFix: " + exception.Message + "\nStackTrace: \n" + exception.StackTrace);
                return null; 
            }

            if (insertedExceptionModel == null)
                return null;

            var exceptionCommentRange = insertedExceptionModel.GetDescriptionDocumentRange();
            if (exceptionCommentRange == DocumentRange.InvalidRange)
                return null;

            var copyExceptionDescription = string.IsNullOrEmpty(insertedExceptionModel.ExceptionDescription) ||
                insertedExceptionModel.ExceptionDescription.Contains("[MARKER]");
            var exceptionDescription = copyExceptionDescription ? string.Empty : insertedExceptionModel.ExceptionDescription;

            // TODO: Replace string.Empty with default text of exceptionType if available

            var nameSuggestionsExpression = new NameSuggestionsExpression(new[] { exceptionDescription });
            var field = new TemplateField("name", nameSuggestionsExpression, 0);
            var fieldInfo = new HotspotInfo(field, exceptionCommentRange);

            return textControl =>
            {
                var hotspotSession = Shell.Instance.GetComponent<LiveTemplatesManager>().CreateHotspotSessionAtopExistingText(
                    Error.ThrownExceptionModel.ExceptionType.Module.GetSolution(),
                    TextRange.InvalidRange,
                    textControl,
                    LiveTemplatesManager.EscapeAction.LeaveTextAndCaret,
                    new[] { fieldInfo });

                hotspotSession.Execute();
            };
        }

        public override string Text
        {
            get
            {
                return String.Format(Resources.QuickFixInsertExceptionDocumentation,
                    Error.ThrownExceptionModel.ExceptionType.GetClrName().ShortName);
            }
        }
    }
}