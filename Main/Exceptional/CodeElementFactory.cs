/// <copyright>Copyright (c) 2009 CodeGears.net All rights reserved.</copyright>

using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;

namespace CodeGears.ReSharper.Exceptional
{
    /// <summary>Provides services to create various code elements.</summary>
    public class CodeElementFactory
    {
        private CSharpElementFactory Factory { get; set; }

        public CodeElementFactory(CSharpElementFactory factory)
        {
            Factory = factory;
        }

        /// <summary>Creates a variable declaration for catch clause.</summary>
        /// <param name="exceptionType">The type of a created variable.</param>
        public ICatchVariableDeclarationNode CreateCatchVariableDeclarationNode(IDeclaredType exceptionType)
        {
            var tryStatement = this.Factory.CreateStatement("try {} catch(Exception e) {}") as ITryStatement;
            if (tryStatement == null)
            {
                return null;
            }

            var catchClause = tryStatement.Catches[0] as ISpecificCatchClause;
            if (catchClause == null)
            {
                return null;
            }

            var exceptionDeclaration = catchClause.ExceptionDeclaration as ICatchVariableDeclarationNode;
            if (exceptionDeclaration == null)
            {
                return null;
            }

            if (exceptionType != null)
            {
                var declaredTypeUsageNode = this.Factory.CreateDeclaredTypeUsageNode(exceptionType);
                exceptionDeclaration.SetDeclaredTypeUsage(declaredTypeUsageNode);
            }

            return exceptionDeclaration;
        }

        /// <summary>Creates an argument that may be passed to invocations.</summary>
        /// <param name="value">A value for an argument.</param>
        public ICSharpArgument CreateArgument(string value)
        {
            var argumentExpression = this.Factory.CreateExpression(value);
            return this.Factory.CreateArgument(ParameterKind.VALUE, argumentExpression);
        }

        /// <summary>Creates a specific catch clause with given <paramref name="exceptionType"/> and <paramref name="catchBody"/>.</summary>
        /// <param name="exceptionType">Type of the exception to catch.</param>
        /// <param name="catchBody">Body of the created catch.</param>
        /// <param name="variableName">A name for catch variable.</param>
        public ISpecificCatchClauseNode CreateSpecificCatchClause(IDeclaredType exceptionType, IBlock catchBody, string variableName)
        {
            var tryStatement = this.Factory.CreateStatement("try {} catch(Exception $0) {}", variableName) as ITryStatement;
            if (tryStatement == null)
            {
                return null;
            }

            var catchClause = tryStatement.Catches[0] as ISpecificCatchClauseNode;
            if (catchClause == null)
            {
                return null;
            }

            if (exceptionType != null)
            {
                var exceptionDeclaration = catchClause.ExceptionDeclaration as ICatchVariableDeclarationNode;
                if (exceptionDeclaration == null)
                {
                    return null;
                }

                var declaredTypeUsageNode = this.Factory.CreateDeclaredTypeUsageNode(exceptionType);
                exceptionDeclaration.SetDeclaredTypeUsage(declaredTypeUsageNode);
            }

            if (catchBody != null)
            {
                var catchWithBody = catchClause as ICatchClause;
                catchWithBody.SetBody(catchBody);
            }

            return catchClause;
        }

        public ITryStatement CreateTryStatement(IDeclaredType exceptionType, string exceptionVariableName)
        {
            var tryStatement = this.Factory.CreateStatement("try {} catch($0 $1) {}", exceptionType.GetCLRName(), exceptionVariableName) as ITryStatement;
            if (tryStatement == null) return tryStatement;

            var catchClause = tryStatement.Catches[0] as ISpecificCatchClauseNode;
            if (catchClause == null) return tryStatement;
            var exceptionDeclaration = catchClause.ExceptionDeclaration as ICatchVariableDeclarationNode;
            if (exceptionDeclaration == null) return tryStatement;

            var declaredTypeUsageNode = this.Factory.CreateDeclaredTypeUsageNode(exceptionType);
            exceptionDeclaration.SetDeclaredTypeUsage(declaredTypeUsageNode);

            return tryStatement;
        }

        public IBlock CreateBlock(ITreeNode node)
        {
            return this.Factory.CreateBlock("{ $0 }", node.GetText());
        }
    }
}