using CodeGears.ReSharper.Exceptional.Highlightings;
using JetBrains.Application;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon;
using JetBrains.ReSharper.Intentions.Base2;
using JetBrains.ReSharper.Psi;
using JetBrains.TextControl;
using JetBrains.Util;

namespace CodeGears.ReSharper.Exceptional.QuickFixes
{
    [QuickFix]
    internal class RemoveExceptionDocumentationFix : OneItemBulbActionImpl, IBulbItem, IQuickFix
    {
        private ExceptionNotThrownHighlighting Error { get; set; }

        public RemoveExceptionDocumentationFix(ExceptionNotThrownHighlighting error)
        {
            Error = error;
        }

        protected override IBulbItem GetBulbItem(IUserDataHolder cache)
        {
            return this;
        }

        public void Execute(ISolution solution, ITextControl textControl)
        {
            using (CommandCookie.Create(Resources.QuickFixRemoveExceptionDocumentation))
            {
                PsiManager.GetInstance(solution).DoTransaction(
                    delegate
                    {
                        var declaratiopnTreeNode = this.Error.MemberDeclaration.ToTreeNode();
                        var commentText = textControl.Document.GetText(this.Error.DocumentRange.TextRange);
                        XmlDocCommentHelper.RemoveExceptionDocumentation(declaratiopnTreeNode, commentText);
                    });
            }
        }

        public string Text
        {
            get { return Resources.QuickFixRemoveExceptionDocumentation; }
        }
    }
}