using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;

namespace ReSharper.Exceptional.Factories
{
    /// <summary>Provides services to create various code elements.</summary>
    public class CodeElementFactory
    {
        private readonly CSharpElementFactory _factory;

        public CodeElementFactory(CSharpElementFactory factory)
        {
            _factory = factory;
        }

        /// <summary>Creates a variable declaration for catch clause.</summary>
        /// <param name="exceptionType">The type of a created variable.</param>
        public ICatchVariableDeclaration CreateCatchVariableDeclarationNode(IDeclaredType exceptionType)
        {
            var tryStatement = _factory.CreateStatement("try {} catch(Exception e) {}") as ITryStatement;
            if (tryStatement == null) 
                return null;

            var catchClause = tryStatement.Catches[0] as ISpecificCatchClause;
            if (catchClause == null) 
                return null;

            var exceptionDeclaration = catchClause.ExceptionDeclaration;
            if (exceptionDeclaration == null) 
                return null;

            if (exceptionType != null)
            {
                var declaredTypeUsageNode = _factory.CreateDeclaredTypeUsageNode(exceptionType);
                exceptionDeclaration.SetDeclaredTypeUsage(declaredTypeUsageNode);
            }

            return exceptionDeclaration;
        }

        /// <summary>Creates an argument that may be passed to invocations.</summary>
        /// <param name="value">A value for an argument.</param>
        public ICSharpArgument CreateArgument(string value)
        {
            var argumentExpression = _factory.CreateExpression(value);
            return _factory.CreateArgument(ParameterKind.VALUE, argumentExpression);
        }

        /// <summary>Creates a specific catch clause with given <paramref name="exceptionType"/> and <paramref name="catchBody"/>.</summary>
        /// <param name="exceptionType">Type of the exception to catch.</param>
        /// <param name="catchBody">Body of the created catch.</param>
        /// <param name="variableName">A name for catch variable.</param>
        public ISpecificCatchClause CreateSpecificCatchClause(IDeclaredType exceptionType, IBlock catchBody, string variableName)
        {
            var tryStatement = _factory.CreateStatement("try {} catch(Exception $0) {}", variableName) as ITryStatement;
            if (tryStatement == null) 
                return null;

            var catchClause = tryStatement.Catches[0] as ISpecificCatchClause;
            if (catchClause == null)
                return null;

            if (exceptionType != null)
            {
                var exceptionDeclaration = catchClause.ExceptionDeclaration;
                if (exceptionDeclaration == null) 
                    return null;

                var declaredTypeUsageNode = _factory.CreateDeclaredTypeUsageNode(exceptionType);
                exceptionDeclaration.SetDeclaredTypeUsage(declaredTypeUsageNode);
            }

            if (catchBody != null)
            {
                var catchWithBody = catchClause as ICatchClause;
                catchWithBody.SetBody(catchBody);
            }

            return catchClause;
        }

        /// <summary>Creates a try statement for the given exception type and variable name. </summary>
        /// <param name="exceptionType">The exception type. </param>
        /// <param name="exceptionVariableName">The exception variable name. </param>
        /// <returns>The try statement. </returns>
        public ITryStatement CreateTryStatement(IDeclaredType exceptionType, string exceptionVariableName)
        {
            var tryStatement = _factory.CreateStatement("try {} catch($0 $1) {}", 
                exceptionType.GetClrName().ShortName, exceptionVariableName) as ITryStatement;
            if (tryStatement == null) 
                return null;

            var catchClause = tryStatement.Catches[0] as ISpecificCatchClause;
            if (catchClause == null) 
                return tryStatement;

            var exceptionDeclaration = catchClause.ExceptionDeclaration;
            if (exceptionDeclaration == null) 
                return tryStatement;

            var declaredTypeUsageNode = _factory.CreateDeclaredTypeUsageNode(exceptionType);
            exceptionDeclaration.SetDeclaredTypeUsage(declaredTypeUsageNode);

            return tryStatement;
        }

        /// <summary>Creates a block with the content of the given node. </summary>
        /// <param name="node">The node which is used as content. </param>
        /// <returns>The block. </returns>
        public IBlock CreateBlock(ITreeNode node)
        {
            return _factory.CreateBlock("{ \n$0\n }", node.GetText());
        }
    }
}