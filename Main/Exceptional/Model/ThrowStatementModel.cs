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
    internal class ThrowStatementModel : TreeElementModelBase<IThrowStatementNode>, IExceptionsOrigin
    {
        private ThrownExceptionModel ThrownExceptionModel { get; set; }

        public override DocumentRange DocumentRange
        {
            get
            {
                //if we have exceptiontype then highlight the type
                if(this.Node.ExceptionNode != null)
                {
                    return this.Node.ExceptionNode.GetDocumentRange();
                }

                //if not highlight the throw keyword
                return this.Node.ThrowKeyword.GetDocumentRange();
            }
        }

        /// <summary>Specifies if this throw statement is a rethrow statement.</summary>
        public bool IsRethrow
        {
            get { return this.Node.Exception == null; }
        }

        public ThrowStatementModel(IAnalyzeUnit analyzeUnit, IThrowStatementNode throwStatement, IBlockModel containingBlockModel) 
            : base(analyzeUnit, throwStatement)
        {
            ContainingBlockModel = containingBlockModel;

            ThrownExceptionModel = new ThrownExceptionModel(analyzeUnit, GetExceptionType(), this);

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
            if (this.Node.Exception != null)
            {
                //var creation = this.Node.ExceptionNode as IObjectCreationExpressionNode;
                //creation.Reference.GetName();
                return this.Node.Exception.GetExpressionType() as IDeclaredType;
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

            var objectCreationExpressionNode = this.Node.Exception as IObjectCreationExpressionNode;
            if (objectCreationExpressionNode == null) return new TextRange[0];

            if (objectCreationExpressionNode.Arguments.Count == 0)
            {
                var messageExpression = CSharpElementFactory.GetInstance(this.AnalyzeUnit.GetPsiModule()).CreateExpressionAsIs(
                    "\"See inner exception for details.\"");

                var messageArgument = CSharpElementFactory.GetInstance(this.AnalyzeUnit.GetPsiModule()).CreateArgument(ParameterKind.VALUE, messageExpression);
                
                messageArgument = objectCreationExpressionNode.AddArgumentAfter(messageArgument, null);
                ranges.Add(messageArgument.GetDocumentRange().TextRange);
            }

            if (objectCreationExpressionNode.Arguments.Count == 1)
            {
                var messageArgument = objectCreationExpressionNode.ArgumentList.Arguments[0];

                var innerExceptionExpression = CSharpElementFactory.GetInstance(this.AnalyzeUnit.GetPsiModule()).CreateExpressionAsIs(variableName);
                var innerExpressionArgument = CSharpElementFactory.GetInstance(this.AnalyzeUnit.GetPsiModule()).CreateArgument(ParameterKind.VALUE, innerExceptionExpression);

                innerExpressionArgument = objectCreationExpressionNode.AddArgumentAfter(innerExpressionArgument, messageArgument);
                ranges.Add(innerExpressionArgument.GetDocumentRange().TextRange);
            }

            return ranges.ToArray();
        }

        public bool HasInnerException(string variableName)
        {
            var objectCreationExpressionNode = this.Node.Exception as IObjectCreationExpressionNode;
            if (objectCreationExpressionNode == null) return false;
            if (objectCreationExpressionNode.Arguments.Count < 2) return false;

            var secondArgument = objectCreationExpressionNode.Arguments[1];
            return secondArgument.GetText().Equals(variableName);

        }
    }
}