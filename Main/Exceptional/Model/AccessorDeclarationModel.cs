/// <copyright>Copyright (c) 2009 CodeGears.net All rights reserved.</copyright>

using JetBrains.ReSharper.Psi.CSharp.Tree;

namespace CodeGears.ReSharper.Exceptional.Model
{
    internal class AccessorDeclarationModel : BlockModelBase<IAccessorDeclarationNode>
    {
        public AccessorDeclarationModel(IAnalyzeUnit analyzeUnit, IAccessorDeclarationNode node)
            : base(analyzeUnit, node)
        {
        }
    }
}