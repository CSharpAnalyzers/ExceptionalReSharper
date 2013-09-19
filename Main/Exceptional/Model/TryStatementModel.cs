// Copyright (c) 2009-2010 Cofinite Solutions. All rights reserved.
using System.Collections.Generic;
using CodeGears.ReSharper.Exceptional.Analyzers;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Tree;

namespace CodeGears.ReSharper.Exceptional.Model
{
    internal class TryStatementModel : BlockModelBase<ITryStatement>
    {
        public List<CatchClauseModel> CatchClauseModels { get; private set; }

        public TryStatementModel(IAnalyzeUnit analyzeUnit, ITryStatement tryStatement)
            : base(analyzeUnit, tryStatement)
        {
            CatchClauseModels = GetCatchClauses();
        }

        private List<CatchClauseModel> GetCatchClauses()
        {
            var result = new List<CatchClauseModel>();

            foreach (var catchClause in Node.Catches)
            {
                var model = new CatchClauseModel(AnalyzeUnit, catchClause as ICatchClause);
                model.ParentBlock = this;
                result.Add(model);
            }

            return result;
        }

        public override bool CatchesException(IDeclaredType exception)
        {
            foreach (var catchClauseModel in CatchClauseModels)
            {
                if (catchClauseModel.Catches(exception))
                {
                    return true;
                }
            }

            return ParentBlock.CatchesException(exception);
        }

        public override IDeclaredType GetCatchedException()
        {
            return ParentBlock.GetCatchedException();
        }

        public override IEnumerable<ThrownExceptionModel> ThrownExceptionModelsNotCatched
        {
            get
            {
                foreach (var throwStatementModel in base.ThrownExceptionModelsNotCatched)
                {
                    yield return throwStatementModel;
                }

                for (var i = 0; i < CatchClauseModels.Count; i++)
                {
                    IBlockModel catchClauseModel = CatchClauseModels[i];
                    foreach (var model in catchClauseModel.ThrownExceptionModelsNotCatched)
                    {
                        yield return model;
                    }
                }
            }
        }

        public override TryStatementModel FindNearestTryBlock()
        {
            return this;
        }

        public override IBlock Contents
        {
            get { return Node.Try; }
        }

        public override void Accept(AnalyzerBase analyzerBase)
        {
            base.Accept(analyzerBase);

            foreach (var catchClauseModel in CatchClauseModels)
            {
                catchClauseModel.Accept(analyzerBase);
            }
        }

        public void AddCatchClause(IDeclaredType exceptionType)
        {
            var codeElementFactory = new CodeElementFactory(GetElementFactory());
            var variableName = NameFactory.CatchVariableName(Node, exceptionType);
            var catchClauseNode = codeElementFactory.CreateSpecificCatchClause(exceptionType, null, variableName);
            Node.AddCatchClause(catchClauseNode);
        }
    }
}