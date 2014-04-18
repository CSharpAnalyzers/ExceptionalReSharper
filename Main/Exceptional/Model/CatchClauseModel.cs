// Copyright (c) 2009-2010 Cofinite Solutions. All rights reserved.
using System;
using CodeGears.ReSharper.Exceptional.Analyzers;
using JetBrains.DocumentModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;

namespace CodeGears.ReSharper.Exceptional.Model
{
    internal class CatchClauseModel : BlockModelBase<ICatchClause>
    {
        public CatchVariableModel VariableModel { get; set; }
        public bool IsCatchAll { get; private set; }

        public bool Catches(IDeclaredType exception)
        {
            if (exception == null) return false;
            if (Node.ExceptionType == null) return false;

            return exception.IsSubtypeOf(Node.ExceptionType);
        }

        public bool HasExceptionType
        {
            get { return Node is ISpecificCatchClause; }
        }

        public bool HasVariable
        {
            get { return VariableModel != null; }
        }

        public override DocumentRange DocumentRange
        {
            get { return Node.CatchKeyword.GetDocumentRange(); }
        }

        public CatchClauseModel(IAnalyzeUnit analyzeUnit, ICatchClause catchClauseNode)
            : base(analyzeUnit, catchClauseNode)
        {
            IsCatchAll = GetIsCatchAll();
        }

        private bool GetIsCatchAll()
        {
            if (Node.ExceptionType == null) return false;

            return Node.ExceptionType.GetClrName().ShortName.Equals("System.Exception");
        }

        public override void Accept(AnalyzerBase analyzerBase)
        {
            analyzerBase.Visit(this);

            base.Accept(analyzerBase);
        }

        #region IBlockModel implementation

        public override IDeclaredType GetCatchedException()
        {
            return Node.ExceptionType;
        }

        public override IBlock Contents
        {
            get { return Node.Body; }
        }

        public override bool CatchesException(IDeclaredType exception)
        {
            return ParentBlock.CatchesException(exception);
        }

        #endregion

        public void AddCatchVariable(string variableName)
        {
            if (Node is IGeneralCatchClause)
            {
                if (HasVariable) return;

                if (String.IsNullOrEmpty(variableName))
                {
                    variableName = NameFactory.CatchVariableName(Node, GetCatchedException());
                }

                var codeFactory = new CodeElementFactory(GetElementFactory());

                var newCatch = codeFactory.CreateSpecificCatchClause(null, Node.Body, variableName);
                if (newCatch == null) return;

                Node.ReplaceBy(newCatch);

                Node = newCatch;
                VariableModel = new CatchVariableModel(AnalyzeUnit, newCatch.ExceptionDeclaration as ICatchVariableDeclaration);
            }
            else
            {
                if (HasVariable) return;

                if (String.IsNullOrEmpty(variableName))
                {
                    variableName = NameFactory.CatchVariableName(Node, GetCatchedException());
                }

                var specificNode = Node as ISpecificCatchClause;

                var exceptionType = specificNode.ExceptionTypeUsage as IUserDeclaredTypeUsage;
                var exceptionTypeName = exceptionType.TypeName.NameIdentifier.Name;

                var tempTry = GetElementFactory().CreateStatement("try {} catch($0 $1) {}", exceptionTypeName, variableName) as ITryStatement;
                if (tempTry == null) return;

                var tempCatch = tempTry.Catches[0] as ISpecificCatchClause;
                if (tempCatch == null) return;

                var resultVariable = specificNode.SetExceptionDeclaration(tempCatch.ExceptionDeclaration);
                VariableModel = new CatchVariableModel(AnalyzeUnit, resultVariable);
            }
        }
    }
}