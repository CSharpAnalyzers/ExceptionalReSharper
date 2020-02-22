using System.Collections.Generic;
using System.Linq;
using JetBrains.DocumentModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.Util;
using ReSharper.Exceptional.Analyzers;
using ReSharper.Exceptional.Utilities;

namespace ReSharper.Exceptional.Models.ExceptionsOrigins
{
    internal class ThrowExpressionModel : TreeElementModelBase<IThrowExpression>, IExceptionsOriginModel
    {
        private readonly ThrownExceptionModel _thrownException;

        /// <summary>Initializes a new instance of the <see cref="ThrowExpressionModel"/> class. </summary>
        /// <param name="analyzeUnit">The analyze unit.</param>
        /// <param name="throwExpression">The throw expression.</param>
        /// <param name="containingBlock">The containing block.</param>
        public ThrowExpressionModel(IAnalyzeUnit analyzeUnit, IThrowExpression throwExpression, IBlockModel containingBlock)
            : base(analyzeUnit, throwExpression)
        {
            ContainingBlock = containingBlock;

            var exceptionType = GetExceptionType();
            var exceptionDescription = GetThrownExceptionMessage(throwExpression);

            string accessor = null;
            if (containingBlock is AccessorDeclarationModel)
                accessor = ((AccessorDeclarationModel)containingBlock).Node.NameIdentifier.Name;

            _thrownException = new ThrownExceptionModel(analyzeUnit, this, exceptionType, exceptionDescription, false, accessor);
        }

        /// <summary>Gets the parent block which contains this block. </summary>
        public IBlockModel ContainingBlock { get; private set; }

        /// <summary>Gets the document range of the throw expression which is highlighted. </summary>
        public override DocumentRange DocumentRange
        {
            get
            {
                // if we have exception type then highlight the type
                if (Node.Exception != null)
                    return Node.Exception.GetDocumentRange();

                // otherwise highlight the throw keyword
                return Node.ThrowKeyword.GetDocumentRange();
            }
        }

        /// <summary>Gets a value indicating whether the throw statement is a rethrow statement. </summary>
        public bool IsRethrow
        {
            get { return Node.Exception == null; }
        }

        /// <summary>Gets the node. </summary>
        ITreeNode IExceptionsOriginModel.Node
        {
            get { return Node; }
        }

        /// <summary>Searches for the nearest containing catch clause. </summary>
        /// <returns>The catch clause. </returns>
        public CatchClauseModel FindOuterCatchClause()
        {
            var outerBlock = ContainingBlock;

            while (outerBlock != null && (outerBlock is CatchClauseModel) == false)
                outerBlock = outerBlock.ParentBlock;

            return outerBlock as CatchClauseModel;
        }

        /// <summary>Runs the analyzer against all defined elements. </summary>
        /// <param name="analyzer">The analyzer. </param>
        public override void Accept(AnalyzerBase analyzer)
        {
            analyzer.Visit(this);
            _thrownException.Accept(analyzer);
        }

        /// <summary>Creates a try-catch block around this block. </summary>
        /// <param name="exceptionType">The exception type to catch. </param>
        /// <returns><c>true</c> if the try-catch block could be created; otherwise, <c>false</c>. </returns>
        public bool SurroundWithTryBlock(IDeclaredType exceptionType)
        {
            var codeElementFactory = new CodeElementFactory(GetElementFactory());
            var exceptionVariableName = NameFactory.CatchVariableName(Node, exceptionType);
            var tryStatement = codeElementFactory.CreateTryStatement(exceptionType, exceptionVariableName, Node);
            var block = codeElementFactory.CreateBlock(Node);

            tryStatement.SetTry(block);
            Node.GetContainingStatement().ReplaceBy(tryStatement);

            return true;
        }

        /// <summary>Gets the list of exception which can be thrown from this object. </summary>
        public IEnumerable<ThrownExceptionModel> ThrownExceptions
        {
            get { return new List<ThrownExceptionModel>(new[] { _thrownException }); }
        }

        /// <summary>Checks whether this throw statement throws given <paramref name="exceptionType"/>.</summary>
        public bool Throws(IDeclaredType exceptionType)
        {
            return _thrownException.IsException(exceptionType);
        }
        public TextRange[] AddInnerException(string variableName)
        {
            var ranges = new List<TextRange>();

            var objectCreationExpressionNode = Node.Exception as IObjectCreationExpression;
            if (objectCreationExpressionNode == null)
                return new TextRange[0];

            if (objectCreationExpressionNode.Arguments.Count == 0)
            {
                var messageExpression = CSharpElementFactory.GetInstance(AnalyzeUnit.Node)
                    .CreateExpressionAsIs("\"See the inner exception for details.\"");

                var messageArgument = CSharpElementFactory.GetInstance(AnalyzeUnit.Node)
                    .CreateArgument(ParameterKind.VALUE, messageExpression);

                messageArgument = objectCreationExpressionNode.AddArgumentAfter(messageArgument, null);
                ranges.Add(messageArgument.GetDocumentRange().TextRange);
            }
            else
            {
                var lastArgument = objectCreationExpressionNode.ArgumentList.Arguments.Last();

                var innerExceptionExpression = CSharpElementFactory.GetInstance(AnalyzeUnit.Node)
                    .CreateExpressionAsIs(variableName);

                var innerExpressionArgument = CSharpElementFactory.GetInstance(AnalyzeUnit.Node)
                    .CreateArgument(ParameterKind.VALUE, innerExceptionExpression);

                innerExpressionArgument = objectCreationExpressionNode.AddArgumentAfter(innerExpressionArgument, lastArgument);
                ranges.Add(innerExpressionArgument.GetDocumentRange().TextRange);
            }

            return ranges.ToArray();
        }

        public bool IsDirectExceptionInstantiation
        {
            get { return Node.Exception is IObjectCreationExpression; }
        }

        public bool IsInnerExceptionPassed(string variableName)
        {
            var objectCreationExpressionNode = Node.Exception as IObjectCreationExpression;
            if (objectCreationExpressionNode == null)
                return false;

            if (objectCreationExpressionNode.Arguments.Count < 1)
                return false;

            return objectCreationExpressionNode.Arguments
                .Any(a => a.GetText().Equals(variableName));
        }

        private IDeclaredType GetExceptionType()
        {
            if (Node.Exception != null)
                return Node.Exception.GetExpressionType() as IDeclaredType;
            return FindOuterCatchClause().CaughtException; // Node.Exception == null when this is a "throw;" statement
        }

        private static string GetThrownExceptionMessage(IThrowExpression throwStatement)
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
