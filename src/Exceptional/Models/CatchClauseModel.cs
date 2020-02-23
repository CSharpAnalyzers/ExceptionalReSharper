using System;
using JetBrains.DocumentModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.Psi.VB.Tree;
using ReSharper.Exceptional.Analyzers;
using ReSharper.Exceptional.Utilities;
using IBlock = JetBrains.ReSharper.Psi.CSharp.Tree.IBlock;
using ICatchVariableDeclaration = JetBrains.ReSharper.Psi.CSharp.Tree.ICatchVariableDeclaration;
using IThrowStatement = JetBrains.ReSharper.Psi.CSharp.Tree.IThrowStatement;
using ITryStatement = JetBrains.ReSharper.Psi.CSharp.Tree.ITryStatement;

namespace ReSharper.Exceptional.Models
{
    using System.Linq;

    internal class CatchClauseModel : BlockModelBase<ICatchClause>
    {
        public CatchClauseModel(ICatchClause catchClauseNode, TryStatementModel tryStatementModel, IAnalyzeUnit analyzeUnit)
            : base(analyzeUnit, catchClauseNode)
        {
            ParentBlock = tryStatementModel;
            IsCatchAll = GetIsCatchAll();
        }

        /// <summary>Gets a value indicating whether this is a catch clause which catches System.Exception. </summary>
        public bool IsCatchAll { get; private set; }

        public bool IsExceptionTypeSpecified
        {
            get { return Node is ISpecificCatchClause; }
        }

        public CatchVariableModel Variable { get; set; }

        public bool HasVariable
        {
            get { return Variable != null; }
        }

        public override DocumentRange DocumentRange
        {
            get { return Node.CatchKeyword.GetDocumentRange(); }
        }

        public IDeclaredType CaughtException
        {
            get { return Node.ExceptionType; }
        }

        public override IBlock Content
        {
            get { return Node.Body; }
        }

        public bool Catches(IDeclaredType exception)
        {
            if (exception == null)
                return false;

            if (Node.ExceptionType == null)
                return false;

            return exception.IsSubtypeOf(Node.ExceptionType);
        }

        /// <summary>Analyzes the object and its children. </summary>
        /// <param name="analyzer">The analyzer. </param>
        public override void Accept(AnalyzerBase analyzer)
        {
            analyzer.Visit(this);
            base.Accept(analyzer);
        }

        /// <summary>Checks whether the block catches the given exception. </summary>
        /// <param name="exception">The exception. </param>
        /// <returns><c>true</c> if the exception is caught in the block; otherwise, <c>false</c>. </returns>
        public override bool CatchesException(IDeclaredType exception)
        {
            return ParentBlock.ParentBlock.CatchesException(exception); // Warning: ParentBlock of CatchClause is TryStatement and not the method!
        }

        public void AddCatchVariable(string variableName)
        {
            if (Node is IGeneralCatchClause)
            {
                if (HasVariable)
                    return;

                if (String.IsNullOrEmpty(variableName))
                    variableName = NameFactory.CatchVariableName(Node, CaughtException);

                var codeFactory = new CodeElementFactory(GetElementFactory());

                var newCatch = codeFactory.CreateSpecificCatchClause(null, Node.Body, variableName);
                if (newCatch == null)
                    return;

                Node.ReplaceBy(newCatch);

                Node = newCatch;
                Variable = new CatchVariableModel(AnalyzeUnit, newCatch.ExceptionDeclaration as ICatchVariableDeclaration);
            }
            else
            {
                if (HasVariable)
                    return;

                if (String.IsNullOrEmpty(variableName))
                    variableName = NameFactory.CatchVariableName(Node, CaughtException);

                var specificNode = (ISpecificCatchClause)Node;
                var exceptionType = (IUserDeclaredTypeUsage)specificNode.ExceptionTypeUsage;
                var exceptionTypeName = exceptionType.TypeName.NameIdentifier.Name;

                var tempTry = GetElementFactory().CreateStatement("try {} catch($0 $1) {}", exceptionTypeName, variableName) as ITryStatement;
                if (tempTry == null)
                    return;

                var tempCatch = tempTry.Catches[0] as ISpecificCatchClause;
                if (tempCatch == null)
                    return;

                var resultVariable = specificNode.SetExceptionDeclaration(tempCatch.ExceptionDeclaration);
                Variable = new CatchVariableModel(AnalyzeUnit, resultVariable);
            }
        }

        private bool GetIsCatchAll()
        {
            if (Node.ExceptionType == null)
                return false;

            bool hasConditionalClause = Node.Filter != null;
            bool rethrows = ContainsRethrowStatement(Node.Body);
            bool isSystemException = Node.ExceptionType.GetClrName().FullName == "System.Exception";

            return isSystemException && !hasConditionalClause && !rethrows;
        }

        private bool ContainsRethrowStatement(IBlock body)
        {
            var statements = body.Statements;

            return Enumerable.OfType<IThrowStatement>(statements).Any();
        }
    }
}