/// <copyright file="InsertExceptionDocumentationFix.cs" manufacturer="CodeGears">
///   Copyright (c) CodeGears. All rights reserved.
/// </copyright>

using System;
using CodeGears.ReSharper.Exceptional.Highlightings;
using JetBrains.Application.Progress;
using JetBrains.DocumentModel;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.LiveTemplates.Hotspots;
using JetBrains.ReSharper.Feature.Services.LiveTemplates.LiveTemplates;
using JetBrains.ReSharper.Intentions;
using JetBrains.ReSharper.LiveTemplates;
using JetBrains.TextControl;
using JetBrains.Util;

namespace CodeGears.ReSharper.Exceptional.QuickFixes
{
    [QuickFix]
    internal class InsertExceptionDocumentationFix : SingleActionFix
    {
        private ExceptionNotDocumentedHighlighting Error { get; set; }

        public InsertExceptionDocumentationFix(ExceptionNotDocumentedHighlighting error)
        {
            Error = error;
        }

        protected override Action<ITextControl> ExecuteTransaction(ISolution solution, IProgressIndicator progress)
        {
            var methodDeclaration = this.Error.ThrownExceptionModel.AnalyzeUnit;

            var insertedExceptionModel = methodDeclaration.DocCommentBlockModel.AddExceptionDocumentation(this.Error.ThrownExceptionModel.ExceptionType);
            if (insertedExceptionModel == null) return null;

            var exceptionCommentRange = insertedExceptionModel.GetDescriptionDocumentRange();
            if (exceptionCommentRange == DocumentRange.InvalidRange) return null;

            var nameSuggestionsExpression = new NameSuggestionsExpression(new[] {"Thrown when "});
            var field = new TemplateField("name", nameSuggestionsExpression, 0);
            var fieldInfo = new HotspotInfo(field, exceptionCommentRange.TextRange);

            return textControl =>
                       {
                           var hotspotSession = LiveTemplatesManager.CreateHotpotSessionAtopExistingText(
                               this.Error.ThrownExceptionModel.ExceptionType.GetManager().Solution,
                               TextRange.InvalidRange,
                               textControl,
                               LiveTemplatesManager.EscapeAction.LeaveTextAndCaret,
                               new[] {fieldInfo});

                           hotspotSession.Execute();
                       };
        }

        public override string Text
        {
            get { return String.Format(Resources.QuickFixInsertExceptionDocumentation, this.Error.ThrownExceptionModel.ExceptionType.GetCLRName()); }
        }
    }
}