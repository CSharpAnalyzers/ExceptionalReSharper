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
    public class InsertExceptionDocumentationFix : OneItemBulbActionImpl, IBulbItem, IQuickFix
    {
        private ExceptionNotDocumentedHighlighting Error { get; set; }

        public InsertExceptionDocumentationFix(ExceptionNotDocumentedHighlighting error)
        {
            Error = error;
        }

        protected override IBulbItem GetBulbItem(IUserDataHolder cache)
        {
            return this;
        }

        public void Execute(ISolution solution, ITextControl textControl)
        {
            using (CommandCookie.Create(Resources.QuickFixInsertExceptionDocumentation))
            {
                PsiManager.GetInstance(solution).DoTransaction(
                    delegate
                        {
                            var declaratiopnTreeNode = this.Error.ThrowStatement.GetContainingTypeMemberDeclaration().ToTreeNode();
                            XmlDocCommentHelper.AddExceptionDocumentation(declaratiopnTreeNode, this.Error.Exception.GetCLRName(), this.Error.ThrowStatement.GetProject());
                        });
            }
        }

        public string Text
        {
            get { return Resources.QuickFixInsertExceptionDocumentation; }
        }
    }
}