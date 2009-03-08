using CodeGears.ReSharper.Exceptional.Analyzers;
using JetBrains.DocumentModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Tree;

namespace CodeGears.ReSharper.Exceptional.Model
{
    internal class ThrowStatementModel : ModelBase
    {
        public bool IsCatched { get; private set; }
        public bool IsDocumented { get; private set; }
        public IThrowStatement ThrowStatement { get; private set; }
        public IBlockModel ContainingBlockModel { get; private set; }
        public IDeclaredType ExceptionType { get; private set; }
        public ExceptionCreationModel ExceptionCreationModel { get; private set; }

        public override DocumentRange DocumentRange
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

            containingBlockModel.ThrowStatementModels.Add(this);

            ExceptionCreationModel = new ExceptionCreationModel(methodDeclarationModel, this.ThrowStatement.Exception as IObjectCreationExpression);
            ExceptionType = GetExceptionType();
            IsCatched = CheckIfCatched();
            IsDocumented = CheckIfDocumented();
        }

        private bool CheckIfDocumented()
        {
            var docCommentBlockNode = this.MethodDeclarationModel.DocCommentBlockModel;
            if (docCommentBlockNode == null) return false;

            foreach (var exceptionDocumentationModel in docCommentBlockNode.ExceptionDocCommentModels)
            {
                if (this.Throws(exceptionDocumentationModel.ExceptionType))
                {
                    return true;
                }
            }

            return false;
        }

        private bool CheckIfCatched()
        {
            if (ExceptionType == null) return false;

            return ContainingBlockModel.CatchesException(ExceptionType);
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

        public override void Accept(AnalyzerBase analyzerBase)
        {
            analyzerBase.Visit(this);
        }
    }
}