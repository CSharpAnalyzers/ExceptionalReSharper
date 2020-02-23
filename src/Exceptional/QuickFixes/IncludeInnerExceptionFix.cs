using System;
using JetBrains.Application.Progress;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.TextControl;
using ReSharper.Exceptional.Highlightings;
using ReSharper.Exceptional.Utilities;
using JetBrains.ReSharper.Feature.Services.QuickFixes;

namespace ReSharper.Exceptional.QuickFixes
{
    [QuickFix]
    internal class IncludeInnerExceptionFix : SingleActionFix
    {
        private ThrowFromCatchWithNoInnerExceptionHighlighting Error { get; set; }

        public IncludeInnerExceptionFix(ThrowFromCatchWithNoInnerExceptionHighlighting error)
        {
            Error = error;
        }

        public override string Text
        {
            get { return Resources.QuickFixIncludeInnerException; }
        }

        protected override Action<ITextControl> ExecutePsiTransaction(ISolution solution, IProgressIndicator progress)
        {
            if (Error.ThrowStatement != null)
            {
                return this.ExecutePsiTransactionForStatement();
            }

            return this.ExecutePsiTransactionForExpression();
        }

        private Action<ITextControl> ExecutePsiTransactionForStatement()
        {
            var throwStatementModel = Error.ThrowStatement;
            var outerCatchClause = throwStatementModel.FindOuterCatchClause();

            string variableName;
            if (outerCatchClause.Node is ISpecificCatchClause)
                variableName = ((ISpecificCatchClause)outerCatchClause.Node).ExceptionDeclaration.DeclaredName;
            else
                variableName = NameFactory.CatchVariableName(outerCatchClause.Node, outerCatchClause.CaughtException);

            if (outerCatchClause.Node is ISpecificCatchClause)
            {
                outerCatchClause.AddCatchVariable(variableName);
                throwStatementModel.AddInnerException(variableName);
            }
            else
            {
                throwStatementModel.AddInnerException(variableName);
                outerCatchClause.AddCatchVariable(variableName);
            }

            return null;
        }

        private Action<ITextControl> ExecutePsiTransactionForExpression()
        {
            var throwExpressionModel = Error.ThrowExpression;
            var outerCatchClause = throwExpressionModel.FindOuterCatchClause();

            string variableName;
            if (outerCatchClause.Node is ISpecificCatchClause)
                variableName = ((ISpecificCatchClause)outerCatchClause.Node).ExceptionDeclaration.DeclaredName;
            else
                variableName = NameFactory.CatchVariableName(outerCatchClause.Node, outerCatchClause.CaughtException);

            if (outerCatchClause.Node is ISpecificCatchClause)
            {
                outerCatchClause.AddCatchVariable(variableName);
                throwExpressionModel.AddInnerException(variableName);
            }
            else
            {
                throwExpressionModel.AddInnerException(variableName);
                outerCatchClause.AddCatchVariable(variableName);
            }

            return null;
        }
    }
}