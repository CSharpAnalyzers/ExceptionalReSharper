// Copyright (c) 2009-2010 Cofinite Solutions. All rights reserved.
using System;
using CodeGears.ReSharper.Exceptional.Highlightings;
using JetBrains.Application.Progress;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.Bulbs;
using JetBrains.TextControl;

namespace CodeGears.ReSharper.Exceptional.QuickFixes
{
    [QuickFix]
    internal class CatchExceptionFix : SingleActionFix
    {
        private ExceptionNotDocumentedHighlighting Error { get; set; }

        public CatchExceptionFix(ExceptionNotDocumentedHighlighting error)
        {
            Error = error;
        }

        protected override Action<ITextControl> ExecuteTransaction(ISolution solution, IProgressIndicator progress)
        {
            var exceptionsOriginModel = this.Error.ThrownExceptionModel.Parent;

            var nearestTryBlock = exceptionsOriginModel.ContainingBlockModel.FindNearestTryBlock();

            if(nearestTryBlock == null)
            {
                exceptionsOriginModel.SurroundWithTryBlock(this.Error.ThrownExceptionModel.ExceptionType);
            }
            else
            {
                nearestTryBlock.AddCatchClause(this.Error.ThrownExceptionModel.ExceptionType);
            }

            return null;
        }

        public override string Text
        {
            get { return String.Format(Resources.QuickFixCatchException, this.Error.ThrownExceptionModel.ExceptionType.GetCLRName()); }
        }
    }
}