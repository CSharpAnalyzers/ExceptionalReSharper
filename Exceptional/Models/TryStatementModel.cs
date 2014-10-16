using System.Collections.Generic;
using System.Linq;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using ReSharper.Exceptional.Analyzers;
using ReSharper.Exceptional.Factories;

namespace ReSharper.Exceptional.Models
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
            return Node.Catches
                .Select(c => new CatchClauseModel(c, this, AnalyzeUnit))
                .ToList();
        }

        public override bool CatchesException(IDeclaredType exception)
        {
            if (CatchClauseModels.Any(catchClauseModel => catchClauseModel.Catches(exception)))
                return true;

            return ParentBlock.CatchesException(exception);
        }

        public override IDeclaredType GetCaughtException()
        {
            return ParentBlock.GetCaughtException();
        }

        public override IEnumerable<ThrownExceptionModel> ThrownExceptionModelsNotCaught
        {
            get
            {
                foreach (var throwStatementModel in base.ThrownExceptionModelsNotCaught)
                    yield return throwStatementModel;

                foreach (var model in CatchClauseModels.SelectMany(m => m.ThrownExceptionModelsNotCaught))
                    yield return model;
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