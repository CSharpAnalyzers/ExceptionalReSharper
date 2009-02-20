using CodeGears.ReSharper.Exceptional.Model;

namespace CodeGears.ReSharper.Exceptional.Analyzers
{
    public class IsDocumentedExceptionThrownAnalyzer : Visitor
    {
        private ThrownExceptionsModel ThrownExceptionsModel { get; set; }

        public IsDocumentedExceptionThrownAnalyzer(ThrownExceptionsModel thrownExceptionsModel)
        {
            ThrownExceptionsModel = thrownExceptionsModel;
        }

        public override void Visit(ExceptionDocCommentModel exceptionDocumentationModel)
        {
            exceptionDocumentationModel.IsDocumentedExceptionThrown = Analyze(exceptionDocumentationModel);
        }

        private bool Analyze(ExceptionDocCommentModel exceptionDocCommentModel)
        {
            foreach (var throwStatement in this.ThrownExceptionsModel.ThrownExceptions)
            {
                if (throwStatement.Throws(exceptionDocCommentModel.ExceptionType) == false) continue;

                if (throwStatement.IsCatched) continue;

                return true;
            }

            return false;
        }
    }
}