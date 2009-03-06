using System.Collections.Generic;
using CodeGears.ReSharper.Exceptional.Analyzers;
using JetBrains.ReSharper.Daemon.CSharp.Stages;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Tree;

namespace CodeGears.ReSharper.Exceptional.Model
{
    internal class TryStatementModel : ModelBase, IBlockModel
    {
        public ITryStatement TryStatement { get; private set; }
        
        public List<CatchClauseModel> CatchClauseModels { get; private set; }
        public List<ThrowStatementModel> ThrowStatementModels { get; private set; }
        public List<TryStatementModel> TryStatementModels { get; private set; }
        public IBlockModel ParentBlock { get; set; }

        public TryStatementModel(MethodDeclarationModel methodDeclarationModel, ITryStatement tryStatement)
            : base(methodDeclarationModel)
        {
            TryStatement = tryStatement;
            CatchClauseModels = new List<CatchClauseModel>();
            ThrowStatementModels = new List<ThrowStatementModel>();
            TryStatementModels = new List<TryStatementModel>();
        }

        public bool CatchesException(IDeclaredType exception)
        {
            foreach (var catchClauseModel in this.CatchClauseModels)
            {
                if(catchClauseModel.Catches(exception))
                {
                    return true;
                }
            }

            return this.ParentBlock.CatchesException(exception);
        }

        public IDeclaredType GetCatchedException()
        {
            return this.ParentBlock.GetCatchedException();
        }

        public override void Accept(AnalyzerBase analyzerBase)
        {
            analyzerBase.Visit(this);

            foreach (var innerTryStatementModel in this.TryStatementModels)
            {
                innerTryStatementModel.Accept(analyzerBase);
            }

            foreach (var catchClauseModel in this.CatchClauseModels)
            {
                catchClauseModel.Accept(analyzerBase);
            }

            foreach (var throwStatementModel in this.ThrowStatementModels)
            {
                throwStatementModel.Accept(analyzerBase);
            }
        }

        public override void AssignHighlights(CSharpDaemonStageProcessBase process)
        {
            foreach (var innerTryStatementModel in this.TryStatementModels)
            {
                innerTryStatementModel.AssignHighlights(process);
            }

            foreach (var catchClauseModel in this.CatchClauseModels)
            {
                catchClauseModel.AssignHighlights(process);
            }

            foreach (var throwStatementModel in this.ThrowStatementModels)
            {
                throwStatementModel.AssignHighlights(process);
            }
        }
    }
}