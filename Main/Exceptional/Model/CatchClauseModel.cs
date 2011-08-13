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
            if (this.Node.ExceptionType == null) return false;

            return exception.IsSubtypeOf(this.Node.ExceptionType);
        }

        public bool HasExceptionType
        {
            get { return this.Node is ISpecificCatchClause; }
        }

        public bool HasVariable
        {
            get { return this.VariableModel != null; }
        }

        public override DocumentRange DocumentRange
        {
            get { return this.Node.CatchKeyword.GetDocumentRange(); }
        }

        public CatchClauseModel(IAnalyzeUnit analyzeUnit, ICatchClause catchClauseNode)
            : base(analyzeUnit, catchClauseNode)
        {
            this.IsCatchAll = GetIsCatchAll();
        }

        private bool GetIsCatchAll()
        {
            if (this.Node.ExceptionType == null) return false;

            return this.Node.ExceptionType.GetClrName().ShortName.Equals("System.Exception");
        }

        public override void Accept(AnalyzerBase analyzerBase)
        {
            analyzerBase.Visit(this);

            base.Accept(analyzerBase);
        }

        #region IBlockModel implementation

        public override IDeclaredType GetCatchedException()
        {
            return this.Node.ExceptionType;
        }

        public override IBlock Contents
        {
            get { return this.Node.Body; }
        }

        public override bool CatchesException(IDeclaredType exception)
        {
            return this.ParentBlock.CatchesException(exception);
        }

        #endregion

        public void AddCatchVariable(string variableName)
        {
            if (this.Node is IGeneralCatchClause)
            {
                if (this.HasVariable) return;

                if (String.IsNullOrEmpty(variableName))
                {
                    variableName = NameFactory.CatchVariableName(this.Node, GetCatchedException());
                }

                var codeFactory = new CodeElementFactory(this.GetElementFactory());

                var newCatch = codeFactory.CreateSpecificCatchClause(null, this.Node.Body, variableName);
                if (newCatch == null) return;

                this.Node.ReplaceBy(newCatch);

                this.Node = newCatch;
                this.VariableModel = new CatchVariableModel(this.AnalyzeUnit, newCatch.ExceptionDeclaration as ICatchVariableDeclaration);
            }
            else
            {
                if (this.HasVariable) return;

                if (String.IsNullOrEmpty(variableName))
                {
                    variableName = NameFactory.CatchVariableName(this.Node, GetCatchedException());
                }

                var specificNode = this.Node as ISpecificCatchClause;

                var exceptionType = specificNode.ExceptionTypeUsage as IUserDeclaredTypeUsage;
                var exceptionTypeName = exceptionType.TypeName.NameIdentifier.Name;

                var tempTry = this.GetElementFactory().CreateStatement("try {} catch($0 $1) {}", exceptionTypeName, variableName) as ITryStatement;
                if (tempTry == null) return;

                var tempCatch = tempTry.Catches[0] as ISpecificCatchClause;
                if (tempCatch == null) return;

                var resultVariable = specificNode.SetExceptionDeclaration(tempCatch.ExceptionDeclaration);
                this.VariableModel = new CatchVariableModel(this.AnalyzeUnit, resultVariable);
            }
        }
    }
}