using CodeGears.ReSharper.Exceptional.Highlightings;
using CodeGears.ReSharper.Exceptional.Model;
using CodeGears.ReSharper.Exceptional.Templates;
using JetBrains.Application;
using JetBrains.DocumentModel;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.TextControl;

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
                ExceptionDocCommentModel insertedExceptionModel = null;

                PsiManager.GetInstance(solution).DoTransaction(
                    delegate
                        {
                            var methodDeclaration = this.Error.ThrowStatementModel.MethodDeclarationModel;
                            methodDeclaration.EnsureHasDocComment();
                            
                            if(methodDeclaration.DocCommentBlockModel != null)
                            {
                                insertedExceptionModel = methodDeclaration.DocCommentBlockModel.AddExceptionDocumentation(this.Error.ThrowStatementModel.ExceptionType);   
                            }
                        });

                if (insertedExceptionModel == null) return;

                var exceptionCommentRange = insertedExceptionModel.GetDescriptionDocumentRange();
                if (exceptionCommentRange == DocumentRange.InvalidRange) return;

                var templateHotSpot = new TemplateHotSpot("Comment", exceptionCommentRange.TextRange);
                templateHotSpot.Suggestions.Add("Thrown when ");
                
                TemplateRunner.Run(solution, textControl, exceptionCommentRange.TextRange, templateHotSpot);
            }
        }

        public override string Text
        {
            get { return Resources.QuickFixInsertExceptionDocumentation; }
        }
    }
}