using System;
using JetBrains.Application.Progress;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.Bulbs;
using JetBrains.TextControl;
using JetBrains.Util;
using ReSharper.Exceptional.Highlightings;

namespace ReSharper.Exceptional.QuickFixes
{
    [QuickFix]
    internal class CatchExceptionFix : SingleActionFix
    {
        private ExceptionNotDocumentedOptionalHighlighting Error { get; set; }

        public CatchExceptionFix(ExceptionNotDocumentedOptionalHighlighting error)
        {
            Error = error;
        }

        protected override Action<ITextControl> ExecutePsiTransaction(ISolution solution, IProgressIndicator progress)
        {
            var exceptionsOriginModel = Error.ThrownExceptionModel.Parent;

            var nearestTryBlock = exceptionsOriginModel.ContainingBlockModel.FindNearestTryBlock();
            if (nearestTryBlock == null)
            {
                exceptionsOriginModel.SurroundWithTryBlock(Error.ThrownExceptionModel.ExceptionType);
            }
            else
            {
                nearestTryBlock.AddCatchClause(Error.ThrownExceptionModel.ExceptionType);
            }

            return null;
        }

        public override string Text
        {
            get { return String.Format(Resources.QuickFixCatchException, Error.ThrownExceptionModel.ExceptionType.GetClrName().ShortName); }
        }
    }
}