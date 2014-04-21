// Copyright (c) 2009-2010 Cofinite Solutions. All rights reserved.
using JetBrains.ReSharper.Psi.CSharp.Tree;

namespace CodeGears.ReSharper.Exceptional.Model
{
    /// <summary>Stores data about processed <see cref="IMethodDeclaration"/></summary>
    internal class MethodDeclarationModel : AnalyzeUnitModelBase<IMethodDeclaration>
    {
        public MethodDeclarationModel(IMethodDeclaration methodDeclaration)
            : base(null, methodDeclaration)
        {
        }

        public override IBlock Contents
        {
            get { return Node.Body; }
        }
    }
}