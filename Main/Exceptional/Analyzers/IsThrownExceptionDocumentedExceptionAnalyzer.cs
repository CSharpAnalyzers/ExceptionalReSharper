using CodeGears.ReSharper.Exceptional.Model;

namespace CodeGears.ReSharper.Exceptional.Analyzers
{
    internal class IsThrownExceptionDocumentedExceptionAnalyzer : AnalyzerBase
    {
        public override void Visit(ThrowStatementModel throwStatementModel)
        {
            throwStatementModel.IsDocumented = Analyze(throwStatementModel);
        }

        private bool Analyze(ThrowStatementModel throwStatementModel)
        {
            if (ProcessContext.Instance.IsValid() == false) return false;

            var docCommentBlockNode = ProcessContext.Instance.MethodDeclarationModel.DocCommentBlockModel;
            if(docCommentBlockNode == null) return false;

            foreach (var documentedException in docCommentBlockNode.Exceptions)
            {
                if(throwStatementModel.Throws(documentedException.ExceptionType))
                {
                    return true;
                }
            }

            return false;
        }
    }
}