using System.Collections.Generic;
using CodeGears.ReSharper.Exceptional.Highlightings;
using JetBrains.Application;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.CodeInsight.Services.Lookup;
using JetBrains.ReSharper.Daemon;
using JetBrains.ReSharper.Intentions.Util;
using JetBrains.ReSharper.LiveTemplates;
using JetBrains.ReSharper.LiveTemplates.Execution;
using JetBrains.ReSharper.Psi;
using JetBrains.TextControl;
using JetBrains.Util;
using TemplateUtil=JetBrains.ReSharper.LiveTemplates.TemplateUtil;

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

        public override void Execute(ISolution solution, ITextControl textControl)
        {
            using (CommandCookie.Create(Resources.QuickFixInsertExceptionDocumentation))
            {
                var exceptionCommentRange = TextRange.InvalidRange;

                PsiManager.GetInstance(solution).DoTransaction(
                    delegate
                        {
                            var model = this.Error.ThrowStatementModel;
                            var declaratiopnTreeNode = model.ThrowStatement.GetContainingTypeMemberDeclaration().ToTreeNode();
                            exceptionCommentRange = XmlDocCommentHelper.InsertExceptionDocumentation(declaratiopnTreeNode, model.ExceptionType.GetCLRName());
                        });

                if(exceptionCommentRange == TextRange.InvalidRange) return;

                var messageSuggestions = new List<string>(new[] { "Thrown when " });
                var messageTemplateField = new TemplateFieldInfo(new TemplateField("Comment", new ParamNamesExpression(messageSuggestions, messageSuggestions[0]), 0), new[] { exceptionCommentRange });
                JetBrains.ReSharper.Intentions.Util.TemplateUtil.ExecuteTemplate(solution, textControl, exceptionCommentRange, new[] { messageTemplateField });

                var currentSession = LiveTemplatesController.Instance.CurrentSession;
                if(currentSession != null)
                {
                    currentSession.TemplateSession.Closed +=
                        delegate
                            {
                                textControl.SelectionModel.RemoveSelection();
                            };
                }
            }
        }

        public override string Text
        {
            get { return Resources.QuickFixInsertExceptionDocumentation; }
        }

        private class ParamNamesExpression : QuickFixTemplateExpression
        {
            private readonly List<string> myNames;

            public ParamNamesExpression(List<string> names, string defaultName)
                : base(defaultName)
            {
                this.myNames = names;
            }

            protected override IList<ILookupItem> GetLookupItems()
            {
                return this.myNames.ConvertAll<ILookupItem>(name => new TextLookupItem(name));
            }
        }
    }
}