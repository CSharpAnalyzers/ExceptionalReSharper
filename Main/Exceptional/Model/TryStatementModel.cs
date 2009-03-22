/// <copyright file="TryStatementModel.cs" manufacturer="CodeGears">
///   Copyright (c) CodeGears. All rights reserved.
/// </copyright>

using System.Collections.Generic;
using CodeGears.ReSharper.Exceptional.Analyzers;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Tree;

namespace CodeGears.ReSharper.Exceptional.Model
{
    internal class TryStatementModel : BlockModelBase<ITryStatementNode>
    {
        public List<CatchClauseModel> CatchClauseModels { get; private set; }

        public TryStatementModel(IAnalyzeUnit analyzeUnit, ITryStatementNode tryStatement)
            : base(analyzeUnit, tryStatement)
        {
            CatchClauseModels = new List<CatchClauseModel>();
        }

        public override bool CatchesException(IDeclaredType exception)
        {
            foreach (var catchClauseModel in this.CatchClauseModels)
            {
                if(catchClauseModel.Catches(exception))
                {
                    return true;
                }
            }

            //TODO: Refactor. Construct catch models right at try processing.
            foreach (var catchClause in this.Node.Catches)
            {
                if(catchClause.ExceptionType == null && exception.GetCLRName().Equals("System.Exception"))
                    return true;

                if(catchClause.ExceptionType == null)
                    continue;

                if (exception.IsSubtypeOf(catchClause.ExceptionType))
                    return true;
            }

            return this.ParentBlock.CatchesException(exception);
        }

        public override IDeclaredType GetCatchedException()
        {
            return this.ParentBlock.GetCatchedException();
        }

        public override IEnumerable<ThrownExceptionModel> ThrownExceptionModelsNotCatched
        {
            get
            {
                foreach (var throwStatementModel in base.ThrownExceptionModelsNotCatched)
                {
                    yield return throwStatementModel;
                }

                for (var i = 0; i < this.CatchClauseModels.Count; i++)
                {
                    IBlockModel catchClauseModel = this.CatchClauseModels[i];
                    foreach (var model in catchClauseModel.ThrownExceptionModelsNotCatched)
                    {
                        yield return model;
                    }
                }
            }
        }

        public override void Accept(AnalyzerBase analyzerBase)
        {
            base.Accept(analyzerBase);

            foreach (var catchClauseModel in this.CatchClauseModels)
            {
                catchClauseModel.Accept(analyzerBase);
            }
        }
    }
}