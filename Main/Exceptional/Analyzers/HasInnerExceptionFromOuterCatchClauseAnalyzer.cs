using CodeGears.ReSharper.Exceptional.Highlightings;
using CodeGears.ReSharper.Exceptional.Model;

namespace CodeGears.ReSharper.Exceptional.Analyzers
{
    /// <summary>Analyzes throw statements and checks if the contain inner exception when thrown from inside a catch clause.</summary>
    internal class HasInnerExceptionFromOuterCatchClauseAnalyzer : AnalyzerBase
    {
        public override void Visit(ThrowStatementModel throwStatementModel)
        {
            if (throwStatementModel == null) return;
            if (AnalyzeIfHasInnerException(throwStatementModel) == false) return;

            this.Process.AddHighlighting(throwStatementModel.DocumentRange, new ThrowFromCatchWithNoInnerExceptionHighlighting(throwStatementModel));
        }

        private static bool AnalyzeIfHasInnerException(ThrowStatementModel throwStatementModel)
        {
            if (ProcessContext.Instance.IsValid() == false) return true;

            var outerCatchClause = throwStatementModel.FindOuterCatchClause();
            if (outerCatchClause == null) return true;

            outerCatchClause.IsRethrown = throwStatementModel.IsRethrow;

            if (outerCatchClause.HasExceptionType == false || throwStatementModel.IsRethrow)
            {
                //There is no variable decaration or there is no exception creation
                return throwStatementModel.IsRethrow;
            }

            if(outerCatchClause.HasVariable == false)
            {
                return false;
            }

            return false;
//            var list = new List<IDeclaredElement>(outerCatch.LocalVariables);
//            var catchVariable = list.Find(element => element is ICatchVariableDeclaration);
//            if (catchVariable == null) return false;
//
//            var exception = throwStatementModel.ThrowStatement.Exception as IObjectCreationExpressionNode;
//            if (exception == null) return false;
//
//            var arguments = new List<ICSharpArgumentNode>(exception.ArgumentList.Arguments);
//            var match = arguments.Find(arg =>
//            {
//                var reference = arg.ValueNode as IReferenceExpressionNode;
//                if (reference == null) return false;
//
//                return reference.NameIdentifier.Name.Equals(catchVariable.ShortName);
//            });
//
//            var result = match != null;
//
//            catchModel.IsRethrown = result;
//
//            return result;
        }
    }
}