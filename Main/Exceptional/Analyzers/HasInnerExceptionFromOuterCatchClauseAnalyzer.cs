using System.Collections.Generic;
using CodeGears.ReSharper.Exceptional.Model;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Tree;

namespace CodeGears.ReSharper.Exceptional.Analyzers
{
    internal class HasInnerExceptionFromOuterCatchClauseAnalyzer : AnalyzerBase
    {
        public override void Visit(ThrowStatementModel throwStatementModel)
        {
            throwStatementModel.ThrowsWithInnerException = Analyze(throwStatementModel);
        }

        private bool Analyze(ThrowStatementModel throwStatementModel)
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