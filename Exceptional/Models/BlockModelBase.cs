using System.Collections.Generic;
using System.Linq;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;
using ReSharper.Exceptional.Analyzers;

namespace ReSharper.Exceptional.Models
{
    internal abstract class BlockModelBase<T> : TreeElementModelBase<T>, IBlockModel where T : ITreeNode
    {
        public List<TryStatementModel> TryStatementModels { get; private set; }
        public List<IExceptionsOriginModel> ExceptionOriginModels { get; private set; }
        public IBlockModel ParentBlock { get; set; }

        protected BlockModelBase(IAnalyzeUnit analyzeUnit, T node)
            : base(analyzeUnit, node)
        {
            TryStatementModels = new List<TryStatementModel>();
            ExceptionOriginModels = new List<IExceptionsOriginModel>();
        }

        public abstract IBlock Contents { get; }

        public virtual bool CatchesException(IDeclaredType exception)
        {
            return false;
        }

        public virtual IDeclaredType GetCaughtException()
        {
            return null;
        }

        public override void Accept(AnalyzerBase analyzerBase)
        {
            foreach (var tryStatementModel in TryStatementModels)
                tryStatementModel.Accept(analyzerBase);

            foreach (var throwStatementModel in ExceptionOriginModels)
                throwStatementModel.Accept(analyzerBase);
        }

        public virtual IEnumerable<ThrownExceptionModel> ThrownExceptionModelsNotCaught
        {
            get
            {
                foreach (var throwStatementModel in ExceptionOriginModels)
                {
                    foreach (var thrownExceptionModel in throwStatementModel.ThrownExceptions.Where(m => m.IsCaught == false))
                        yield return thrownExceptionModel;
                }

                foreach (var model in TryStatementModels.SelectMany(m => m.ThrownExceptionModelsNotCaught))
                    yield return model;
            }
        }

        public virtual TryStatementModel FindNearestTryBlock()
        {
            if (ParentBlock == null)
                return null;

            return ParentBlock.FindNearestTryBlock();
        }
    }
}