/// <copyright>Copyright (c) 2009 CodeGears.net All rights reserved.</copyright>

using System;
using JetBrains.ReSharper.Psi.CSharp.Tree;

namespace CodeGears.ReSharper.Exceptional.Model
{
    /// <summary>Stores data about processed <see cref="IMethodDeclaration"/></summary>
    internal class MethodDeclarationModel : AnalyzeUnitModelBase<IMethodDeclarationNode>
    {
        public MethodDeclarationModel(IMethodDeclarationNode methodDeclaration)
            : base(null, methodDeclaration)
        {
        }

        public override IBlock Contents
        {
            get { return this.Node.Body; }
        }
    }
}