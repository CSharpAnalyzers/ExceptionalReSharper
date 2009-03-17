/// <copyright file="GeneralCatchClauseModel.cs" manufacturer="CodeGears">
///   Copyright (c) CodeGears. All rights reserved.
/// </copyright>

using System;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Tree;

namespace CodeGears.ReSharper.Exceptional.Model
{
    internal class GeneralCatchClauseModel : CatchClauseModel
    {
        private IGeneralCatchClauseNode GeneralCatchClauseNode
        {
            get { return this.CatchClauseNode as IGeneralCatchClauseNode; }
        }

        public GeneralCatchClauseModel(MethodDeclarationModel methodDeclarationModel, ICatchClauseNode catchClause) 
            : base(methodDeclarationModel, catchClause) { }

        public override void AddCatchVariable(string variableName)
        {
            if (this.HasVariable) return;
            if (String.IsNullOrEmpty(variableName))
            {
                variableName = SuggestVariableName();
            }

            var codeFactory = new CodeElementFactory(this.GetPsiModule());


            var newCatch = codeFactory.CreateSpecificCatchClause(null, this.CatchClauseNode.Body, variableName);
            if (newCatch == null) return;

            this.GeneralCatchClauseNode.ReplaceBy(newCatch);

            this.CatchClauseNode = newCatch;
            this.VariableModel = new CatchVariableModel(this.MethodDeclarationModel, newCatch.ExceptionDeclaration);
        }

        public override IDeclaredType GetCatchedException()
        {
            return TypeFactory.CreateTypeByCLRName("System.Exception", this.GetPsiModule());
        }

        public override bool Catches(IDeclaredType exception)
        {
            if (exception == null) return false;

            return exception.GetCLRName().Equals("System.Exception");
        }

        public override bool HasExceptionType
        {
            get { return false; }
        }
    }
}