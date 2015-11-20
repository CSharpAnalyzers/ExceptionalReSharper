using System.Collections.Generic;
using System.Linq;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;
using ReSharper.Exceptional.Analyzers;
using ReSharper.Exceptional.Models.ExceptionsOrigins;

namespace ReSharper.Exceptional.Models
{
    internal abstract class BlockModelBase<T> : TreeElementModelBase<T>, IBlockModel where T : ITreeNode
    {
        protected BlockModelBase(IAnalyzeUnit analyzeUnit, T node)
            : base(analyzeUnit, node)
        {
            TryStatements = new List<TryStatementModel>();
            ThrownExceptions = new List<IExceptionsOriginModel>();
        }

        /// <summary>Gets the try statements defined in the block. </summary>
        public List<TryStatementModel> TryStatements { get; private set; }

        /// <summary>Gets the list of exception which can be thrown from this block. </summary>
        public List<IExceptionsOriginModel> ThrownExceptions { get; private set; }

        /// <summary>Gets the parent block. </summary>
        public IBlockModel ParentBlock { get; set; }

        /// <summary>Gets the content block of the object. </summary>
        public abstract IBlock Content { get; }

        /// <summary>Gets the list of not caught thrown exceptions. </summary>
        public virtual IEnumerable<ThrownExceptionModel> UncaughtThrownExceptions
        {
            get
            {
                foreach (var thrownException in ThrownExceptions)
                {
                    foreach (var thrownExceptionModel in thrownException.ThrownExceptions.Where(m => m.IsCaught == false))
                        yield return thrownExceptionModel;
                }

                foreach (var model in TryStatements.SelectMany(m => m.UncaughtThrownExceptions))
                    yield return model;
            }
        }

        /// <summary>Checks whether the block catches the given exception. </summary>
        /// <param name="exception">The exception. </param>
        /// <returns><c>true</c> if the exception is caught in the block; otherwise, <c>false</c>. </returns>
        public virtual bool CatchesException(IDeclaredType exception)
        {
            return false;
        }

        /// <summary>Analyzes the object and its children. </summary>
        /// <param name="analyzer">The analyzer. </param>
        public override void Accept(AnalyzerBase analyzer)
        {
            foreach (var tryStatement in TryStatements)
                tryStatement.Accept(analyzer);

            foreach (var thrownException in ThrownExceptions)
                thrownException.Accept(analyzer);
        }

        /// <summary>Finds the nearest parent try statement which encloses this block. </summary>
        /// <returns>The try statement. </returns>
        public virtual TryStatementModel FindNearestTryStatement()
        {
            if (ParentBlock == null)
                return null;

            return ParentBlock.FindNearestTryStatement();
        }
    }
}