// Copyright (c) 2009-2010 Cofinite Solutions. All rights reserved.
using JetBrains.ReSharper.Psi.CSharp.Tree;

namespace CodeGears.ReSharper.Exceptional.Model
{
    internal class AccessorDeclarationModel : BlockModelBase<IAccessorDeclarationNode>
    {
        public AccessorDeclarationModel(IAnalyzeUnit analyzeUnit, IAccessorDeclarationNode node)
            : base(analyzeUnit, node) { }

        public override IBlock Contents
        {
            get { return this.Node.Body; }
        }
    }
}