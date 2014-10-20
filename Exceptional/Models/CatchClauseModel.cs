using System;
using JetBrains.DocumentModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;
using ReSharper.Exceptional.Analyzers;
using ReSharper.Exceptional.Utilities;

namespace ReSharper.Exceptional.Models
{
    internal class CatchClauseModel : BlockModelBase<ICatchClause>
    {
        public CatchClauseModel(ICatchClause catchClauseNode, TryStatementModel tryStatementModel, IAnalyzeUnit analyzeUnit)
            : base(analyzeUnit, catchClauseNode)
        {
            IsCatchAll = GetIsCatchAll();
            ParentBlock = tryStatementModel;
        }

        public CatchVariableModel Variable { get; set; }

        /// <summary>Gets a value indicating whether this is a catch clause which catches System.Exception. </summary>
        public bool IsCatchAll { get; private set; }

        public bool HasExceptionType
        {
            get { return Node is ISpecificCatchClause; }
        }

        public bool HasVariable
        {
            get { return Variable != null; }
        }

        public override DocumentRange DocumentRange
        {
            get { return Node.CatchKeyword.GetDocumentRange(); }
        }
        
        public override IDeclaredType CaughtException
        {
            get { return Node.ExceptionType; }
        }

        public override IBlock Contents
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

        public override void Accept(AnalyzerBase analyzerBase)
        {
            analyzerBase.Visit(this);

            base.Accept(analyzerBase);
        }

        public override bool CatchesException(IDeclaredType exception)
        {
            return ParentBlock.CatchesException(exception);
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
                if (HasVariable) return;

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

            return Node.ExceptionType.GetClrName().FullName == "System.Exception";
        }
    }
}