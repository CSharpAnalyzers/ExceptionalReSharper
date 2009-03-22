/// <copyright file="CatchClauseModel.cs" manufacturer="CodeGears">
///   Copyright (c) CodeGears. All rights reserved.
/// </copyright>

using System;
using System.Collections.Generic;
using CodeGears.ReSharper.Exceptional.Analyzers;
using JetBrains.DocumentModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Naming;
using JetBrains.ReSharper.Psi.Naming.Extentions;
using JetBrains.ReSharper.Psi.Naming.Settings;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.Util;

namespace CodeGears.ReSharper.Exceptional.Model
{
    internal class CatchClauseModel : BlockModelBase<ICatchClauseNode>
    {
        public CatchVariableModel VariableModel { get; set; }
        public bool IsCatchAll { get; private set; }
        public bool IsRethrown { get; set; }

        public bool Catches(IDeclaredType exception)
        {
            if(exception == null) return false;

            if(this.Node is IGeneralCatchClauseNode)
            {
                return exception.GetCLRName().Equals("System.Exception");
            }

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
            this.IsRethrown = GetIsRethrown();
        }

        private bool GetIsRethrown()
        {
            //TODO: Implement
            return true;
        }

        private bool GetIsCatchAll()
        {
            if(this.Node.ExceptionType == null) return false;

            return this.Node.ExceptionType.GetCLRName().Equals("System.Exception");
        }

        //public override void AssignHighlights(CSharpDaemonStageProcessBase process)
        //{
        //    if(this.IsRethrown == false)
        //    {
        //        var treeNode = this.CatchClause.ToTreeNode();
        //        process.AddHighlighting(treeNode.CatchKeyword.GetDocumentRange(), new SwallowedExceptionsHighlighting(this));
        //    }
        //}

        public override void Accept(AnalyzerBase analyzerBase)
        {
            analyzerBase.Visit(this);

            base.Accept(analyzerBase);
        }

        #region IBlockModel implementation

        public override IDeclaredType GetCatchedException()
        {
            if(this.Node is IGeneralCatchClauseNode)
            {
                return TypeFactory.CreateTypeByCLRName("System.Exception", this.AnalyzeUnit.GetPsiModule());
            }

            return this.Node.ExceptionType;
        }

        public override bool CatchesException(IDeclaredType exception)
        {
            return this.ParentBlock.CatchesException(exception);
        }
        #endregion

        public void AddCatchVariable(string variableName)
        {
            if(this.Node is IGeneralCatchClauseNode)
            {
                if (this.HasVariable) return;
                if (String.IsNullOrEmpty(variableName))
                {
                    variableName = SuggestVariableName();
                }

                var codeFactory = new CodeElementFactory(this.AnalyzeUnit.GetPsiModule());


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
                    variableName = SuggestVariableName();
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

        public string SuggestVariableName()
        {
            var namesCollection = NameSuggestionUtil.CreateEmptyCollection(
                this.Node.Language, PluralityKinds.Single, true, this.Node.GetSolution());
            namesCollection.Add(this.GetCatchedException(), PluralityKinds.Single);

            var roots = new List<NameRoot>(namesCollection.GetRoots("e", PredefinedPrefixPolicy.Preserve));
            var newNames = new List<string>(NamingManager.SuggestUniqueShortNames(roots, this.Node, NamedElementKinds.Locals, ScopeKind.LocalSelfScoped));

            return newNames.Count > 0 ? newNames.GetLast() : "exception";
        }
    }
}