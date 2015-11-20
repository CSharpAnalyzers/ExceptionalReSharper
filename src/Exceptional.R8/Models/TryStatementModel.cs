using System.Collections.Generic;
using System.Linq;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using ReSharper.Exceptional.Analyzers;
using ReSharper.Exceptional.Utilities;

namespace ReSharper.Exceptional.Models
{
    /// <summary>Describes a try statement. </summary>
    internal class TryStatementModel : BlockModelBase<ITryStatement>
    {
        public TryStatementModel(IAnalyzeUnit analyzeUnit, ITryStatement tryStatement)
            : base(analyzeUnit, tryStatement)
        {
            CatchClauses = GetCatchClauses();
        }

        /// <summary>Gets the list of catch clauses of the try statement. </summary>
        public List<CatchClauseModel> CatchClauses { get; private set; }

        /// <summary>Gets the list of not caught thrown exceptions. </summary>
        public override IEnumerable<ThrownExceptionModel> UncaughtThrownExceptions
        {
            get
            {
                foreach (var throwStatementModel in base.UncaughtThrownExceptions)
                    yield return throwStatementModel;

                foreach (var model in CatchClauses.SelectMany(m => m.UncaughtThrownExceptions))
                    yield return model;
            }
        }

        /// <summary>Gets the content block of the object. </summary>
        public override IBlock Content
        {
            get { return Node.Try; }
        }

        /// <summary>Checks whether the block catches the given exception. </summary>
        /// <param name="exception">The exception. </param>
        /// <returns><c>true</c> if the exception is caught in the block; otherwise, <c>false</c>. </returns>
        public override bool CatchesException(IDeclaredType exception)
        {
            if (CatchClauses.Any(catchClauseModel => catchClauseModel.Catches(exception)))
                return true;

            return ParentBlock.CatchesException(exception);
        }

        /// <summary>Finds the nearest parent try statement which encloses this block. </summary>
        /// <returns>The try statement. </returns>
        public override TryStatementModel FindNearestTryStatement()
        {
            return this;
        }

        /// <summary>Analyzes the object and its children. </summary>
        /// <param name="analyzer">The analyzer base. </param>
        public override void Accept(AnalyzerBase analyzer)
        {
            base.Accept(analyzer);
            foreach (var catchClauseModel in CatchClauses)
                catchClauseModel.Accept(analyzer);
        }

        /// <summary>Adds a catch clause to the try statement. </summary>
        /// <param name="exceptionType">The exception type in the added catch clause. </param>
        public void AddCatchClause(IDeclaredType exceptionType)
        {
            var codeElementFactory = new CodeElementFactory(GetElementFactory());
            var variableName = NameFactory.CatchVariableName(Node, exceptionType);
            var catchClauseNode = codeElementFactory.CreateSpecificCatchClause(exceptionType, null, variableName);

            Node.AddCatchClause(catchClauseNode);
        }

        private List<CatchClauseModel> GetCatchClauses()
        {
            return Node.Catches
                .Select(c => new CatchClauseModel(c, this, AnalyzeUnit))
                .ToList();
        }
    }
}