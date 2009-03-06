using CodeGears.ReSharper.Exceptional.Analyzers;
using CodeGears.ReSharper.Exceptional.Highlightings;
using JetBrains.DocumentModel;
using JetBrains.ReSharper.Daemon.CSharp.Stages;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Tree;

namespace CodeGears.ReSharper.Exceptional.Model
{
    internal class ThrowStatementModel : ModelBase
    {
        public bool IsCatched { get; set; }
        public bool IsDocumented { get; set; }
        public bool ThrowsWithInnerException { get; set; }

        public IThrowStatement ThrowStatement { get; private set; }
        public IBlockModel ContainingBlockModel { get; private set; }
        public IDeclaredType ExceptionType { get; private set; }
        public ExceptionCreationModel ExceptionCreationModel { get; private set; }

        private DocumentRange DocumentRange
        {
            get { return this.ThrowStatement.GetDocumentRange(); }
        }

        public bool IsRethrow
        {
            get { return this.ThrowStatement.Exception == null; }
        }

        public ThrowStatementModel(MethodDeclarationModel methodDeclarationModel, IThrowStatement throwStatement, IBlockModel containingBlockModel) : base(methodDeclarationModel)
        {
            ThrowStatement = throwStatement;
            ContainingBlockModel = containingBlockModel;
            IsCatched = false;
            IsDocumented = false;
            ThrowsWithInnerException = true;

            containingBlockModel.ThrowStatementModels.Add(this);

            ExceptionCreationModel = new ExceptionCreationModel(methodDeclarationModel, this.ThrowStatement.Exception as IObjectCreationExpression);
            ExceptionType = GetExceptionType();
        }

        /// <summary>Checks whether this throw statement throws given <paramref name="exceptionType"/>.</summary>
        public bool Throws(IDeclaredType exceptionType)
        {
            if (this.ExceptionType == null) return false;
            if (exceptionType == null) return false;

            return this.ExceptionType.GetCLRName().Equals(exceptionType.GetCLRName());
        }

        /// <summary>Searches for the nearest containing catch clause.</summary>
        public CatchClauseModel FindOuterCatchClause()
        {
            var outerBlock = this.ContainingBlockModel;

            while(outerBlock != null && (outerBlock is CatchClauseModel) == false)
            {
                outerBlock = outerBlock.ParentBlock;
            }

            return outerBlock as CatchClauseModel;
        }

        private IDeclaredType GetExceptionType()
        {
            if (this.ThrowStatement.Exception != null)
            {
                return this.ThrowStatement.Exception.GetExpressionType() as IDeclaredType;
            }

            return this.ContainingBlockModel.GetCatchedException();
        }

        #region Assigns & Accepts

        public override void AssignHighlights(CSharpDaemonStageProcessBase process)
        {
            if (this.IsCatched == false && this.IsDocumented == false)
            {
                process.AddHighlighting(this.DocumentRange, new ExceptionNotDocumentedHighlighting(this));
            }

            if (this.ThrowsWithInnerException == false)
            {
                process.AddHighlighting(this.DocumentRange,
                    new ThrowFromCatchWithNoInnerExceptionHighlighting(this));
            }
        }

        public override void Accept(AnalyzerBase analyzerBase)
        {
            analyzerBase.Visit(this);
        }
        #endregion
    }
}