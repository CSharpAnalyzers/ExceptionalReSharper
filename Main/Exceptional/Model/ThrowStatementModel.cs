/// <copyright file="ThrowStatementModel.cs" manufacturer="CodeGears">
///   Copyright (c) CodeGears. All rights reserved.
/// </copyright>

using System.Collections.Generic;
using CodeGears.ReSharper.Exceptional.Analyzers;
using JetBrains.DocumentModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.Util;

namespace CodeGears.ReSharper.Exceptional.Model
{
    internal class ThrowStatementModel : ModelBase, IExceptionsOrigin
    {
        private ThrownExceptionModel ThrownExceptionModel { get; set; }
        private IThrowStatement ThrowStatement { get; set; }

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

        public TextRange[] AddInnerException(string variableName)
        {
            var ranges = new List<TextRange>();

            var objectCreationExpressionNode = this.ThrowStatement.Exception as IObjectCreationExpressionNode;
            if (objectCreationExpressionNode == null) return new TextRange[0];

            if (objectCreationExpressionNode.Arguments.Count == 0)
            {
                var messageExpression = CSharpElementFactory.GetInstance(this.GetPsiModule()).CreateExpressionAsIs(
                    "\"See inner exception for details.\"");

                var messageArgument = CSharpElementFactory.GetInstance(this.GetPsiModule()).CreateArgument(ParameterKind.VALUE, messageExpression);
                
                messageArgument = objectCreationExpressionNode.AddArgumentAfter(messageArgument, null);
                ranges.Add(messageArgument.GetDocumentRange().TextRange);
            }

            if (objectCreationExpressionNode.Arguments.Count == 1)
            {
                var messageArgument = objectCreationExpressionNode.ArgumentList.Arguments[0];

                var innerExceptionExpression = CSharpElementFactory.GetInstance(this.GetPsiModule()).CreateExpressionAsIs(variableName);
                var innerExpressionArgument = CSharpElementFactory.GetInstance(this.GetPsiModule()).CreateArgument(ParameterKind.VALUE, innerExceptionExpression);

                innerExpressionArgument = objectCreationExpressionNode.AddArgumentAfter(innerExpressionArgument, messageArgument);
                ranges.Add(innerExpressionArgument.GetDocumentRange().TextRange);
            }

            return ranges.ToArray();
        }

        public bool HasInnerException(string variableName)
        {
            var objectCreationExpressionNode = this.ThrowStatement.Exception as IObjectCreationExpressionNode;
            if (objectCreationExpressionNode == null) return false;
            if (objectCreationExpressionNode.Arguments.Count < 2) return false;

            var secondArgument = objectCreationExpressionNode.Arguments[1];
            return secondArgument.GetText().Equals(variableName);

        }
    }
}