using System.Collections.Generic;
using CodeGears.ReSharper.Exceptional.Analyzers;
using JetBrains.DocumentModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Tree;

namespace CodeGears.ReSharper.Exceptional.Model
{
    internal class ThrowStatementModel : ModelBase, IExceptionsOrigin
    {
        public ThrownExceptionModel ThrownExceptionModel { get; private set; }
        public IThrowStatement ThrowStatement { get; private set; }

        public override DocumentRange DocumentRange
        {
            get
            {
                var node = this.ThrowStatement.ToTreeNode();

                if(node.ExceptionNode != null)
                {
                    return node.ExceptionNode.GetDocumentRange();
                }

                return node.ThrowKeyword.GetDocumentRange();
            }
        }

        public bool IsRethrow
        {
            get { return this.ThrowStatement.Exception == null; }
        }

        public ThrowStatementModel(MethodDeclarationModel methodDeclarationModel, IThrowStatement throwStatement, IBlockModel containingBlockModel) : base(methodDeclarationModel)
        {
            ThrowStatement = throwStatement;
            ContainingBlockModel = containingBlockModel;

            ThrownExceptionModel = new ThrownExceptionModel(methodDeclarationModel, GetExceptionType(), this);

            containingBlockModel.ThrowStatementModels.Add(this);
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

            this.ThrownExceptionModel.Accept(analyzerBase);
        }

        public List<ThrownExceptionModel> ThrownExceptions
        {
            get { return new List<ThrownExceptionModel>(new[] { ThrownExceptionModel }); }
        }

        public IBlockModel ContainingBlockModel { get; private set; }

        /// <summary>Checks whether this throw statement throws given <paramref name="exceptionType"/>.</summary>
        public bool Throws(IDeclaredType exceptionType)
        {
            return this.ThrownExceptionModel.Throws(exceptionType);
        }
    }
}