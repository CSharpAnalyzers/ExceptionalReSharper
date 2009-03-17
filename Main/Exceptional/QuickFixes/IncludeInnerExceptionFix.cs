/// <copyright file="IncludeInnerExceptionFix.cs" manufacturer="CodeGears">
///   Copyright (c) CodeGears. All rights reserved.
/// </copyright>

using System;
using CodeGears.ReSharper.Exceptional.Highlightings;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Intentions;
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

        protected override Action<ITextControl> ExecuteTransaction(ISolution solution, JetBrains.Application.Progress.IProgressIndicator progress)
        {
            var throwStatementModel = this.Error.ThrowStatementModel;

            var outerCatchClause = throwStatementModel.FindOuterCatchClause();
            var variableName = outerCatchClause.SuggestVariableName();

            if(outerCatchClause.Node is ISpecificCatchClauseNode)
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