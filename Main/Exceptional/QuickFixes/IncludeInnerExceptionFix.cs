// Copyright (c) 2009-2010 Cofinite Solutions. All rights reserved.
using System;
using CodeGears.ReSharper.Exceptional.Highlightings;
using JetBrains.Application.Progress;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.Bulbs;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.TextControl;

namespace CodeGears.ReSharper.Exceptional.QuickFixes
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
            var variableName = NameFactory.CatchVariableName(outerCatchClause.Node, outerCatchClause.GetCatchedException());

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