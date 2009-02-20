using CodeGears.ReSharper.Exceptional.Model;

namespace CodeGears.ReSharper.Exceptional.Analyzers
{
    public class IsThrownExceptionDocumentedExceptionAnalyzer : Visitor
    {
        private DocumentedExceptionsModel DocumentedExceptionsModel { get; set; }

        public IsThrownExceptionDocumentedExceptionAnalyzer(DocumentedExceptionsModel documentedExceptionsModel)
        {
            DocumentedExceptionsModel = documentedExceptionsModel;
        }

        public override void Visit(ThrowStatementModel throwStatementModel)
        {
            throwStatementModel.IsDocumented = Analyze(throwStatementModel);
        }

        private bool Analyze(ThrowStatementModel throwStatementModel)
        {
            foreach (var documentedException in this.DocumentedExceptionsModel.DocumentedExceptions)
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