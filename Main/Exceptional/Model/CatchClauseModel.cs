// Copyright (c) 2009-2010 Cofinite Solutions. All rights reserved.
using System;
using CodeGears.ReSharper.Exceptional.Analyzers;
using JetBrains.DocumentModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;

namespace CodeGears.ReSharper.Exceptional.Model
{
    internal class CatchClauseModel : BlockModelBase<ICatchClauseNode>
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
            get { return this.Node is ISpecificCatchClauseNode; }
        }

        public bool HasVariable
        {
            get { return this.VariableModel != null; }
        }

        public override DocumentRange DocumentRange
        {
            get { return this.Node.CatchKeyword.GetDocumentRange(); }
        }

        public CatchClauseModel(IAnalyzeUnit analyzeUnit, ICatchClauseNode catchClauseNode)
            : base(analyzeUnit, catchClauseNode)
        {
            this.IsCatchAll = GetIsCatchAll();
        }

        private bool GetIsCatchAll()
        {
            if (this.Node.ExceptionType == null) return false;

            return this.Node.ExceptionType.GetCLRName().Equals("System.Exception");
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
            if (this.Node is IGeneralCatchClauseNode)
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
                this.VariableModel = new CatchVariableModel(this.AnalyzeUnit, newCatch.ExceptionDeclaration as ICatchVariableDeclarationNode);
            }
            else
            {
                if (this.HasVariable) return;

                if (String.IsNullOrEmpty(variableName))
                {
                    variableName = NameFactory.CatchVariableName(this.Node, GetCatchedException());
                }

                var specificNode = this.Node as ISpecificCatchClauseNode;

                var exceptionType = specificNode.ExceptionTypeUsage as IUserDeclaredTypeUsageNode;
                var exceptionTypeName = exceptionType.TypeName.NameIdentifier.Name;

                var tempTry = this.GetElementFactory().CreateStatement("try {} catch($0 $1) {}", exceptionTypeName, variableName) as ITryStatementNode;
                if (tempTry == null) return;

                var tempCatch = tempTry.Catches[0] as ISpecificCatchClauseNode;
                if (tempCatch == null) return;

                var resultVariable = specificNode.SetExceptionDeclarationNode(tempCatch.ExceptionDeclarationNode);
                this.VariableModel = new CatchVariableModel(this.AnalyzeUnit, resultVariable);
            }
        }
    }
}