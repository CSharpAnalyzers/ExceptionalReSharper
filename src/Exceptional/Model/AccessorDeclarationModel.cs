// Copyright (c) 2009-2010 Cofinite Solutions. All rights reserved.
using JetBrains.ReSharper.Psi.CSharp.Tree;

namespace CodeGears.ReSharper.Exceptional.Model
{
    internal class AccessorDeclarationModel : BlockModelBase<IAccessorDeclaration>
    {
        public AccessorDeclarationModel(IAnalyzeUnit analyzeUnit, IAccessorDeclaration node)
            : base(analyzeUnit, node) { }

        public override IBlock Contents
        {
            get { return Node.Body; }
        }
    }
}