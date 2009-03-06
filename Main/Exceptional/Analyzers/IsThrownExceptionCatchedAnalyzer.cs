using CodeGears.ReSharper.Exceptional.Model;

namespace CodeGears.ReSharper.Exceptional.Analyzers
{
    internal class IsThrownExceptionCatchedAnalyzer : AnalyzerBase
    {
        public override void Visit(ThrowStatementModel throwStatementModel)
        {
            throwStatementModel.IsCatched = AnalyzeIfCatched(throwStatementModel);
            throwStatementModel.IsDocumented = AnalyzeIfDocumented(throwStatementModel);
        }

        private static bool AnalyzeIfDocumented(ThrowStatementModel throwStatementModel)
        {
            if (ProcessContext.Instance.IsValid() == false) return false;

            var docCommentBlockNode = ProcessContext.Instance.MethodDeclarationModel.DocCommentBlockModel;
            if (docCommentBlockNode == null) return false;

            foreach (var exceptionDocumentationModel in docCommentBlockNode.Exceptions)
            {
                if(throwStatementModel.Throws(exceptionDocumentationModel.ExceptionType))
                {
                    exceptionDocumentationModel.IsThrown |= throwStatementModel.IsCatched == false;
                    return true;
                }
            }

            return false;
        }

        private static bool AnalyzeIfCatched(ThrowStatementModel throwStatementModel)
        {
            if (ProcessContext.Instance.IsValid() == false) return false;

            var exception = throwStatementModel.ExceptionType;
            if (exception == null) return false;

            return throwStatementModel.ContainingBlockModel.CatchesException(exception);
        }
    }
}