using CodeGears.ReSharper.Exceptional.Highlightings;
using JetBrains.Application;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon;
using JetBrains.ReSharper.Psi;
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

        public override void Execute(ISolution solution, ITextControl textControl)
        {
            using (CommandCookie.Create(Resources.QuickFixRemoveExceptionDocumentation))
            {
                PsiManager.GetInstance(solution).DoTransaction(
                    delegate
                    {
                        //this.Error.ExceptionDocumentationModel.Remove();
                    });
            }
        }

        public override string Text
        {
            get { return Resources.QuickFixRemoveExceptionDocumentation; }
        }
    }
}