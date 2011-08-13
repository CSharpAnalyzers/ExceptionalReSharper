// Copyright (c) 2009-2010 Cofinite Solutions. All rights reserved.
using JetBrains.ReSharper.Psi.CSharp.Tree;

namespace CodeGears.ReSharper.Exceptional.Model
{
    internal class CatchVariableModel : TreeElementModelBase<ICatchVariableDeclaration>
    {
        public ICSharpIdentifier VariableName
        {
            get { return this.Node.Name; }
        }

        public CatchVariableModel(IAnalyzeUnit analyzeUnit, ICatchVariableDeclaration catchVariableDeclaration)
            : base(analyzeUnit, catchVariableDeclaration)
        {
        }
    }
}