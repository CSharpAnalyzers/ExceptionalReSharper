using CodeGears.ReSharper.Exceptional.Model;

namespace CodeGears.ReSharper.Exceptional.Analyzers
{
    internal class IsDocumentedExceptionThrownAnalyzer : AnalyzerBase
    {
        public override void Visit(ExceptionDocumentationModel exceptionDocumentationModel)
        {
            exceptionDocumentationModel.IsThrown = Analyze(exceptionDocumentationModel);
        }

        private bool Analyze(ExceptionDocumentationModel exceptionDocCommentModel)
        {
            ProcessContext.Instance.AssertMethodDeclaration();
            var throwStatementModels = ProcessContext.Instance.MethodDeclarationModel.ThrowStatementModels;

            foreach (var throwStatement in throwStatementModels)
            {
                if (throwStatement.Throws(exceptionDocCommentModel.ExceptionTypeName) == false) continue;

                if (throwStatement.IsCatched) continue;

                return true;
            }

            return false;
        }
    }
}