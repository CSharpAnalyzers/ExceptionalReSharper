// Copyright (c) 2009-2010 Cofinite Solutions. All rights reserved.
using System.Collections.Generic;
using CodeGears.ReSharper.Exceptional.Analyzers;
using JetBrains.DocumentModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.Util;

namespace CodeGears.ReSharper.Exceptional.Model
{
    internal class ThrowStatementModel : TreeElementModelBase<IThrowStatement>, IExceptionsOriginModel
    {
        private ThrownExceptionModel ThrownExceptionModel { get; set; }
        public IBlockModel ContainingBlockModel { get; private set; }

        /// <summary>
        /// Gets the document range.
        /// </summary>
        public override DocumentRange DocumentRange
        {
            get
            {
                //if we have exceptiontype then highlight the type
                if (Node.Exception != null)
                {
                    return Node.Exception.GetDocumentRange();
                }

                //if not highlight the throw keyword
                return Node.ThrowKeyword.GetDocumentRange();
            }
        }

        /// <summary>
        /// Specifies if this throw statement is a rethrow statement.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is rethrow; otherwise, <c>false</c>.
        /// </value>
        public bool IsRethrow
        {
            get { return Node.Exception == null; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ThrowStatementModel"/> class.
        /// </summary>
        /// <param name="analyzeUnit">The analyze unit.</param>
        /// <param name="throwStatement">The throw statement.</param>
        /// <param name="containingBlockModel">The containing block model.</param>
        public ThrowStatementModel(IAnalyzeUnit analyzeUnit, IThrowStatement throwStatement,
                                   IBlockModel containingBlockModel)
            : base(analyzeUnit, throwStatement)
        {
            ContainingBlockModel = containingBlockModel;

            ThrownExceptionModel = new ThrownExceptionModel(analyzeUnit, GetExceptionType(), this);

            containingBlockModel.ExceptionOriginModels.Add(this);
        }

        /// <summary>
        /// Searches for the nearest containing catch clause.
        /// </summary>
        /// <returns></returns>
        public CatchClauseModel FindOuterCatchClause()
        {
            var outerBlock = ContainingBlockModel;

            while (outerBlock != null && (outerBlock is CatchClauseModel) == false)
            {
                outerBlock = outerBlock.ParentBlock;
            }

            return outerBlock as CatchClauseModel;
        }

        private IDeclaredType GetExceptionType()
        {
            if (Node.Exception != null)
            {
                //var creation = Node.ExceptionNode as IObjectCreationExpressionNode;
                //creation.Reference.GetName();
                return Node.Exception.GetExpressionType() as IDeclaredType;
            }

            return ContainingBlockModel.GetCatchedException();
        }

        public override void Accept(AnalyzerBase analyzerBase)
        {
            analyzerBase.Visit(this);

            ThrownExceptionModel.Accept(analyzerBase);
        }

        public void SurroundWithTryBlock(IDeclaredType exceptionType)
        {
            var codeElementFactory = new CodeElementFactory(GetElementFactory());
            var exceptionVariableName = NameFactory.CatchVariableName(Node, exceptionType);
            var tryStatement = codeElementFactory.CreateTryStatement(exceptionType, exceptionVariableName);
            var block = codeElementFactory.CreateBlock(Node);
            tryStatement.SetTry(block);
            Node.ReplaceBy(tryStatement);
        }

        public IEnumerable<ThrownExceptionModel> ThrownExceptions
        {
            get { return new List<ThrownExceptionModel>(new[] {ThrownExceptionModel}); }
        }

        /// <summary>Checks whether this throw statement throws given <paramref name="exceptionType"/>.</summary>
        public bool Throws(IDeclaredType exceptionType)
        {
            return ThrownExceptionModel.Throws(exceptionType);
        }

        public TextRange[] AddInnerException(string variableName)
        {
            var ranges = new List<TextRange>();

            var objectCreationExpressionNode = Node.Exception as IObjectCreationExpression;
            if (objectCreationExpressionNode == null)
            {
                return new TextRange[0];
            }

            if (objectCreationExpressionNode.Arguments.Count == 0)
            {
                var messageExpression = CSharpElementFactory.GetInstance(AnalyzeUnit.GetPsiModule(), true, true).
                    CreateExpressionAsIs(
                    "\"See inner exception for details.\"");

                var messageArgument =
					CSharpElementFactory.GetInstance(AnalyzeUnit.GetPsiModule(), true, true).CreateArgument(
                        ParameterKind.VALUE, messageExpression);

                messageArgument = objectCreationExpressionNode.AddArgumentAfter(messageArgument, null);
                ranges.Add(messageArgument.GetDocumentRange().TextRange);
            }

            if (objectCreationExpressionNode.Arguments.Count == 1)
            {
                var messageArgument = objectCreationExpressionNode.ArgumentList.Arguments[0];

                var innerExceptionExpression = CSharpElementFactory.GetInstance(AnalyzeUnit.GetPsiModule()).CreateExpressionAsIs(variableName);
                var innerExpressionArgument = CSharpElementFactory.GetInstance(AnalyzeUnit.GetPsiModule()).CreateArgument(ParameterKind.VALUE, innerExceptionExpression);

                innerExpressionArgument = objectCreationExpressionNode.AddArgumentAfter(innerExpressionArgument, messageArgument);
                ranges.Add(innerExpressionArgument.GetDocumentRange().TextRange);
            }

            return ranges.ToArray();
        }

        public bool HasInnerException(string variableName)
        {
            var objectCreationExpressionNode = Node.Exception as IObjectCreationExpression;
            if (objectCreationExpressionNode == null)
            {
                return false;
            }
            if (objectCreationExpressionNode.Arguments.Count < 2)
            {
                return false;
            }

            var secondArgument = objectCreationExpressionNode.Arguments[1];
            return secondArgument.GetText().Equals(variableName);
        }
    }
}