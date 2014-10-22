using System.Collections.Generic;
using JetBrains.DocumentModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.Util;
using ReSharper.Exceptional.Analyzers;
using ReSharper.Exceptional.Utilities;

namespace ReSharper.Exceptional.Models
{
    internal class ThrowStatementModel : TreeElementModelBase<IThrowStatement>, IExceptionsOriginModel
    {
        private readonly ThrownExceptionModel _thrownException;

        /// <summary>Initializes a new instance of the <see cref="ThrowStatementModel"/> class. </summary>
        /// <param name="analyzeUnit">The analyze unit.</param>
        /// <param name="throwStatement">The throw statement.</param>
        /// <param name="containingBlock">The containing block.</param>
        public ThrowStatementModel(IAnalyzeUnit analyzeUnit, IThrowStatement throwStatement, IBlockModel containingBlock)
            : base(analyzeUnit, throwStatement)
        {
            ContainingBlock = containingBlock;

            var exceptionType = GetExceptionType();
            var exceptionDescription = GetThrownExceptionMessage(throwStatement);

            _thrownException = new ThrownExceptionModel(analyzeUnit, this, exceptionType, exceptionDescription);
        }

        /// <summary>Gets the parent block which contains this block. </summary>
        public IBlockModel ContainingBlock { get; private set; }

        /// <summary>Gets the document range of the throw statement which is highlighted. </summary>
        public override DocumentRange DocumentRange
        {
            get
            {
                // if we have exceptiontype then highlight the type
                if (Node.Exception != null)
                    return Node.Exception.GetDocumentRange();

                // otherwise highlight the throw keyword
                return Node.ThrowKeyword.GetDocumentRange();
            }
        }

        /// <summary>Gets a value indicating whether the throw statement is a rethrow statement. </summary>
        /// <value><c>true</c> if this instance is rethrow; otherwise, <c>false</c>. </value>
        public bool IsRethrow
        {
            get { return Node.Exception == null; }
        }

        /// <summary>Searches for the nearest containing catch clause. </summary>
        /// <returns></returns>
        public CatchClauseModel FindOuterCatchClause()
        {
            var outerBlock = ContainingBlock;

            while (outerBlock != null && (outerBlock is CatchClauseModel) == false)
                outerBlock = outerBlock.ParentBlock;

            return outerBlock as CatchClauseModel;
        }

        public override void Accept(AnalyzerBase analyzerBase)
        {
            analyzerBase.Visit(this);
            _thrownException.Accept(analyzerBase);
        }

        public bool SurroundWithTryBlock(IDeclaredType exceptionType)
        {
            var codeElementFactory = new CodeElementFactory(GetElementFactory());
            var exceptionVariableName = NameFactory.CatchVariableName(Node, exceptionType);
            var tryStatement = codeElementFactory.CreateTryStatement(exceptionType, exceptionVariableName);
            var block = codeElementFactory.CreateBlock(Node);
            
            tryStatement.SetTry(block);
            Node.ReplaceBy(tryStatement);

            return true; 
        }
        
        public IEnumerable<ThrownExceptionModel> ThrownExceptions
        {
            get { return new List<ThrownExceptionModel>(new[] { _thrownException }); }
        }

        /// <summary>Checks whether this throw statement throws given <paramref name="exceptionType"/>.</summary>
        public bool Throws(IDeclaredType exceptionType)
        {
            return _thrownException.Throws(exceptionType);
        }

        public TextRange[] AddInnerException(string variableName)
        {
            var ranges = new List<TextRange>();

            var objectCreationExpressionNode = Node.Exception as IObjectCreationExpression;
            if (objectCreationExpressionNode == null)
                return new TextRange[0];

            if (objectCreationExpressionNode.Arguments.Count == 0)
            {
                var messageExpression = CSharpElementFactory.GetInstance(AnalyzeUnit.GetPsiModule())
                    .CreateExpressionAsIs("\"See the inner exception for details.\"");

                var messageArgument = CSharpElementFactory.GetInstance(AnalyzeUnit.GetPsiModule())
                    .CreateArgument(ParameterKind.VALUE, messageExpression);

                messageArgument = objectCreationExpressionNode.AddArgumentAfter(messageArgument, null);
                ranges.Add(messageArgument.GetDocumentRange().TextRange);
            }

            if (objectCreationExpressionNode.Arguments.Count == 1)
            {
                var messageArgument = objectCreationExpressionNode.ArgumentList.Arguments[0];

                var innerExceptionExpression = CSharpElementFactory.GetInstance(AnalyzeUnit.GetPsiModule())
                    .CreateExpressionAsIs(variableName);

                var innerExpressionArgument = CSharpElementFactory.GetInstance(AnalyzeUnit.GetPsiModule())
                    .CreateArgument(ParameterKind.VALUE, innerExceptionExpression);

                innerExpressionArgument = objectCreationExpressionNode.AddArgumentAfter(innerExpressionArgument, messageArgument);
                ranges.Add(innerExpressionArgument.GetDocumentRange().TextRange);
            }

            return ranges.ToArray();
        }

        public bool HasInnerException(string variableName)
        {
            var objectCreationExpressionNode = Node.Exception as IObjectCreationExpression;
            if (objectCreationExpressionNode == null)
                return false;

            if (objectCreationExpressionNode.Arguments.Count < 2)
                return false;

            var secondArgument = objectCreationExpressionNode.Arguments[1];
            return secondArgument.GetText().Equals(variableName);
        }

        private IDeclaredType GetExceptionType()
        {
            if (Node.Exception != null)
                return Node.Exception.GetExpressionType() as IDeclaredType;
            return null;
        }

        private static string GetThrownExceptionMessage(IThrowStatement throwStatement)
        {
            if (throwStatement.Exception is IObjectCreationExpression)
            {
                var arguments = ((IObjectCreationExpression)throwStatement.Exception).Arguments;
                if (arguments.Count > 0)
                {
                    var literal = arguments[0].Value as ICSharpLiteralExpression;
                    if (literal != null && literal.Literal != null)
                    {
                        var exp = literal.Literal.Parent as ICSharpLiteralExpression;
                        if (exp != null && exp.ConstantValue.Value != null)
                        {
                            return exp.ConstantValue.Value.ToString();
                        }
                    }
                }
            }
            return string.Empty;
        }
    }
}