using System;
using JetBrains.Application.Progress;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.Bulbs;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.TextControl;
using ReSharper.Exceptional.Highlightings;
using ReSharper.Exceptional.Utilities;

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
            var throwStatementModel = Error.ThrowStatementModel;

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
    }
}