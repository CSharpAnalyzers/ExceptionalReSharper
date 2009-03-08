using CodeGears.ReSharper.Exceptional.Highlightings;
using CodeGears.ReSharper.Exceptional.Model;

namespace CodeGears.ReSharper.Exceptional.Analyzers
{
    /// <summary>Analyzes an exception documentation and checks if it is thrown from the documented element.</summary>
    internal class IsDocumentedExceptionThrownAnalyzer : AnalyzerBase
    {
        public override void Visit(ExceptionDocCommentModel exceptionDocumentationModel)
        {
            if (exceptionDocumentationModel == null) return;
            if (AnalyzeIfExeptionThrown(exceptionDocumentationModel)) return;

            this.Process.AddHighlighting(exceptionDocumentationModel.DocumentRange, new ExceptionNotThrownHighlighting(exceptionDocumentationModel));
        }

        private static bool AnalyzeIfExeptionThrown(ExceptionDocCommentModel exceptionDocumentationModel)
        {
            foreach (var throwStatementModel in exceptionDocumentationModel.MethodDeclarationModel.ThrowStatementModelsNotCatched)
            {
                if(throwStatementModel.Throws(exceptionDocumentationModel.ExceptionType))
                {
                    return true;
                }
            }

            return false;
        }
    }
}